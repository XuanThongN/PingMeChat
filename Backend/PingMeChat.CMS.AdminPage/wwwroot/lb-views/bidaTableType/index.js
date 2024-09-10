$(function () {
    var columns = [
        {
            targets: 0,
            data: null,
            sortable: false,
            render: (data, type, row, meta) => {
                return meta.row + 1; // Tính toán số thứ tự dựa trên chỉ số hàng
            }
        },
        { targets: 1, data: "id", visible: false },
        { targets: 2, data: "name", sortable: false },
        { targets: 3, data: "price", className: "text-right", sortable: false, render: numberFormatCurrency() },
        { targets: 4, data: "createdDateString", className: "text-center", sortable: false },
        {
            targets: 5,
            data: null,
            sortable: false,
            autoWidth: true,
            width: "180px",
            defaultContent: '',
            render: (data, type, row, meta) => {
                return `<div class="d-flex justify-content-center">
                        <a class="btn btn-outline-info btn-sm btn-view-detail-modal" data-view-detail-id="${row.id}"  data-bs-toggle="modal" data-bs-target="#ViewDetailBidaTableTypeModal"><i class="fas fa-eye mx-1"></i></a>
                        <a class="btn btn-outline-warning btn-sm btn-edit-modal mx-1" data-edit-id="${row.id}"><i class="fas fa-user-edit mx-1"></i></a>
                        <a class="btn btn-outline-danger btn-sm btn-delete-modal" data-delete-id="${row.id}" data-bs-toggle="modal" data-bs-target="#DeleteBidaTableTypeModal"><i class="fas fa-window-close mx-1"></i></a>
                    </div>`;
            }
        }
    ];

    const table = DataTableUtils.init('#BidaTableType-table', '/BidaTableType/Pagination', columns, '#form-filter');
    $(document).on('click', '.btn-reset', function () {
        DataTableUtils.resetFilters('#form-filter', table);
    });
    // Bắt sự kiện modal được show
    $(document).on('show.bs.modal', '#createModal, #UpdateBidaTableTypeModal', function () {
        // Khởi tạo input tiền tệ chỉ trong modal hiện tại
        MoneyUtils.initMoneyInput($('.money-input'));
    });



    // Xử lý sự kiện click nút sửa
    $(document).on('click', '.btn-edit-modal', function () {
        var id = $(this).data('edit-id');
        let url = "/BidaTableType/Edit"
        let type = "GET"
        let data = { id: id }
        callAPI(url, type, data, (res) => {
            $("#UpdateBidaTableTypeModal .modal-content").html(res)

            $('#UpdateBidaTableTypeModal').modal('show'); // Mục đích để hiển thị modal ở đây là để khởi tạo Cleave.js cho input tiền tệ
        }, () => {
            console.log("Error");
        })

    });

    // Xử lý sự kiện click lưu thay đổi trong modal sửa
    $(document).on('click', '#UpdateBidaTableTypeModal .save-button', function () {
        if (!validateUpdateForm()) return
        let form = $('#UpdateBidaTableTypeModal #bidaTableTypeForm');
        let data = form.convertFormToFormData();
        let url = "/BidaTableType/Edit"
        let type = "POST"
        callAPI(url, type, JSON.stringify(data), (res) => {
            if (res.statusCode === 200) {
                toastSuccess(res.message)
                $('#UpdateBidaTableTypeModal').modal('hide');
                table.ajax.reload()
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

    // Xử lý sự kiện click nút xóa
    $(document).on('click', '.btn-delete-modal', function () {
        var id = $(this).data('delete-id');
        var modal = $('#DeleteBidaTableTypeModal');
        modal.find('.delete-button').attr('data-delete-id', id);
    });

    //Xác nhận xoá
    $(document).on('click', '#confirmDeleteButton', function () {
        let id = $(this).attr('data-delete-id');
        let url = "/BidaTableType/Delete?id=" + id
        let type = "DELETE"
        let data = { id: id }
        callAPI(url, type, data, (res) => {
            if (res.statusCode === 200) {
                toastSuccess(res.message)
                table.ajax.reload()
                $('#DeleteBidaTableTypeModal .close').click()
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
    // Xử lý sự kiện click nút xem
    $(document).on('click', '.btn-view-detail-modal', function () {
        var id = $(this).data('view-detail-id');
        // Thêm logic để mở modal xem chi tiết
        let url = "/BidaTableType/Detail"
        let type = "GET"
        let data = { id: id }
        callAPI(url, type, data, (res) => {
            $("#ViewDetailBidaTableTypeModal .modal-dialog").html(res)

        }, () => {
            console.log("Error");
        })

    });

    // Tạo hàm để validate form
    function validateForm() {
        let isValid = true;

        // Validate mã bàn
        isValid = validateField($('#Name'), {
            required: true,
            minLength: 3,
            maxLength: 50
        }) && isValid;

        // Validate cho trường tiền tệ
        isValid = validateField($('#Price'), {
            required: true,
            min: 1000,
            max: 1000000000
        }) && isValid;

        return isValid;
    }

    // Tạo hàm để validate update form
    function validateUpdateForm() {
        let isValid = true;

        // Validate mã bàn
        isValid = validateField($("#UpdateBidaTableTypeModal #Name"), {
            required: true,
            minLength: 3,
            maxLength: 50
        }) && isValid;

        // Validate cho trường tiền tệ
        isValid = validateField($('#UpdateBidaTableTypeModal #Price'), {
            required: true,
            number: true,
            min: 1000,
            max: 1000000000
        }) && isValid;

        return isValid;
    }
    // Xử lý sự kiện submit form tạo mới
    $(document).on('submit', '#CreateBidaTableTypeForm', function (e) {
        e.preventDefault();
        if (validateForm()) {
            let url = "/BidaTableType/Create"
            let type = "POST"
            let data = $(this).convertFormToFormData()
            callAPI(url, type, JSON.stringify(data), (res) => {
                if (res.statusCode == 400) {
                    toastError(res.message)
                    return;
                }
                //ẩn modal và ẩn luôn modal-backdrop
                $("#createModal").modal('hide')
                $("#createModal .btn-close").click()
                table.ajax.reload()
                resetFormAfterSubmit();

            }, (xhr, status, error) => {
                LoadingOverlay.hide();
                if (xhr.responseJSON.statusCode == 500) {
                    toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                    return;
                }
                toastError(xhr.responseJSON.Result);
            })
        }
    });

    // Thêm sự kiện blur để validate realtime
    $('#Name, #Price').on('blur', function () {
        validateField($(this), {
            required: true,
            minLength: $(this).attr('id') === 'Name' ? 3 : undefined,
            maxLength: $(this).attr('id') === 'Name' ? 50 : undefined
        });
    });

    function resetFormAfterSubmit() {
        $('#CreateBidaTableTypeForm')[0].reset();
        $('.error-message').remove();
    }
});