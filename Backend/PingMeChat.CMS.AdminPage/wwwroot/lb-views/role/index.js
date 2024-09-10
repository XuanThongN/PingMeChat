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
        { targets: 2, data: "name", sortable: false },
        {
            targets: 3,
            data: "status",
            sortable: false,
            className: 'text-center',
            render: function (data, type, row, meta) {
                if (data === false) {
                    return `<div class="d-flex justify-content-center">
                      <button class="badge badge-danger text-center text-light changeStatus" style="border: none;"  data-bs-toggle="modal" data-bs-target="#changeStatusModal" data-id="${row.id}" >
                     <i class="fas fa-ban mx-1"></i>
                       Đã khóa
                        </button>
                    </div> `;
                }
                else {
                    return `<div class="d-flex justify-content-center">
                         <button class="badge badge-success text-center text-light changeStatus" style="border: none;" data-bs-toggle="modal" data-bs-target="#changeStatusModal" data-id="${row.id}">
                       <i class="far fa-check-circle mx-1"></i>  Hoạt động
                        </button>
                    </div> `;
                }
            }
        },
        {
            targets: 4,
            data: "createdDate",
            sortable: false,
            className: 'text-center',
            render: function (data, type, row) {
                return (data)
                    ? moment(data).format("DD/MM/YYYY HH:mm")
                    : null;
            }
        },
        { targets: 5, data: "createdBy", sortable: false, },
        {
            targets: 6,
            data: "updatedDate",
            sortable: false,
            className: 'text-center',
            render: function (data, type, row) {
                return (data)
                    ? moment(data).format("DD/MM/YYYY HH:mm")
                    : null;
            }
        },
        { targets: 7, data: "updatedBy", sortable: false, },
        {
            targets: 8,
            data: null,
            sortable: false,
            autoWidth: true,
            className: 'text-center',

            defaultContent: '',
            render: (data, type, row, meta) => {
                return `
               <a  title="Xem chi tiết" class="btn btn-outline-info btn-sm btn-view-modal" data-view-id="${row.id}" data-toggle="modal" data-target="#viewModal"><i class="fas fa-eye mx-1"></i></a>
           `
            }
        }
    ];

    var url = '/Role/Pagination';

    const table = DataTableUtils.init('#tableId', url, columns, '#form-filter');
    $(document).on('click', '.btn-reset', function () {
        DataTableUtils.resetFilters('#form-filter', table);
    });

    
});
// khi bấm lưu nhóm quyền
$('#createForm').on('submit', function (e) {
    e.preventDefault();
    var url = '/Role/Create'
    let type = "POST"

    // Lấy danh sách trường trong form và chuyển đổi thành JSON
    var data = $(this).serializeArray().reduce(function (obj, item) {
        if (item.name === "status") {
            obj[item.name] = item.value == 1 ? true : false;
        }
        else {
            obj[item.name] = item.value;
        }
        return obj;
    }, {});

    LoadingOverlay.show();
    // ajax -- index.js
    callAPI(url, type, JSON.stringify(data), function (data) {
        LoadingOverlay.hide();
        if (data.statusCode == 201) {
            $("#createModal").modal('hide');
            toastSuccess(data.message);
            reloadDataTable('tableId')
        } else {
            toastError(data.message)
        }
    }, function (xhr, status, error) {
        LoadingOverlay.hide();
        toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
    });
});
//$('#createModal').on('hidden.bs.modal', function () {
//    $('#createRole')[0].reset();
//});

//$('#createRole').on('submit', function (e) {
//    e.preventDefault();

//    // Lấy danh sách trường trong form và chuyển đổi thành JSON
//    var formData = $(this).serializeArray().reduce(function (obj, item) {
//        obj[item.name] = item.value;
//        return obj;
//    }, {});

//    var jsonData = JSON.stringify(formData);
//    var url = '/Role/Create'
//    LoadingOverlay.show();

//    // ajax -- index.js
//    callAPI(url, "POST", jsonData, function (data) {
//        if (data.statusCode == 201) {
//            $("#createModal").modal('hide');
//            LoadingOverlay.hide();
//            toastSuccess(data.message);
//            reloadDataTable('tableId')
//        } else {
//            toastError(data.message)
//        }
//    }, function (xhr, status, error) {
//        LoadingOverlay.hide();
//        toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
//    });
//});

//// xem thông tin chi tiết về nhóm quyền
//$(document).on('click', '.btn-view-modal', function () {
//    let id = $(this).attr('data-view-id')
//    let url = '/Role/GetParialView/' + id

//    getPartialViewModal(url, successCallBackWhenViewModal, errorCallBackWhenViewModal)

//})

//successCallBackWhenViewModal = function successCallBackWhenViewModal(content) {
//    $('#modalView div.modal-dialog div.modal-content').html(content);
//    $('#modalView').modal('show');
//}

//successCallbackCreateData = data => {
//    $("#createModal").modal('hide');
//    let url = '/Role/GetRolePagination'
//    updateDataTable(url)
//    return;
//}
//errorCallBackWhenViewModal = function errorCallBackWhenViewModal() {
//    toastError("Lỗi hệ thống, vui lòng thử lại sau")
//}
//errorCallbackCreateData = () => {
//    //toastError("Lỗi trong quá trình xử lý, vui lòng thử lại sau")
//    return;
//}



// chỉnh sửa thông tin nhóm quyền