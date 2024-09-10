$(function () {
    $('#feedbackForm').on('submit', function (e) {
        e.preventDefault();

        // Lấy danh sách trường trong form và chuyển đổi thành JSON
        var feedbackRequestModel = $(this).serializeArray().reduce(function (obj, item) {
            obj[item.name] = item.value
            return obj;
        }, {});

        let url = `/RegistrationForm/Feedback`

        callAPI(url, "POST", JSON.stringify(feedbackRequestModel), successCallback, errorCallback)

    });

    successCallback = (data) => {
        if (data.statusCode == 200 || data.statusCode == 201) {
           // toastSuccess("Yêu cầu đã được xác nhận thành công")
            $('#modalFeedBack').modal('hide');
            reloadDataTable('registrationFormTableId');
        } 
    }

    errorCallback = () => {
        toastError("Lỗi hệ thống, vui lòng thử lại sau")
    }
});
