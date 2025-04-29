(function () {
    const btnAgregarNuevoSlider = document.getElementById("btnAgregarNuevoSlider");
    let dataTable;
    let filaSeleccionada;

    const MODEL = {
        id: 0,
        estado: true,
        peliculaId: 0
    }

    inicio();
    function inicio() {
        //incializarComponentes();
        inicializarDataTable();
        dataTable.on("init", () => {
            eventos();
        });

    }

    function incializarComponentes() {
        $('#SliderDTO_PeliculaId').select2({
            theme: "bootstrap-5",
            width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
            placeholder: $(this).data('placeholder'),
            closeOnSelect: false,
            tags: true
        });
    }


    function eventos() {
        btnAgregarNuevoSlider.addEventListener("click", () => { showModalHandler("Crear Slider") });

        document.getElementById("modalBtnAceptar").addEventListener("click", createCategoria);

        //const btnsEditarCategoria = document.querySelectorAll(".btnEditarCategoria");
        //btnsEditarCategoria.forEach((item) => {
        //    item.addEventListener("click", updateCategoria);
        //});

        $("#table").on("click", ".btnEditarSlider", btnUpdateHandler);

        $("#table").on("click", ".btnEliminarSlider", btnEliminarHandler);

        //const btnsEliminarCategoria = document.querySelectorAll(".btnEliminarCategoria");
        //btnsEliminarCategoria.forEach((item) => {
        //    item.addEventListener("click", btnEliminarHandler);
        //});
    }

    function inicializarDataTable() {

        dataTable = $("#table").DataTable({
            processing: true,
            responsive: true,
            destroy: true,
            pageLength: 6,
            ajax: {
                url: $.MisUrls.url._ObtenerSliders,
                type: "GET",
                datatype: "json"
            },
            columns: [
                //{ data: "id", render: imagen => `<img src="/imagenes/mesa2.jpg" class="img-fluid" width="100px"></img>`, width: "10%" },
                { data: 'id', width: "5%" },
                { data: 'pelicula.nombre', width: "5%" },
                {
                    data: 'estado', render: estado => `${estado === true ? 'activo' : 'inactivo'}`, width: "5%"
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
            <button class="btn btn-danger btnEliminarSlider" data-id="${data}" title="Eliminar Slider">
                <i class="fa-solid fa-trash-can-arrow-up"></i>
            </button>
            <button class="btn btn-primary btnEditarSlider" data-id="${data}" title="Actualizar Slider">
                <i class="fa-solid fa-file-pen"></i>
            </button>`;
        }
    };

    async function createCategoria() {
        if (!$("#formCreateSlider").valid()) return;


        const formData = new FormData();

        const modelo = getModelSlider();


        formData.append("modelo", JSON.stringify(modelo));

        try {
            let responseJson;
            if (modelo.id === 0) {

                responseJson = await enviarPeticion($.MisUrls.url._CrearSlider, "POST", formData);
                if (!responseJson) return;
                toastr.success("Exito", "Creado exitosamente");
                dataTable.row.add(responseJson).draw(false);
            } else {

                responseJson = await enviarPeticion($.MisUrls.url._ActualizarSlider, "PATCH", formData);
                if (!responseJson) return;
                toastr.success("Exito", "Actualizado exitosamente");
                dataTable.row(filaSeleccionada).data(responseJson).draw(false);
            }
            if (responseJson) {

                //mostrarMensajeExito("Operación completada correctamente");

                $("#modalCrearSlider").modal("hide");
            }

        } catch (error) {
            //mostrarMensajeError(error.message || "Ocurrió un error inesperado.");
        } finally {
            //$(".modal-content").LoadingOverlay("hide");

        }
    }

    function btnUpdateHandler() {

        filaSeleccionada = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = dataTable.row(filaSeleccionada).data();
        showModalHandler("Actualizar Slider", data, false);

    }

    function disableDropdownCategoriaWithPelicula(option = false) {
        if (option)
            document.getElementById("dropdownCategoriasWithPelicula").classList.add("d-none");
        else document.getElementById("dropdownCategoriasWithPelicula").classList.remove("d-none");
    }

    async function btnEliminarHandler() {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = dataTable.row(fila).data();

        Swal.fire({
            title: 'Aviso',
            text: "¿Desea eliminar la pelicula del slider? ",
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
            preConfirm: () => removeSlider(data.id, fila)
        });
    }

    async function removeSlider(id, fila) {

        const formData = new FormData();


        //const id = $(this).closest(".btnEliminarCategoria").data("id");

        formData.append("modelo", JSON.stringify(id));


        try {
            let responseJson = await enviarPeticion($.MisUrls.url._EliminarSlider, "Delete", formData);


            if (!responseJson) return;

            dataTable.row(fila).remove().draw();

            toastr.success("", "Eliminado exitosamente");

        } catch (error) {

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

    function showModalHandler(title, modelo = MODEL, showDropdownCategoriaWithPelicula = true) {
        const { id, estado, peliculaId } = modelo;

        if (showDropdownCategoriaWithPelicula)
            disableDropdownCategoriaWithPelicula(false);
        else
            disableDropdownCategoriaWithPelicula(true);

        $("#id").val(parseInt(id));
        $("#SliderDTO_Estado").prop("checked", estado);
        $("#SliderDTO_PeliculaId").val(parseInt(peliculaId));



        $("#modalTitle").text(title);
        $("#modalCrearSlider").modal("show");
    }

    function getModelSlider() {
        return {
            id: parseInt($("#id").val()),
            estado: $("#SliderDTO_Estado").prop('checked'),
            peliculaId: parseInt($("#SliderDTO_PeliculaId").val()
            )
        }
    }

})();