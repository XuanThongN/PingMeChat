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
        { targets: 1, data: "code", sortable: false },
        { targets: 2, data: "bidaTableTypeName", sortable: false, },
        { targets: 3, data: "createdDateString", sortable: false, },
        {
            targets: 4, data: "bidaTableStatusName", className: "text-center", sortable: false, render: function (data, type, row, meta) {
                if (data == null) {
                    return '-'
                }
                //Set màu cho trạng thái
                if (row.bidaTableStatus == 0) { // Bàn trống
                    return `<span class="badge badge-success">${data}</span>`;
                } else if (row.bidaTableStatus == 1) { // Đang chơi
                    return `<span class="badge badge-warning">${data}</span>`;
                }
                else // Bảo trì
                    return `<span class="badge badge-danger">${data}</span>`;
            }
        },
        {
            targets: 5,
            data: null,
            sortable: false,
            autoWidth: true,
            defaultContent: '',
            render: (data, type, row, meta) => {
                return `<div class="d-flex justify-content-center">
                        <a class="btn btn-outline-info btn-sm btn-view-detail-modal me-2" data-view-detail-id="${row.id}"  data-bs-toggle="modal" data-bs-target="#ViewDetailBidaTableModal"><i class="fas fa-eye mx-1"></i></a>
                        <a class="btn btn-outline-danger btn-sm btn-delete-modal" data-delete-id="${row.id}" data-bs-toggle="modal" data-bs-target="#DeleteBidaTableModal"><i class="fas fa-window-close mx-1"></i></a>
                    </div>`;
            }
        }
    ];
    const table = DataTableUtils.init('#BidaTable-table', '/BidaTable/Pagination', columns, '#form-filter');
    $(document).on('click', '.btn-reset', function () {
        DataTableUtils.resetFilters('#form-filter', table);
    });


    // Xử lý sự kiện click nút sửa
    $(document).on('click', '.btn-edit-modal', function () {
        var id = $(this).data('edit-id');
        // Thêm logic để mở modal sửa và load dữ liệu
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
        let url = "/BidaTable/Detail"
        let type = "GET"
        let data = { id: id }
        callAPI(url, type, data, (res) => {
            $("#ViewDetailBidaTableModal .modal-dialog").html(res)

        }, function (xhr, status, error) {
            if (xhr.responseJSON.statusCode == 500) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                return;
            }
            toastError(xhr.responseJSON.Result);
        })
    });


    // Tạo hàm để validate form
    function validateForm() {
        let isValid = true;

        // Validate mã bàn
        isValid = validateField($('#Code'), {
            required: true,
            minLength: 2,
            maxLength: 50
        }) && isValid;

        // Validate loại bàn
        isValid = validateField($('#BidaTableTypeId'), {
            required: true
        }) && isValid;

        return isValid;
    }

    function resetFormAfterSubmit() {
        $('#CreateBidaTableForm')[0].reset();
        $('.error-message').remove();
    }

    // Xử lý khi modal tạo mới được mở
    $('#createModal').on('show.bs.modal', function (e) {
        var $newSelect = $('#createModal .select2');
        $newSelect.select2({
            dropdownParent: $('#createModal'),
            width: '100%',
        });
        renderBidaTableTypeList($newSelect);
    });


    // Xử lý sự kiện submit form tạo mới
    $(document).on('submit', '#CreateBidaTableForm', function (e) {
        e.preventDefault();
        if (validateForm()) {
            let url = "/BidaTable/Create"
            let type = "POST"
            let data = $(this).serializeArray().reduce(function (obj, item) {
                if (item.name === "Price") {
                    obj[item.name] = parseInt(item.value);
                } else {
                    obj[item.name] = item.value;
                }
                return obj;
            }, {});

            // hiện loading 
            LoadingOverlay.show()
            callAPI(url, type, JSON.stringify(data), (res) => {
                LoadingOverlay.hide(); // Ẩn loading

                if (res.statusCode != 201) {
                    toastError(res.message);
                    return;
                }
                //ẩn modal và ẩn luôn modal-backdrop
                $("#createModal .btn-close").click()
                table.ajax.reload()
                $('#CreateBidaTableForm').clearForm();
                toastSuccess(res.message);


            }, (xhr, status, error) => {
                console.log("xhr", xhr)
                LoadingOverlay.hide(); // Ẩn loading
                toastError(xhr.responseJSON.Result);

            })
        }
    });

    // Hàm để render danh sách loại bàn
    function renderBidaTableTypeList($select) {
        var url = "/BidaTableType/GetAllBidaTableType";
        GetData(url,
            function (response) {
                if (response) {
                    var types = response;
                    var optionsHtml = '<option value="" selected disabled>Chọn loại bàn</option>';
                    types.forEach(type => {
                        optionsHtml += `<option value="${type.value}">${type.text}</option>`;
                    });
                    $select.html(optionsHtml);
                    $select.trigger('change');
                } else {
                    toastError(response.message);
                }
            },
            function (xhr, status, error) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }

    // Xử lý sự kiện click nút xóa
    $(document).on('click', '.btn-delete-modal', function () {
        var id = $(this).attr('data-delete-id');
        var modal = $('#DeleteBidaTableModal');
        modal.find('.delete-button').attr('data-delete-id', id);
    });

    //Xác nhận xoá
    $(document).on('click', '#confirmDeleteButton', function () {
        let id = $(this).attr('data-delete-id');
        let url = `/BidaTable/Delete?id=${id}`
        let type = "DELETE"
        let data = { id: id }
        callAPI(url, type, data, (res) => {
            if (res.statusCode === 200) {
                toastSuccess(res.message);
                table.ajax.reload()
                $('#confirmDeleteButton').attr('data-delete-id', null)
                $('#DeleteBidaTableModal').modal('hide')
                return
            }
            toastError(res.message);
        }, (xhr, status, error) => {
            LoadingOverlay.hide();
            if (xhr.responseJSON.statusCode == 500) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                return;
            }
            toastError(xhr.responseJSON.Result);
        })

    });

    // Xử lý sự kiện khi modal xóa được ẩn
    $('#DeleteBidaTableModal').on('hidden.bs.modal', function () {
        $(this).find('#confirmDeleteButton').removeAttr('data-delete-id')
    });
});