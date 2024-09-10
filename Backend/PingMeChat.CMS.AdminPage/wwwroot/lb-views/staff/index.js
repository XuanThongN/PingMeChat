$(function () {
    var columns = [
        { targets: 0, data: "id", visible: false },
        {
            targets: 1,
            data: null,
            searchable: false,
            orderable: false,
            className: 'dt-body-center',
            render: function (data, type, row, meta) {
                return `<input type="checkbox" name="checkBoxRow"  value="${row.id}">`;
            }
        },
        { targets: 2, data: "fullName", sortable: false },
        { targets: 3, data: "code", sortable: false, },
        { targets: 4, data: "sexName", sortable: false, },
        {
            targets: 5, data: "email", sortable: false, render: function (data, type, row, meta) {
                if (data == null) {
                    return '<i>Đang cập nhật</i>'
                }
                return data
            }
        },
        {
            targets: 6, data: "phoneNumber", sortable: false, render: function (data, type, row, meta) {
                if (data == null) {
                    return '<i>Đang cập nhật</i>'
                }
                return data
            }
        },
        {
            targets: 7, data: "statusName", sortable: false, render: function (data, type, row, meta) {
                if (data != null) {
                    if (row.status == 1) {
                        return `<span class="d-flex justify-content-center">
                                      <span class="badge badge-success text-center text-light" >
                                       ${data}
                                    </span>
                                </span> `;

                    } else if (row.status == 2) {
                        return `<span class="d-flex justify-content-center">
                                      <span class="badge badge-danger text-center text-light" >
                                       ${data}
                                    </span>
                                </span> `;
                    }
                } else {
                    return `<span class="d-flex justify-content-center">
                         <span class="badge badge-warning text-center text-light" >
                            Chưa xác định
                        </span>
                    </span> `;
                }
            }
        },
        {
            targets: 8, data: null, sortable: false, render: function (data, type, row, meta) {
                if (Array.isArray(row.pdsJobs) && row.pdsJobs.length > 0) {
                    // Sử dụng map để xử lý từng đối tượng trong danh sách pdsJobs
                    const processedData = row.pdsJobs.map((job) => {
                        if (job && job.department && job.department.name != null) {
                            return job.department.name;
                        } else {
                            return '<i>Đang cập nhật</i>';
                        }
                    });

                    // Trả về chuỗi các kết quả đã xử lý
                    return processedData.join(', '); // Có thể thay đổi cách bạn muốn hiển thị kết quả
                } else {
                    return '<i>Đang cập nhật</i>';
                }
            }
        },
        {
            targets: 9,
            data: null,
            sortable: false,
            autoWidth: true,
            defaultContent: '',
            render: (data, type, row, meta) => {
                return `
                        <div class="d-flex justify-content-center">
                             <a class="btn btn-outline-danger btn-sm btn-view-cancel-modal" data-view-cancel-id="${row.id}"  data-toggle="modal" data-target="#modalViewCancel"><i class="fas fa-eye mx-1"></i>Thông tin</a>
                        </div>
                        <div class="d-flex justify-content-center">
                             <a class="btn btn-outline-danger btn-sm btn-edit-modal" data-edit-id="${row.id}" data-toggle="modal" data-target="#modalEdit"><i class="fas fa-user-edit mx-1"></i>Cập nhật</a>
                        </div>
                         `
            }
        }
    ];

    var url = '/Staff/Pagination'
    var dataSrc = "data"
    var reqData = null;
    var tableId = 'staffTableId';
    updateDataTable(url, columns, tableId, reqData, dataSrc)


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
        var searchUrl = '/Staff/Search?' + queryString;
        updateDataTable(searchUrl, columns, tableId, reqData, dataSrc)
    });

    // reset form
    $(document).on('click', '.btn-reset', function () {
        $('#form-filter')[0].reset();
        updateDataTable(url, columns, tableId, reqData, dataSrc)
    })
});

// cập nhật thông tin của nhân viên
$(document).on('click', '.btn-edit-modal', function () {
    let id = $(this).attr('data-edit-id')
    let url = `/Staff/GetParialEdit/${id}`

    getPartialViewModal(url, successCallBackWhenViewModal, errorCallBackWhenViewModal)
})
successCallBackWhenViewModal = function successCallBack(content) {
    $('#modalEdit div.modal-dialog div.modal-content').html(content);
    $('#modalEdit').modal('show');
}
errorCallBackWhenViewModal = function errorCallBack() {
    toastError("Lỗi hệ thống, vui lòng thử lại sau")
}