$(function () {
    var columns = [
        { targets: 0, data: "id", visible: false },
        { targets: 1, data: "studentName", sortable: false },
        { targets: 2, data: "studentCode", sortable: false, },
        {
            targets: 3, data: "createdDate",
            sortable: false,
            render: function (data, type, row, meta) {
                return fomartDateTimeServer(data)
            }
        },
        {
            targets: 4, data: "statusProcessingName", sortable: false, render: function (data, type, row, meta) {
                if (data != null) {
                    if (row.statusProcessing == 1) {
                        return `<span class="d-flex justify-content-center">
                                      <span class="badge badge-primary text-center text-light" >
                                       ${data}
                                    </span>
                                </span> `;

                    } else if (row.statusProcessing == 2) {
                        return `<span class="d-flex justify-content-center">
                                      <span class="badge badge-success text-center text-light" >
                                       ${data}
                                    </span>
                                </span> `;
                    } else if (row.statusProcessing == 3) {
                        return `<span class="d-flex justify-content-center">
                                      <span class="badge badge-danger text-center text-light" >
                                       ${data}
                                    </span>
                                </span> `;
                    } else {
                        return `<span class="d-flex justify-content-center">
                                      <span class="badge badge-danger text-center text-light" >
                                        Chưa xác định
                                    </span>
                                </span> `;
                    }
                } else {
                    return `<span class="d-flex justify-content-center">
                         <span class="badge badge-danger text-center text-light" >
                            Chưa xác định
                        </span>
                    </span> `;
                }
            }
        },
        {
            targets: 5, data: "formName",
            sortable: false,
            autoWidth: true,
            width: "260px",

        },
        {
            targets: 6,
            data: null,
            sortable: false,
            autoWidth: true,
            width: "260px",
            defaultContent: '',
            render: (data, type, row, meta) => {
                if (row.statusProcessing == 1) {
                    return `<div class="d-flex justify-content-center">
                                <a class="btn btn-info btn-sm btn-view-modal mx-1" href="/RegistrationForm/ShowFilePDF/${row.id}/${row.formTypeId}" target="_blank" data-view-id="${row.id}"  data-toggle="modal" ><i class="fas fa-print mx-1"></i></a>
                                <a class="btn btn-primary btn-sm btn-feedback-modal  me-1" data-feedback-id="${row.id}" data-bs-toggle="modal" data-target="#modalFeedBack"><i class="far fa-comments mx-1"></i></a>
                                <a class="btn btn-success btn-sm btn-confirm-save-modal me-1" data-confirm-save-id="${row.id}" data-confirm-save-formType-id="${row.formTypeId}" data-confirm-status="${row.statusProcessing}" data-code-student="${row.studentCode}" data-toggle="modal" data-target="#confirmSaveModal"><i class="fas fa-user-edit mx-1"></i></a>
                                <a class="btn btn-danger btn-sm btn-confirm-cancel-modal" data-confirm-cancel-id="${row.id}" data-confirm-cancel-formType-id="${row.formTypeId}" data-confirm-status="${row.statusProcessing}" data-code-student="${row.studentCode}" data-toggle="modal" data-target="#confirmCancelModal"><i class="fas fa-window-close mx-1"></i></a>
                            </div>
                            `
                } else if (row.statusProcessing == 2) {
                    return `
                        <div class="d-flex justify-content-center">
                             <a class="btn btn-info btn-sm btn-view-modal px-1 me-1" href="/RegistrationForm/ShowFilePDF/${row.id}/${row.formTypeId}" target="_blank" data-view-id="${row.id}"  data-toggle="modal" ><i class="fas fa-print mx-2"></i></a>
                        </div>
                         `
                } else {
                    return `
                        <div class="d-flex justify-content-center">
                                <a class="btn btn-primary btn-sm btn-feedback-modal  me-1" data-feedback-id="${row.id}" data-bs-toggle="modal" data-target="#modalFeedBack"><i class="far fa-comments mx-1"></i></a>
                             <a class="btn btn-danger btn-sm btn-view-cancel-modal px-1" data-view-cancel-id="${row.id}"  data-toggle="modal" data-target="#modalViewCancel"><i class="fas fa-eye me-1"></i></a>
                        </div>
                         `
                }

            }
        }
    ];

    var url = '/RegistrationForm/GetRegistrationFormPagination'
    var dataSrc = "data"
    var reqData = null;
    var tableId = 'registrationFormTableId';
    updateDataTable(url, columns, tableId, reqData, dataSrc)


    // filter
    $('#form-filter').on('submit', function (e) {
        e.preventDefault();
        // validate
        if (validateTime('val_fromDate', 'val_toDate', 'val_form'))
            return;
        // lấy giá trị trong form
        var form = $(this).serializeArray();
        var data = {};
        $.each(form, function (i, v) {
            data["" + v.name + ""] = v.value;
        });
        // Chuyển đổi dữ liệu thành query parameter
        var queryString = $.param(data);

        // Chuyển hướng trang với query parameter và cập nhật datatable
        var searchUrl = '/RegistrationForm/Search?' + queryString;
        updateDataTable(searchUrl, columns, tableId, reqData, dataSrc)
    })

    // reset form
    $(document).on('click', '.btn-reset', function () {
        $('#form-filter')[0].reset();
        updateDataTable(url, columns, tableId, reqData, dataSrc)
    })

});


