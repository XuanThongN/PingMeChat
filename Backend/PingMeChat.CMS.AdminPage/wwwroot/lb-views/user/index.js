var currentId;
var userNameAccount;

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
        { targets: 2, data: "userName", sortable: false },
        { targets: 3, data: "email", sortable: false, },
        {
            targets: 4, data: "dateRegistered",
            sortable: false,
            render: function (data, type, row, meta) {
                return fomartDateTimeServer(data)
            }
        },

        {
            targets: 5, data: "isLocked", sortable: false, render: function (data, type, row, meta) {
                if (data == false) {
                    return `<div class="d-flex justify-content-center">
                         <button class="badge badge-success text-center text-light lockAccount" style="border: none;" data-bs-toggle="modal" data-bs-target="#lockAccountModal" data-id="${row.id}" data-username="${row.userName}">
                        <i class="far fa-check-circle mx-1"></i>  Hoạt động
                        </button>
                    </div> `;

                } else {
                    return `<div class="d-flex justify-content-center">
                         <button class="badge badge-danger text-center text-light unlockAccount" style="border: none;"  data-bs-toggle="modal" data-bs-target="#unlockAccountModal" data-id="${row.id}" data-username="${row.userName}">
                          <i class="fas fa-power-off mx-1"></i>    Đã khóa
                        </button>
                    </div> `;
                }
            }
        },
        {
            targets: 6, data: "fullName",
            sortable: false,
            render: function (data, type, row, meta) {
                if (data == null) {
                    return `<i class="text-warning">Đang cập nhật</i>`
                } else {
                    return data
                }
            }
        },

        {
            targets: 7,
            data: null,
            sortable: false,
            defaultContent: '',
            render: (data, type, row, meta) => {
                return `
                    <div class="text-center">
                       <a  title="Xem chi tiết" class="btn btn-outline-info btn-sm btn-view-modal" data-view-id="${row.id}" data-toggle="modal" data-target="#viewModal"><i class="fas fa-eye mx-1"></i></a>
                       <a title="Đổi mật khẩu" class="btn btn-outline-primary btn-sm btn-view-modal" data-view-id="${row.id}" ><i class="fas fa-exchange-alt mx-1"></i></a>
                       <a  title="Cập nhật thông tin" class="btn btn-outline-warning btn-sm btn-edit-modal" data-edit-id="${row.id}" ><i class="fas fa-user-edit mx-1"></i></a>
                       <a  title="Xóa" class="btn btn-outline-danger btn-sm btn-remove-modal" ><i class="fas fa-minus-circle mx-1"></i></a>
                    </div>
               `

            }
        }
    ];
    var url = '/User/Pagination'

    const table = DataTableUtils.init('#tableId', url, columns, '#form-filter');
    $(document).on('click', '.btn-reset', function () {
        DataTableUtils.resetFilters('#form-filter', table);
    });

    // reset form
    $(document).on('click', '.btn-reset', function () {
        $('#form-filter')[0].reset();
        updateDataTable(url, columns, tableId, reqData, dataSrc)
    })


});
$(document).ready(function () {
    // khi modal create hiện thị
    $('#createModal').on('show.bs.modal', function (e) {
        let groupIdTag = 'groupId';
        getGroups(groupIdTag);
    });
    // khi modal create ẩn đi - làm sách form
    $('#createModal').on('hidden.bs.modal', function () {
        $('#createForm')[0].reset();
    });
    //// khi modal create hiện thị
    //$('#editModal').on('show.bs.modal', function (e) {
    //    let groupIdTag = 'updateGroupId';
    //    getGroups(groupIdTag);
    //});

    // hàm gọi api để lấy danh sách nhóm quyền
    function getGroups(groupIdTag) {
        let url = "/group/getallactive";
        LoadingOverlay.show();
        GetData(url,
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode !== 200) {
                    toastError(response.message);
                    return;
                }

                let groups = response.result;
                populateSelect(groupIdTag, groups);
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }
    // phục vụ hiện thị dữ liệu cho edit
    function getGroups(groupIdTag, selectedGroups) {
        let url = "/group/getallactive";
        LoadingOverlay.show();
        GetData(url,
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode !== 200) {
                    toastError(response.message);
                    return;
                }
                let groups = response.result;
                populateSelect(groupIdTag, groups, selectedGroups);
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }
    // // Hàm để thêm dữ liệu vào select
    function populateSelect(selectId, data, selectedValues) {
        const select = $(`#${selectId}`);
        select.empty();
        data.forEach(item => {
            const option = new Option(item.groupName, item.id, false, false);
            select.append(option);
        });
        if (selectedValues && selectedValues.length > 0) {
            select.val(selectedValues).trigger('change');
        }
    }
    // Hàm để thêm dữ liệu vào select
    function populateSelect(selectId, data) {
        // làm sạch dữ liệu trước đó
        $(`#${selectId}`).empty();
        // thêm dữ liệu mới
        const select = $(`#${selectId}`);
        data.forEach(item => {
            select.append(new Option(item.groupName, item.id, false, false));
        });
    }
    // tạo mới tài khoản
    $('#createForm').on('submit', function (e) {
        e.preventDefault();

        if (validateForm()) {
            var url = '/User/Create'

            // Lấy danh sách trường trong form và chuyển đổi thành JSON
            var data = $(this).serializeArray().reduce(function (obj, item) {
                if (item.name === "groupIds") {
                    obj[item.name] = obj[item.name] || [];
                    obj[item.name].push(item.value);
                } else {
                    obj[item.name] = item.value;
                }
                return obj;
            }, {});

            LoadingOverlay.show();
            // ajax -- index.js
            callAPI(url, "POST", JSON.stringify(data), function (data) {
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
                if (xhr.responseJSON.statusCode == 500) {
                    toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                    return;
                }
                toastError(xhr.responseJSON.Result);
            });
        }
    });

    // cập nhật lại thông tin
    $('#updateForm').on('submit', function (e) {
        e.preventDefault();

        if (validateUpdateForm()) {
            var url = '/User/Update'
            var formData = $(this).serializeArray();
            var data = {};

            formData.forEach(function (item) {
                if (item.name === "groupIds") {
                    if (!data[item.name]) {
                        data[item.name] = [];
                    }
                    data[item.name].push(item.value);
                } else if (item.name === "isLocked") {
                    data[item.name] = item.value === "true";
                } else {
                    data[item.name] = item.value;
                }
            });

            // thêm id
            data.id = currentId
            // Đảm bảo groupIds luôn là một mảng
            if (!data.groupIds) {
                data.groupIds = [];
            }

            // Lấy giá trị từ Select2 nếu có
            var selectedGroups = $('#updateGroupId').select2('data');
            if (selectedGroups && selectedGroups.length > 0) {
                data.groupIds = selectedGroups.map(function (item) {
                    return item.id;
                });
            }

            LoadingOverlay.show();
            // ajax -- index.js
            callAPI(url, "PUT", JSON.stringify(data), function (data) {
                LoadingOverlay.hide();
                if (data.statusCode == 200) {
                    $("#editModal").modal('hide');
                    toastSuccess(data.message);
                    reloadDataTable('tableId')
                } else {
                    toastError(data.message)
                }
            }, function (xhr, status, error) {
                LoadingOverlay.hide();
                if (xhr.responseJSON.statusCode == 500) {
                    toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                    return;
                }
                toastError(xhr.responseJSON.Result);
            });
        }
    });


    // Khởi tạo Select2
    $('.select2').select2({
        placeholder: "Chọn...",
        allowClear: true
    });

    // Khi mở modal thay đổi trạng thái - khóa - mở khóa => set giá trị id người dùng hiện tại
    $(document).on('click', '.lockAccount, .unlockAccount ', function () {
        currentId = $(this).data('id');
        userNameAccount = $(this).data('username');

        console.log("userNameAccount", userNameAccount);
        $('.userNameAccount').text(userNameAccount);
    });

    // Khi xác khóa tài khoản
    $('#lockAccountModalBtn').on('click', function () {
        var url = '/User/LockAccount?id=' + currentId;

        LoadingOverlay.show();
        callAPI(url, "Get", null, function (data) {
            LoadingOverlay.hide();
            if (data.statusCode == 200) {
                $("#lockAccountModal").modal('hide');
                toastSuccess(data.message);
                reloadDataTable('tableId')
            } else {
                toastError(data.message)
            }
        }, function (xhr, status, error) {
            LoadingOverlay.hide();
            if (xhr.responseJSON.statusCode == 500) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                return;
            }
            toastError(xhr.responseJSON.Result);
        });
    });

    // Khi xác khóa tài khoản
    $('#unlockAccountModalBtn').on('click', function () {
        var url = '/User/UnLockAccount?id=' + currentId;

        LoadingOverlay.show();
        callAPI(url, "Get", null, function (data) {
            LoadingOverlay.hide();
            if (data.statusCode == 200) {
                $("#unlockAccountModal").modal('hide');
                toastSuccess(data.message);
                reloadDataTable('tableId')
            } else {
                toastError(data.message)
            }
        }, function (xhr, status, error) {
            LoadingOverlay.hide();
            if (xhr.responseJSON.statusCode == 500) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                return;
            }
            toastError(xhr.responseJSON.Result);
        });
    });

    // Xử lý sự kiện click nút sửa
    $(document).on('click', '.btn-edit-modal', function () {
        var id = $(this).data('edit-id');
        currentId = id;
        let url = "/user/GetById?id=" + id;
        let type = "GET"

        LoadingOverlay.show();
        callAPI(url, type, null, function (response) {
            LoadingOverlay.hide();
            if (response.statusCode === 200) {
                populateEditForm(response.result);
                $("#editModal").modal('show');
            } else {
                toastError(response.message);
            }
        }, function (xhr, status, error) {
            LoadingOverlay.hide();
            if (xhr.responseJSON.statusCode == 500) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                return;
            }
            toastError(xhr.responseJSON.Result);
        });

    });

    function populateEditForm(userData) {
        // Điền dữ liệu vào các trường form
        $('#updateForm input[name="id"]').val(userData.id);
        $('#updateForm input[name="userName"]').val(userData.userName);
        $('#updateForm input[name="fullName"]').val(userData.fullName);
        $('#updateForm input[name="email"]').val(userData.email);
        $('#updateForm input[name="phoneNumber"]').val(userData.phoneNumber);
        $('#updateForm input[name="dateRegistered"]').val(fomartDateTimeServer(userData.dateRegistered));

        // ... điền các trường khác

        // Xử lý select cho isLocked
        $('#updateForm select[name="isLocked"]').val(userData.isLocked.toString());

        // Xử lý select cho nhóm người dùng
        getGroups('updateGroupId', userData.groupIds);
    }
    // hàm gọi api để lấy danh sách nhóm quyền
    function getGroups(groupIdTag, userGroups) {
        let url = "/group/getallactive";
        LoadingOverlay.show();
        GetData(url,
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode !== 200) {
                    toastError(response.message);
                    return;
                }
                let groups = response.result;
                populateSelect(groupIdTag, groups, userGroups);
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }

    // Hàm để thêm dữ liệu vào select
    function populateSelect(selectId, data, selectedGroups) {
        // làm sạch dữ liệu trước đó
        $(`#${selectId}`).empty();

        // thêm dữ liệu mới
        const select = $(`#${selectId}`);
        data.forEach(item => {
            let isSelected = selectedGroups && selectedGroups.includes(item.id);
            select.append(new Option(item.groupName, item.id, isSelected, isSelected));
        });

        // Cập nhật Select2
        select.trigger('change');
    }
    // Tạo hàm để validate khi tạo mơi tài khoản
    function validateForm() {
        let isValid = true;

        isValid = validateField($("#fullName"), {
            required: true,
            minLength: 3,
            maxLength: 225
        }) && isValid;

        isValid = validateField($('#email'), {
            required: true,
            email: true,
        }) && isValid;

        isValid = validateField($('#phoneNumber'), {
            required: true,
            phone: true,
        }) && isValid;

        // Thêm kiểm tra cho username
        isValid = validateField($('#userName'), {
            required: true,
            minLength: 3,
            maxLength: 20,
            username: true
        }) && isValid;

        // Kiểm tra password
        isValid = validateField($('#passwordUser'), {
            required: true,
            minLength: 8,
            maxLength: 50,
            password: true
            // Thêm các quy tắc khác cho mật khẩu nếu cần
        }) && isValid;

        //  Kiểm tra confirmPassword
        isValid = validateField($('#confirmPassword'), {
            required: true,
            matchPassword: '#passwordUser'
        }) && isValid;

        return isValid;
    }

    // Tạo hàm để validate khi cập nhật
    function validateUpdateForm() {
        let isValid = true;

        isValid = validateField($('#updateForm input[name="fullName"]'), {
            required: true,
            minLength: 3,
            maxLength: 225
        }) && isValid;

        isValid = validateField($('#updateForm input[name="email"]'), {
            required: true,
            email: true,
        }) && isValid;

        isValid = validateField($('#updateForm input[name="phoneNumber"]'), {
            required: true,
            phone: true,
        }) && isValid;

        return isValid;
    }
})


