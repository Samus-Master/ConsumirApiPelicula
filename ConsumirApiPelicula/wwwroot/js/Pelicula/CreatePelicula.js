
(function () {

    const btnAgregarNuevaPelicula = document.getElementById("btnAgregarNuevaPelicula");
    let dataTable;
    let filaSeleccionada;

    const MODEL = {
        id: 0,
        nombre: "",
        descripcion: "",
        clasificacion: 3,
        rutaImagen: "",
        categoriasId: []
    }

    inicio();
    function inicio() {
        incializarComponentes();
        inicializarDataTable();
        dataTable.on("init", () => {
            eventos();
        });

    }

    function incializarComponentes() {
        $('#PeliculaDTO_CategoriasId').select2({
            theme: "bootstrap-5",
            width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
            placeholder: $(this).data('placeholder'),
            closeOnSelect: false,
            tags: true
        });
    }

    function eventos() {
        btnAgregarNuevaPelicula.addEventListener("click", () => { showModalHandler("Crear Pelicula") });

        document.getElementById("modalBtnAceptar").addEventListener("click", createPelicula);

        $("#table").on("click", ".btnEditarPelicula", btnUpdateHandler);

        $("#table").on("click", ".btnEliminarPelicula", btnEliminarHandler);

        //const btnsEditarPelicula = document.querySelectorAll(".btnEditarPelicula");
        //btnsEditarPelicula.forEach((item) => {
        //    item.addEventListener("click", updatePelicula);
        //});

        //const btnsEliminarPelicula = document.querySelectorAll(".btnEliminarPelicula");
        //btnsEliminarPelicula.forEach((item) => {
        //    item.addEventListener("click", btnEliminarHandler);
        //});

        mostrarImagenPreview();
    }

    function inicializarDataTable() {

        dataTable = $("#table").DataTable({
            processing: true,
            responsive: true,
            destroy: true,
            pageLength: 6,
            ajax: {
                url: $.MisUrls.url._ObtenerPeliculasIncludeCategoria,
                type: "GET",
                datatype: "json"
            },
            columns: [
                //{ data: "id", render: imagen => `<img src="/imagenes/mesa2.jpg" class="img-fluid" width="100px"></img>`, width: "10%" },
                { data: 'id', width: "5%" },
                {
                    data: "rutaImagen", render: data => `<img class="img w-100 h-25" src="${data}"></img>`, width: "10%"
                },
                { data: 'nombre', width: "5%" },
                {
                    data: 'categorias', render: data => `${data.map(item => ` ${item.nombre}`)}`
                },
                { data: "id", render: renderAcciones, width: "10%" }
            ],
            language: {
                //url: "//cdn.datatables.net/plug-ins/1.10.24/i18n/Spanish.json"
                loadingRecords: ""
            },
            columnDefs: [{ className: "text-start", targets: "_all" }]
        });

        function renderAcciones(data) {
            return `
            <button class="btn btn-danger btnEliminarPelicula" data-id="${data}" title="Eliminar Pelicula">
                <i class="fa-solid fa-trash-can-arrow-up"></i>
            </button>
            <button class="btn btn-primary btnEditarPelicula" data-id="${data}" title="Actualizar Pelicula">
                <i class="fa-solid fa-file-pen"></i>
            </button>`;
        }
    };

    async function createPelicula() {
        if (!$("#formCreatePelicula").valid()) return;

        const inputFoto = document.getElementById("PeliculaDTO_RutaImagen");
        const formData = new FormData();

        const modelo = getModelCategoria();


        formData.append("archivos", inputFoto.files[0]);
        formData.append("modelo", JSON.stringify(modelo));


        try {
            let responseJson;
            if (modelo.id === 0) {

                responseJson = await enviarPeticion($.MisUrls.url._CrearPelicula, "POST", formData);

                if (!responseJson) return;
                toastr.success("Exito", "Creado exitosamente");
                dataTable.row.add(responseJson).draw(false);
            } else {
                responseJson = await enviarPeticion($.MisUrls.url._ActualizarPelicula, "PUT", formData);
                if (!responseJson) return;
                toastr.success("Exito", "Actualizado exitosamente");
                dataTable.row(filaSeleccionada).data(responseJson).draw(false);
            }
            if (responseJson) {


                $("#modalCrearPelicula").modal("hide");
            }

        } catch (error) {
            //mostrarMensajeError(error.message || "Ocurrió un error inesperado.");
        } finally {
            //$(".modal-content").LoadingOverlay("hide");

        }
    }

    function btnUpdateHandler() {
        $("#PeliculaDTO_RutaImagen").rules('remove', "required");

        filaSeleccionada = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = dataTable.row(filaSeleccionada).data();
        showModalHandler("Actualizar Pelicula", data);

    }

    async function btnEliminarHandler() {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = dataTable.row(fila).data();

        Swal.fire({
            title: 'Aviso',
            text: "¿Desea eliminar la categoria? ",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'No, cancelar',
            confirmButtonClass: "btn-danger",
            reverseButtons: true,
            showCloseButton: true,
            allowOutsideClick: false,
            allowEscapeKey: false,
            allowEnterKey: false,
            preConfirm: () => removePelicula(data.id, fila)
        });
    }

    async function removePelicula(id, fila) {

        const formData = new FormData();


        //const id = $(this).closest(".btnEliminarPelicula").data("id");

        formData.append("modelo", JSON.stringify(id));


        try {
            let responseJson = await enviarPeticion($.MisUrls.url._EliminarPelicula, "Delete", formData);

            if (!responseJson) return;

            dataTable.row(fila).remove().draw();

            //if (!responseJson) return;
            toastr.info("", "Eliminado exitosamente");


        } catch (error) {
            toastr.error("", error);
        }
    }

    async function enviarPeticion(url, method, formData) {
        const response = await fetch(url, { method, body: formData });


        if (!response.ok) {
            // Si no es ok, intentamos obtener el error en formato JSON
            try {
                let datos = await response.json();

                Swal.fire({
                    title: datos.titulo,
                    text: datos.mensaje,
                    icon: 'error',
                    showConfirmButton: true,
                    showCloseButton: true,
                    allowOutsideClick: false,
                    allowEscapeKey: false,
                    allowEnterKey: false
                });

                return response.json();
            } catch (e) {
                throw new Error("Error inesperado: respuesta inválida del servidor");
            }

            // Lanzar el mensaje del error si está presente
            //throw new Error(errorResponse.message || "Error inesperado");
        }

        return response.json();
    }

    function showModalHandler(title, modelo = MODEL) {
        const { id, nombre, descripcion, categoriasId, categorias, rutaImagen, clasificacion } = modelo;

        const categoriassId = categorias === undefined ? categoriasId : categorias.map(categoria => categoria.id);

        //anadir validacion de la imagen
        $("#PeliculaDTO_RutaImagen").rules("add", {
            required: true
        });

        $("#PeliculaDTO_Id").val(parseInt(id));
        $("#PeliculaDTO_Nombre").val(nombre);
        $("#PeliculaDTO_Descripcion").val(descripcion);
        $("#PeliculaDTO_Clasificacion").val(clasificacion);

        //en el datatable se llama distintos mis categorias
        if (categoriasId) {
            $("#PeliculaDTO_CategoriasId").val(categoriasId).trigger("change");
        } else {
            $("#PeliculaDTO_CategoriasId").val(categorias.map(categoria => categoria.id)).trigger("change");
            
        }

        $("#imgPreview").attr("src", rutaImagen);

        $("#modalCrearPelicula").modal("show");
        $("#modalTitle").text(title);

        $('#modalCrearPelicula').on('shown.bs.modal', function () {
            // Aquí puedes ejecutar tu lógica después de que el modal esté completamente visible
            $("#formCreatePelicula").valid();
        });

        

    }

    function getModelCategoria() {
        return {
            id: parseInt($("#PeliculaDTO_Id").val()),
            nombre: $("#PeliculaDTO_Nombre").val(),
            descripcion: $("#PeliculaDTO_Descripcion").val(),
            clasificacion: $("#PeliculaDTO_Clasificacion").val(),
            categoriasId: $("#PeliculaDTO_CategoriasId").val(),
            rutaImagen: $("#PeliculaDTO_RutaImagen").val()
        }
    }

    function mostrarImagenPreview() {
        $("#PeliculaDTO_RutaImagen").change(function () {

            let file = event.target.files[0];

            // Si hay un archivo seleccionado y es una imagen
            if (file && file.type.startsWith('image/')) {
                // Crea un FileReader para leer el archivo
                const reader = new FileReader();

                // Evento que se dispara cuando se termina de leer el archivo
                reader.onload = function (e) {
                    // Establece la URL de la imagen cargada como el src del elemento <img>
                    const imagePreview = document.getElementById('imgPreview');
                    imagePreview.src = e.target.result;
                    /*  imagePreview.style.display = 'block'; // Muestra la imagen*/
                };

                // Lee el archivo como una URL de datos
                reader.readAsDataURL(file);

            }
        });

    }


})();