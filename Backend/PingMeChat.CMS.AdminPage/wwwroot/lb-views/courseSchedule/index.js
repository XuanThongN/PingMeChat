/*// DropzoneJS Demo Code Start
Dropzone.autoDiscover = false;

// Get the template HTML and remove it from the doumenthe template HTML and remove it from the doument
var previewNode = document.querySelector("#template");
previewNode.id = "";
var previewTemplate = previewNode.parentNode.innerHTML;
previewNode.parentNode.removeChild(previewNode);

var myDropzone = new Dropzone(document.body, {
    // Make the whole body a dropzone
    url: "/target-url", // Set the url
    thumbnailWidth: 80,
    thumbnailHeight: 80,
    parallelUploads: 20,
    acceptedFiles: ".xlsx,.xls",
   // previewTemplate: previewTemplate,
    autoQueue: false, // Make sure the files aren't queued until manually added
   // previewsContainer: "#previews", // Define the container to display the previews
  //  clickable: ".fileinput-button", // Define the element that should be used as click trigger to select files.
});

myDropzone.on("addedfile", function (file) {
    // Hookup the start button
    file.previewElement.querySelector(".start").onclick = function () {
        myDropzone.enqueueFile(file);
    };
});*/

// Update the total progress bar
// myDropzone.on("totaluploadprogress", function (progress) {
//   document.querySelector("#total-progress .progress-bar").style.width =
//     progress + "%";
// });

/*myDropzone.on("sending", function (file) {
    // Show the total progress bar when upload starts
    //document.querySelector("#total-progress").style.opacity = "1";
    // And disable the start button
    file.previewElement
        .querySelector(".start")
        .setAttribute("disabled", "disabled");
});*/

// Hide the total progress bar when nothing's uploading anymore
// myDropzone.on("queuecomplete", function (progress) {
//   document.querySelector("#total-progress").style.opacity = "0";
// });

// Setup the buttons for all transfers
// The "add files" button doesn't need to be setup because the config
// `clickable` has already been specified.
/*document.querySelector("#actions .start").onclick = function () {
    myDropzone.enqueueFiles(myDropzone.getFilesWithStatus(Dropzone.ADDED));
};
document.querySelector("#actions .cancel").onclick = function () {
    myDropzone.removeAllFiles(true);
};*/
// DropzoneJS Demo Code End

Dropzone.options.dropzoneForm = {
    paramName: "file", // Tên tham số gửi tệp lên
    maxFilesize: 20, // Kích thước tệp tối đa (đơn vị MB)
    acceptedFiles: ".xlsx,.xls", // Chỉ chấp nhận tệp Excel
    success: function (file, response) {
        console.log("response", response); // Xử lý phản hồi từ máy chủ
    },
    error: function (file, errorMessage) {
        console.log("error", errorMessage); // Xử lý lỗi từ máy chủ
    },
    init: function () {
        this.on("addedfile", function (file) {
            console.log("added file", file); // Xử lý sự kiện thêm tệp
            file.previewElement.parentNode.removeChild(file.previewElement); // Vô hiệu hóa chức năng hiển thị thông tin tệp
        });
    }
};

