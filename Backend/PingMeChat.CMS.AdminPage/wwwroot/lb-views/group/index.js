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
        { targets: 2, data: "groupName", sortable: false },
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

    var url = '/Group/Pagination';

    const table = DataTableUtils.init('#tableId', url, columns, '#form-filter');
    $(document).on('click', '.btn-reset', function () {
        DataTableUtils.resetFilters('#form-filter', table);
    });


});

$(document).ready(function () {

    // khi modal create hiện thị
    $('#createModal').on('show.bs.modal', function (e) {
        getRoles();
        getUsers();
    });
    
    // hàm gọi api để lấy danh sách nhóm quyền
    function getRoles() {
        let url = "/role/getallactive";
        LoadingOverlay.show();
        GetData(url,
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode !== 200) {
                    toastError(response.message);
                    return;
                }

                let roles = response.result;
                populateSelectRole('roleId', roles);
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }

    // gọi api để lấy danh sách người dùng theo username
    function getUsers() {
        let url = "/user/getallactive";
        LoadingOverlay.show();
        GetData(url,
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode !== 200) {
                    toastError(response.message);
                    return;
                }
                let users = response.result;
                populateSelectUser('userId', users);
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }
    
    // Hàm để thêm dữ liệu vào select
    function populateSelectUser(selectId, data) {
        const select = $(`#${selectId}`);
        data.forEach(item => {
            select.append(new Option(item.fullName, item.id, false, false));
        });
    }
    function populateSelectRole(selectId, data) {
        const select = $(`#${selectId}`);
        data.forEach(item => {
            select.append(new Option(item.name, item.id, false, false));
        });
    }

    // khi submit tạo nhóm người dùng
    $('#createForm').on('submit', function (e) {
        e.preventDefault();
        var url = '/group/Create'
        let type = "POST"

        // Lấy danh sách trường trong form và chuyển đổi thành JSON
        var data = $(this).serializeArray().reduce(function (obj, item) {
            if (item.name === "status") {
                obj[item.name] = item.value == 1 ? true : false;
            } else if (item.name === "roleIds" || item.name === "userIds") {
                // Đảm bảo rằng roleIds và userIds luôn là mảng
                obj[item.name] = obj[item.name] || [];
                obj[item.name].push(item.value);
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
    
    // Khởi tạo Select2
    $('.select2').select2({
        placeholder: "Chọn...",
        allowClear: true
    });
});