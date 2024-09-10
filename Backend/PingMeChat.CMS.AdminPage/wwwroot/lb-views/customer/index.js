$(function () {
    var columns = [
        {
            targets: 0,
            data: null,
            sortable: false,
            searchable: false,
            render: function (data, type, row, meta) {
                return meta.row + meta.settings._iDisplayStart + 1;
            }
        },
        { targets: 1, data: "id", visible: false },
        { targets: 2, data: "fullName", sortable: false },
        { targets: 3, data: "phoneNumber", sortable: false, render: (data, type, row, meta) => formatVietnamesePhoneNumber(data) },
        { targets: 4, data: "address", sortable: false, },
        {
            targets: 5,
            data: null,
            sortable: false,
            autoWidth: true,
            width: "160px",
            defaultContent: '',
            render: (data, type, row, meta) => {
                return `<div class="d-flex justify-content-center">
                        <a class="btn btn-outline-info btn-sm btn-view-detail-modal me-2" data-view-detail-id="${row.id}"  data-bs-toggle="modal" data-bs-target="#ViewDetailCustomerModal"><i class="fas fa-eye mx-1"></i></a>
                        <a class="btn btn-outline-warning btn-sm btn-edit-modal me-2" data-edit-id="${row.id}"  data-bs-toggle="modal" data-bs-target="#UpdateCustomerModal"><i class="fas fa-user-edit mx-1"></i></a>
                        <a class="btn btn-outline-danger btn-sm btn-delete-modal" data-delete-id="${row.id}" data-bs-toggle="modal" data-bs-target="#DeleteCustomerModal"><i class="fas fa-window-close mx-1"></i></a>
                    </div>`;

            }
        }
    ];
    var url = '/customer/Pagination';
    const table = DataTableUtils.init('#tableId', url, columns, '#form-filter');
    $(document).on('click', '.btn-reset', function () {
        DataTableUtils.resetFilters('#form-filter', table);
    });


    // Xử lý sự kiện click nút sửa
    $(document).on('click', '.btn-edit-modal', function () {
        var id = $(this).data('edit-id');
        let url = "/Customer/Update"
        let type = "GET"
        let data = { id: id }
        callAPI(url, type, data, (res) => {
            $("#UpdateCustomerModal .modal-content").html(res)

        }, (xhr, status, error) => {
            if (xhr.responseJSON.statusCode == 500) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                return;
            }
            toastError(xhr.responseJSON.Result);
        })

    });

    // Xử lý sự kiện click lưu thay đổi trong modal sửa
    $(document).on('click', '#UpdateCustomerModal .save-button', function (e) {
        e.preventDefault();
        if (!validateForm("#UpdateCustomerModal")) return
        let form = $('#UpdateCustomerModal form');
        let data = form.convertFormToFormData()
        let url = "/Customer/Update"
        let type = "PUT"
        callAPI(url, type, JSON.stringify(data), (res) => {
            if (res.statusCode === 200) {
                toastSuccess(res.message)
                $('#UpdateCustomerModal').modal('hide');
                table.ajax.reload()
            } else {
                toastError(res.message)
            }

        }, (xhr, status, error) => {
            if (xhr.responseJSON.statusCode == 500) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                return;
            }
            toastError(xhr.responseJSON.Result);
        })

    });

    // Xử lý sự kiện click nút xóa
    $(document).on('click', '.btn-delete-modal', function () {
        var id = $(this).data('delete-id');
        var modal = $('#DeleteCustomerModal');
        modal.find('.delete-button').attr('data-delete-id', id);
    });

    //Xác nhận xoá
    $(document).on('click', '#confirmDeleteButton', function () {
        let id = $(this).attr('data-delete-id');
        let url = "/Customer/Delete?id=" + id
        let type = "DELETE"
        let data = { id: id }
        callAPI(url, type, data, (res) => {
            if (res.statusCode === 200) {
                toastSuccess(res.message)
                table.ajax.reload()
                $('#DeleteCustomerModal .close').click()
                return;
            }
            toastError(res.message)
        }, (xhr, status, error) => {
            LoadingOverlay.hide();
            if (xhr.responseJSON.statusCode == 500) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                return;
            }
            toastError(xhr.responseJSON.Result);
        })

    });

    $('#DeleteCustomerModal').on('hidden.bs.modal', function () {
        $(this).find('#confirmDeleteButton').removeAttr('data-delete-id')
    });

    // Xử lý sự kiện click nút xem
    $(document).on('click', '.btn-view-detail-modal', function () {
        var id = $(this).data('view-detail-id');
        // Thêm logic để mở modal xem chi tiết
        let url = "/Customer/Detail"
        let type = "GET"
        let data = { id: id }
        callAPI(url, type, data, (res) => {
            $("#ViewDetailCustomerModal .modal-dialog").html(res)

        }, (xhr, status, error) => {
            if (xhr.responseJSON.statusCode == 500) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                return;
            }
            toastError(xhr.responseJSON.Result);
        })
    });


    // Tạo hàm để validate form
    function validateForm(modalId) {
        let isValid = true;
        isValid = validateField($(`${modalId} #FullName`), {
            minLength: 3,
            maxlength: 225,
        }) && isValid;
        isValid = validateField($(`${modalId} #PhoneNumber`), {
            required: true,
            phone: true
        }) && isValid;
        isValid = validateField($(`${modalId} #Address`), {
            required: true,
            minLength: 3,
            maxLength: 225
        }) && isValid;

        return isValid;
    }


    // Xử lý sự kiện submit form tạo mới
    $(document).on('submit', '#CreateCustomerForm', function (e) {
        e.preventDefault();
        if (!validateForm("#CreateCustomerModal")) return
        let url = "/Customer/Create"
        let type = "POST"
        let data = $(this).convertFormToFormData()
        callAPI(url, type, JSON.stringify(data), (res) => {
            if (res.statusCode == 201) {
                toastSuccess(res.message);
                //ẩn modal và ẩn luôn modal-backdrop
                $("#CreateCustomerModal .btn-close").click()
                table.ajax.reload()
                $('#CreateCustomerForm').clearForm();
            } else {
                toastError(res.message)
            }
        }, (xhr, status, error) => {
            LoadingOverlay.hide(); // Ẩn loading
            toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            console.error("Error:", error);
        })
    });


});