$(function () {
    var url = '/CourseSchedule/Pagination'
    var columns = [
        { targets: 0, data: "id", visible: false },
        { targets: 1, data: "code", sortable: false },
        { targets: 2, data: "name", sortable: false, },
        { targets: 3, data: "teacher.fullName", sortable: false, },
        { targets: 4, data: "createdDateString", sortable: false, },
        {
            targets: 5, data: "countStudentRegistrationed", sortable: false, render: function (data, type, row, meta) {
                if (data >= row.maxStudentCount) {
                    return `<span class="d-flex justify-content-center">
                                      <span class="badge badge-danger text-center text-light" >
                                       ${data}
                                    </span>
                                </span> `
                } else {
                    return `<span class="d-flex justify-content-center">
                                      <span class="badge badge-success text-center text-light" >
                                       ${data}
                                    </span>
                                </span> `
                }
            }
        },
        {
            targets: 6, data: "maxStudentCount", sortable: false, render: function (data, type, row, meta) {
                return `<span class="d-flex justify-content-center">
                                      <span class=" text-center text-dark" >
                                       ${data}
                                    </span>
                                </span> `
            }
        },
        {
            targets: 7, data: "statusCourseName", sortable: false, render: function (data, type, row, meta) {
                if (data != null) {
                    if (row.statusCourse == 1) {
                        return `<span class="d-flex justify-content-center">
                                      <span class="badge badge-primary text-center text-light" >
                                       ${data}
                                    </span>
                                </span> `;

                    } else if (row.statusCourse == 2) {
                        return `<span class="d-flex justify-content-center">
                                      <span class="badge badge-success text-center text-light" >
                                       ${data}
                                    </span>
                                </span> `;
                    } else if (row.statusCourse == 3) {
                        return `<span class="d-flex justify-content-center">
                                      <span class="badge badge-warning text-center text-light" >
                                       ${data}
                                    </span>
                                </span> `;
                    } else if (row.statusCourse == 4) {
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
            targets: 8,
            data: null,
            sortable: false,
            autoWidth: true,
            defaultContent: '',
            render: (data, type, row, meta) => {
                if (row.statusCourse == 1 || row.statusCourse == 2) {
                    return `<div class="d-flex justify-content-center">
                              <a class="btn btn-outline-primary btn-sm btn-view-modal mx-1" data-view-id="${row.id}"  data-bs-toggle="modal" data-bs-target="#modalView"><i class="fas fa-eye mx-1"></i>Thông tin</a>
                                <a class="btn btn-outline-warning btn-sm btn-edit-modal" data-edit-id="${row.id}" data-bs-toggle="modal" data-bs-target="#modalEdit"><i class="fas fa-user-edit mx-1"></i>Điều chỉnh</a>
                             
                            </div>
                            `
                } else {
                    return `
                        <div class="d-flex justify-content-center">
                             <a class="btn btn-outline-primary btn-sm btn-view-modal mx-1" data-view-id="${row.id}"  data-bs-toggle="modal" data-bs-target="#modalView"><i class="fas fa-eye mx-1"></i>Thông tin</a>
                        </div>
                         `
                }

            }
        }
    ];


    var dataSrc = "result.data"
    var datatable = createDataTable(url, columns, 'courseScheduleTableId', null, dataSrc);

    function updateDataTable(url) {
        $.ajax({
            url: url,
            type: 'GET',
            success: function (result) {
                // Xử lý kết quả thành công từ máy chủ
                if (result && result.statusCode === 200) {
                    $('#courseScheduleTableId').DataTable().clear().draw();
                    $('#courseScheduleTableId').DataTable().rows.add(result.result.data).draw();
                } else if (result && result.statusCode === 404) {
                    // Trường hợp không tìm thấy dữ liệu
                    $('#courseScheduleTableId').DataTable().rows().clear().draw(); // Xóa dữ liệu trong datatable            
                } else {
                    // Xử lý trường hợp có lỗi từ máy chủ
                    toastError(result && result.message ? result.message : "Có lỗi hệ thống! Vui lòng thử lại sau.");
                }
            },
            error: function (xhr, textStatus, error) {
                // Xử lý lỗi từ máy chủ
                toastError("Lỗi hệ thống! Vui lòng thử lại sau.");
            }
        });
    }


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
        var searchUrl = '/CourseSchedule/Search?' + queryString;
        updateDataTable(searchUrl);
    });

    $('#createModal').on('show.bs.modal', function () {
        let url = evn.app.extension.generateCode;
        var data = {
            "prefixCode": "CODEHP"
        };
        callAPI(url, "GET", data, successCallbackViewCreateModal, errorCallbackViewCreateModal)
        $('#academicTerm').select2({
            dropdownParent: $('#createModal .modal-content')
        });
        $('#teacher').select2({
            dropdownParent: $('#createModal .modal-content')
        });
    });

    $('#createModal').on('hidden.bs.modal', function () {
        $('#createCourseSchedule')[0].reset();
    });

    $('#createCourseSchedule').on('submit', function (e) {
        e.preventDefault();

        // Lấy danh sách trường trong form và chuyển đổi thành JSON
        var formData = $(this).serializeArray().reduce(function (obj, item) {
            obj[item.name] = item.value;
            return obj;
        }, {});


        var jsonData = JSON.stringify({
            model : formData
        });
        
        var url = evn.app.courseSchedule.create

        // ajax -- index.js
        callAPI(url, "POST", jsonData, successCallbackCreateData, errorCallbackViewCreateData)

    });
});

// Filter
// reset form
$(document).on('click', '.btn-reset', function () {
    $('#form-filter')[0].reset();
    reloadDataTable('registrationFormTableId');
})


//  modal - chi tiết biểu mẫu
$(document).on('click', '.btn-view-modal', function () {
    let id = $(this).attr("data-view-id")
    let url = `/CourseSchedule/ViewModal/${id}`

    callAPI(url, "GET", null, successCallbackViewModal, errorCallbackViewModal)
})

successCallbackViewModal = (content) => {
    $('#modalView div.modal-dialog div.modal-content').html(content);
    $('#modalView').modal('show');
    
}

errorCallbackViewModal = () => {
    toastError("Lỗi hệ thống, vui lòng thử lại sau")
}
successCallbackViewCreateModal = (content) => {
    if (!content) {
        toastError("Lỗi trong quá trình tạo mã học phần, vui lòng thử lại sau")
    } else {
        $("#code").val(content)
    }
}

successCallbackCreateData = (content) => {
    if (content && content.statusCode === 201) {
        toastSuccess("Tạo mới học phần thành công")
        $('#createModal').modal('hide');
    } else {
        toastError("Lỗi trong quá trình tạo mới học phần, vui lòng thử lại sau")
    }
}
errorCallbackViewCreateData = () => {
    toastError("Lỗi trong quá trình xử lý, vui lòng thử lại sau")
}
errorCallbackViewCreateModal = () => {
    toastError("Lỗi trong quá trình tạo mã học phần, vui lòng thử lại sau")
}