// xác nhận - đã xử lý đơn thành công
$(document).on('click', '.btn-confirm-save-modal', function () {
    let id = $(this).attr("data-confirm-save-id")
    let formTypeId = $(this).attr("data-confirm-save-formType-id")
    // let status = parseInt($(this).attr("data-confirm-status"));
    let status = 2;
    let code = $(this).attr("data-code-student")

    var data = JSON.stringify({
        FormTypeId: formTypeId,
        Status: status,
        StudentCode: code
    });

    function confirmSaveFunction() {
        let url = `/RegistrationForm/ComfirmSave/${id}`

        callAPI(url, "POST", data, successCallbackConfirm, errorCallbackConfirm)
    }

    toastConfirmCancelAndSave("Vui lòng xác minh bạn đã xử lý đơn của sinh viên thành công", confirmSaveFunction)
})

// modal - hiện thị modal feedback
$(document).on('click', '.btn-feedback-modal', function () {
    let id = $(this).attr("data-feedback-id")
    let url = `/RegistrationForm/ViewFeedBackModal/${id}`

    callAPI(url, "GET", null, successCallbackFeedBack, errorCallbackFeedBack)
})

// xác nhận - modal -  hủy yêu cầu đăng ký của sinh viên
$(document).on('click', '.btn-confirm-cancel-modal', function () {
    let id = $(this).attr("data-confirm-cancel-id")
    let url = `/RegistrationForm/ConfirmCancel/${id}`

    callAPI(url, "GET", null, successCallbackConfirmCancel, errorCallbackConfirmCancel)
})
// modal - hiện thị thông tin , lý do đã hủy đơn
$(document).on('click', '.btn-view-cancel-modal', function () {
    let id = $(this).attr("data-view-cancel-id")
    let url = `/RegistrationForm/ViewCancelModal/${id}`

    callAPI(url, "GET", null, successCallbackViewCancel, errorCallbackViewCancel)
})

successCallbackConfirm = function successCallback(data) {
    if (data.statusCode == 200) {
        toastSuccess("Xác nhận thành công");
        // reload datatable
        reloadDataTable('registrationFormTableId');
    } else {
        toastError(data.message)
    }

}

successCallbackConfirmCancel = (content) => {
    $('#modalCancel div.modal-dialog div.modal-content').html(content);
    $('#modalCancel').modal('show');
}
successCallbackFeedBack = (content) => {
    $('#modalFeedBack div.modal-dialog div.modal-content').html(content);
    $('#modalFeedBack').modal('show');
}
errorCallbackConfirmCancel = () => {
    toastError("Lỗi hệ thống, vui lòng thử lại sau")

}
successCallbackViewCancel = (content) => {
    $('#modalViewCancel div.modal-dialog div.modal-content').html(content);
    $('#modalViewCancel').modal('show');
}

errorCallbackConfirm = function errorCallback() {
    toastError("Lỗi hệ thống, vui lòng thử lại sau")
}

errorCallbackViewCancel = () => {
    toastError("Lỗi hệ thống, vui lòng thử lại sau")
}
errorCallbackFeedBack = () => {
    toastError("Lỗi hệ thống, vui lòng thử lại sau")

}