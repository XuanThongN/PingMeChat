Dropzone.options.dropzoneForm = {
    paramName: "file", // Tên tham số gửi tệp lên
    maxFilesize: 20, // Kích thước tệp tối đa (đơn vị MB)
    acceptedFiles: ".xlsx,.xls", // Chỉ chấp nhận tệp Excel
    init: function () {
        this.on("addedfile", function (file) {
            file.previewElement.parentNode.removeChild(file.previewElement); // Vô hiệu hóa chức năng hiển thị thông tin tệp
            toastInfo("Quá trình xử lý đang thực thi, vui lòng chờ")
        });

        this.on("success", function (file, response) {
            if (response.statusCode == 200) {
                toastSuccess("Đã thêm thành công danh sách sinh viên vào hệ thống")
                updateDataTable(url, columns, tableId, reqData, dataSrc);
            } else {
                toastError("Lỗi trong quá trình thực thi, vui lòng kiểm tra thông báo để xem chi tiết lỗi")
            }
        });

        this.on("error", function (file, errorMessage) {
            toastError("Lỗi trong quá trình thực thi, vui lòng kiểm tra thông báo để xem chi tiết lỗi")
        });
    },
};

$(function () {
    var columns = [
        { targets: 0, data: "id", visible: false },
        { targets: 1, data: "fullName", sortable: false },
        { targets: 2, data: "code", sortable: false, },
        { targets: 3, data: "birthdayString", sortable: false, },
        { targets: 4, data: "email", sortable: false, },
        {
            targets: 5, data: "phoneNumber", sortable: false, render: function (data, type, row, meta) {
                if (data == null) {
                    return '<i>Đang cập nhật</i>'
                }
                return data
            }
        },
        {
            targets: 6, data: "className", sortable: false, render: function (data, type, row, meta) {
                if (data == null) {
                    return '<i>Đang cập nhật</i>'
                }
                return data
            }
        }, {
            targets: 7, data: "majorName", sortable: false, render: function (data, type, row, meta) {
                if (data == null) {
                    return '<i>Đang cập nhật</i>'
                }
                return data
            }
        }, {
            targets: 8, data: "facultyName", sortable: false, render: function (data, type, row, meta) {
                if (data == null) {
                    return '<i>Đang cập nhật</i>'
                }
                return data
            }
        },
        {
            targets: 9, data: "academicTermResponseDto.name", sortable: false, render: function (data, type, row, meta) {
                if (data == null) {
                    return '<i>Đang cập nhật</i>'
                }
                return data
            }
        },
        {
            targets: 10,
            data: null,
            sortable: false,
            autoWidth: true,
            width: "160px",
            defaultContent: '',
            render: (data, type, row, meta) => {
                if (row.statusProcessing == 1) {
                    return `<div class="d-flex justify-content-center">
                                <a class="btn btn-outline-info btn-sm btn-view-modal mx-1" href="/RegistrationForm/ShowFilePDF/${row.id}/${row.formTypeId}" target="_blank" data-view-id="${row.id}"  data-toggle="modal" ><i class="fas fa-print mx-1"></i>In</a>
                                <a class="btn btn-outline-success btn-sm btn-confirm-save-modal me-1" data-confirm-save-id="${row.id}" data-confirm-save-formType-id="${row.formTypeId}" data-confirm-status="${row.statusProcessing}" data-code-student="${row.studentCode}" data-toggle="modal" data-target="#confirmSaveModal"><i class="fas fa-user-edit mx-1"></i>Xác nhận</a>
                                <a class="btn btn-outline-danger btn-sm btn-confirm-cancel-modal" data-confirm-cancel-id="${row.id}" data-confirm-cancel-formType-id="${row.formTypeId}" data-confirm-status="${row.statusProcessing}" data-code-student="${row.studentCode}" data-toggle="modal" data-target="#confirmCancelModal"><i class="fas fa-window-close mx-1"></i>Hủy</a>
                            </div>
                            `
                } else if (row.statusProcessing == 2) {
                    return `
                        <div class="d-flex justify-content-center">
                             <a class="btn btn-outline-info btn-sm btn-view-modal" href="/RegistrationForm/ShowFilePDF/${row.id}/${row.formTypeId}" target="_blank" data-view-id="${row.id}"  data-toggle="modal" ><i class="fas fa-print mx-1"></i>In</a>
                        </div>
                         `
                } else {
                    return `
                        <div class="d-flex justify-content-center">
                             <a class="btn btn-outline-danger btn-sm btn-view-cancel-modal" data-view-cancel-id="${row.id}"  data-toggle="modal" data-target="#modalViewCancel"><i class="fas fa-eye mx-1"></i>Thông tin</a>
                        </div>
                         `
                }

            }
        }
    ];
    var url = '/Student/Pagination'
    var dataSrc = "data"
    var reqData = null;
    var tableId = 'studentTableId';
    updateDataTable(url, columns, tableId, reqData, dataSrc);

    // Attach the form submit event listener outside of initTable
    $('#form-filter').on('submit', function (e) {
        e.preventDefault();
        // lấy giá trị trong form
        var form = $(this).serializeArray();
        var data = {};
        $.each(form, function (i, v) {
            data["" + v.name + ""] = v.value;
        });
        // Chuyển đổi dữ liệu thành query parameter
        var queryString = $.param(data);

        // Chuyển hướng trang với query parameter và cập nhật datatable
        var searchUrl = '/RegistrationForm/Pagination?' + queryString;
        updateDataTable(searchUrl, columns, tableId, reqData, dataSrc)

    });

    // reset form
    $(document).on('click', '.btn-reset', function () {
        $('#form-filter')[0].reset();
        updateDataTable(url, columns, tableId, reqData, dataSrc)
    })
});

