(function () {
    const btnAgregarNuevaCategoria = document.getElementById("btnAgregarNuevaCategoria");
    let dataTable;
    let filaSeleccionada;

    const MODEL = {
        id: 0,
        nombre: ""
    }

    inicio();
    function inicio() {
        inicializarDataTable();
        dataTable.on("init", () => {
            eventos();
        });

    }

    function eventos() {
        btnAgregarNuevaCategoria.addEventListener("click", () => { showModalHandler("Crear Categoria") });

        document.getElementById("modalBtnAceptar").addEventListener("click", createCategoria);

        //const btnsEditarCategoria = document.querySelectorAll(".btnEditarCategoria");
        //btnsEditarCategoria.forEach((item) => {
        //    item.addEventListener("click", updateCategoria);
        //});

        $("#table").on("click", ".btnEditarCategoria", btnUpdateHandler);

        $("#table").on("click",".btnEliminarCategoria", btnEliminarHandler);

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
                url: $.MisUrls.url._ObtenerCategorias ,
                type: "GET",
                datatype: "json"
            },
            columns: [
                //{ data: "id", render: imagen => `<img src="/imagenes/mesa2.jpg" class="img-fluid" width="100px"></img>`, width: "10%" },
                { data: 'id', width: "5%" },
                { data: 'nombre', width: "5%" },
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
            <button class="btn btn-danger btnEliminarCategoria" data-id="${data}" title="Eliminar Categoria">
                <i class="fa-solid fa-trash-can-arrow-up"></i>
            </button>
            <button class="btn btn-primary btnEditarCategoria" data-id="${data}" title="Actualizar Categoria">
                <i class="fa-solid fa-file-pen"></i>
            </button>`;
        }
    };

    async function createCategoria() {
        if (!$("#formCreateCategoria").valid()) return;


        const formData = new FormData();

        const modelo = getModelCategoria();

        formData.append("modelo", JSON.stringify(modelo));

        try {
            let responseJson;
            if (modelo.id === 0) {
                
                responseJson = await enviarPeticion($.MisUrls.url._CrearCategoria, "POST", formData);
                if (!responseJson) return;
                toastr.success("Exito", "Creado exitosamente");
                dataTable.row.add(responseJson).draw(false);
            } else {
                responseJson = await enviarPeticion($.MisUrls.url._ActualizarCategoria, "PATCH", formData);
                if (!responseJson) return;
                toastr.success("Exito", "Actualizado exitosamente");
                dataTable.row(filaSeleccionada).data(responseJson).draw(false);
            }
            if (responseJson) {

                //mostrarMensajeExito("Operación completada correctamente");
                
                $("#modalCrearCategoria").modal("hide");
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
        showModalHandler("Actualizar Categoria", data);

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
            preConfirm: () => removeCategoria(data.id, fila)
        });
    }

    async function removeCategoria(id, fila) {

        const formData = new FormData();


        //const id = $(this).closest(".btnEliminarCategoria").data("id");

        formData.append("modelo", JSON.stringify(id));


        try {
            let responseJson = await enviarPeticion($.MisUrls.url._EliminarCategoria, "Delete", formData);


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

    function showModalHandler(title, modelo = MODEL) {

        const { id, nombre } = modelo;

        $("#id").val(parseInt(id));
        $("#Nombre").val(nombre);
        $("#modalTitle").text(title);
        $("#modalCrearCategoria").modal("show");
    }

    function getModelCategoria() {
        return {
            id: parseInt($("#id").val()),
            nombre: $("#Nombre").val()
        }
    }

})();