//// reset form
//$(document).on('click', '.btn-reset', function () {
//    $('#form-filter')[0].reset();
//    reloadDataTable('userTableId');
//})
//// liên quan tới chức năng thêm nhóm quyền
//$(document).on('click', '.addRole>.selectAllRole', function () {
//    $('.roleNameAdd>input[type="checkbox"][name="roleName"]').prop('checked', true);
//})
//$(document).on('click', '.addRole>.uncheckedAllRole', function () {
//    $('.roleNameAdd>input[type="checkbox"][name="roleName"]').prop('checked', false);
//})
//// xem thông tin chi tiết về nhóm quyền
//$(document).on('click', '.btn-view-modal', function () {
//    let id = $(this).attr('data-view-id')
//    let url = `/User/GetParialView/${id}`

//    getPartialViewModal(url, successCallBackWhenViewModal, errorCallBackWhenViewModal)

//})
//successCallBackWhenViewModal = function successCallBack(content) {
//    $('#modalView div.modal-dialog div.modal-content').html(content);
//    $('#modalView').modal('show');
//}
//errorCallBackWhenViewModal = function errorCallBack() {
//    toastError("Lỗi hệ thống, vui lòng thử lại sau")
//}

//// cập nhật thông tin tài khoản

//$(document).on('click', '.btn-edit-modal', function () {
//    let id = $(this).attr('data-edit-id')
//    let url = `/User/GetParialViewEdit/${id}`

//    getPartialViewModal(url, successCallBackWhenEditModal, errorCallBackWhenEditModal)
//})

//successCallbackViewCreateModal = content => {
//    if (!content) {
//        toastError("Lỗi trong quá trình tạo mã, vui lòng thử lại sau")
//    } else {
//        localStorage.setItem("currentCode", content);
//        $("#code").val(content)
//    }
//}
//errorCallbackViewCreateModal = () => {
//    toastError("Lỗi trong quá trình tạo mã tài khoản, vui lòng thử lại sau")
//}

// Thêm đoạn code này ở ngoài event handler
//$('#createModal').on('hidden.bs.modal', function (e) {
//    $('.modal-backdrop').remove();
//    $('body').removeClass('modal-open');
//    $('body').css('padding-right', '');
//});