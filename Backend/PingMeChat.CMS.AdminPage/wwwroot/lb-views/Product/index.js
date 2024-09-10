$(function () {
    var columns = [
        { targets: 0, data: "id", visible: false },
        { targets: 1, data: "code", sortable: false },
        { targets: 2, data: "name", sortable: false, },
        { targets: 3, data: "cost", sortable: false, render: numberFormatCurrency() },
        { targets: 4, data: "price", sortable: false, render: numberFormatCurrency() },
        {
            targets: 5,
            data: null,
            sortable: false,
            autoWidth: true,
            defaultContent: '',
            render: (data, type, row, meta) => {
                return `<div class="d-flex justify-content-center">
                             <a class="btn btn-outline-danger btn-sm btn-view-detail-modal" data-view-detail-id="${row.id}">
                                  <i class="fas fa-eye mx-1"></i>
                             </a>
                            <a class="btn btn-outline-warning btn-sm btn-edit-modal mx-1" data-edit-id="${row.id}"><i class="fas fa-user-edit mx-1"></i></a>
                        </div>
                         `

            }
        }
    ];

    const table = DataTableUtils.init('#Product-table', '/Product/Pagination', columns, '#form-filter');
    $(document).on('click', '.btn-reset', function () {
        DataTableUtils.resetFilters('#form-filter', table);
    });

    // Xử lý sự kiện click nút sửa
    $(document).on('click', '.btn-edit-modal', function () {
        var id = $(this).data('edit-id');
        let url = "/Product/Update"
        let type = "GET"
        let data = { id: id }
        callAPI(url, type, data, (res) => {
            $("#UpdateProductModal .modal-content").html(res)

            $('#UpdateProductModal').modal('show'); // Mục đích để hiển thị modal ở đây là để khởi tạo Cleave.js cho input tiền tệ
        }, () => {
            console.log("Error");
        })

    });

    // Xử lý sự kiện click nút xóa
    $(document).on('click', '.btn-delete-modal', function () {
        var id = $(this).data('delete-id');
        // Thêm logic để xác nhận và xóa bản ghi
    });

    // Xử lý sự kiện click nút xem
    $(document).on('click', '.btn-view-detail-modal', function () {
        var id = $(this).data('view-detail-id');
        // Thêm logic để mở modal xem chi tiết
        let url = "/Product/Detail"
        let type = "GET"
        let data = { id: id }
        callAPI(url, type, data, function (res) {
            let modal = $("#ViewDetailProductModal")
            modal.find('.modal-dialog').html(res)   // Thay đổi nội dung modal
            modal.modal('show') // Mục đích để hiển thị modal ở đây là để khởi tạo Cleave.js cho input tiền tệ

        }, function (xhr, status, error) {
            LoadingOverlay.hide(); // Ẩn loading
            // Error callback
            toastError(xhr.responseJSON.message);
        })
    });

    // Tạo hàm để validate form
    function validateForm(form) {
        let isValid = true;

        // Validate tên bàn
        isValid = validateField($(form).find('#Name'), {
            required: true,
            minLength: 3,
            maxLength: 50
        }) && isValid;

        // Validate giá nhập
        isValid = validateField($(form).find('#Price'), {
            required: true,
            money: true,
            min: 1000,
        }) && isValid;

        return isValid;
    }

    $('#CreateProductForm').on('submit', function (e) {
        e.preventDefault();
        let form = this;
        if (!validateForm(form)) return;
            var formData = $(form).convertFormToFormData()
            var jsonData = JSON.stringify(formData);
            var url = "/Product/Create";
            LoadingOverlay.show(); // Hiển thị loading
            submitForm(url, jsonData,
                function (response) {
                    LoadingOverlay.hide(); // Ẩn loading
                    if (response.statusCode == 201) {
                        // Success callback
                        toastSuccess(response.message);

                        // Đóng modal
                        $('#CreateProductModal').modal('hide');
                        table.ajax.reload();
                        return;
                    }
                    // Error callback

                    toastError(response.message);
                },
                function (xhr, status, error) {
                    if (xhr.responseJSON.statusCode == 500) {
                        toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                        return;
                    }
                    toastError(xhr.responseJSON.Result);

                }
            );
    });

    // Xử lý sự kiện click lưu thay đổi trong modal sửa
    $(document).on('click', '#UpdateProductModal .save-button', function () {
        let form = $('#UpdateProductModal').find('form');
        if (!validateForm(form)) return
        let data = form.convertFormToFormData();
        let url = "/Product/Update"
        let type = "PUT"
        callAPI(url, type, JSON.stringify(data), (res) => {
            if (res.statusCode === 200) {
                toastSuccess(res.message)
                $('#UpdateProductModal').modal('hide');
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

    // Thêm sự kiện blur để validate realtime
    $('#Code, #Name, #Price').on('blur', function () {
        validateField($(this), {
            required: true,
            minLength: $(this).attr('id') === 'Code' ? 3 : undefined,
            maxLength: $(this).attr('id') === 'Code' ? 50 : undefined
        });
    });
    // Bắt sự kiện modal được show
    $(document).on('show.bs.modal', '#CreateProductModal, #UpdateProductModal', function () {
        // Khởi tạo input tiền tệ chỉ trong modal hiện tại
        MoneyUtils.initMoneyInput($(this).find('.money-input'));
    });

    // Bắt sự kiện modal được show
    $(document).on('hide.bs.modal', '#CreateProductModal, #UpdateProductModal', function () {
       $(this).find('form').clearForm(); // Xóa dữ liệu trong form
    });
});


