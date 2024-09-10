$(function () {
    $('#cancelForm').on('submit', function (e) {
        e.preventDefault();

        // Lấy danh sách trường trong form và chuyển đổi thành JSON
        var cancelRequestModel = $(this).serializeArray().reduce(function (obj, item) {
            obj[item.name] = item.value
            return obj;
        }, {});

        var isValidData = validateListTerm(cancelRequestModel);
        if (!isValidData) return;

        
        let url = `/RegistrationForm/ConfirmCancel/${cancelRequestModel.Id}`

         callAPI(url, "POST", JSON.stringify(cancelRequestModel), successCallbackConfirmCancel, errorCallbackConfirmCancel)

    });

    function validateListTerm(cancelRequestModel) {
        var isValid = true;
        var validationMessage = "";

        // Validate AcademicTerm
        if (cancelRequestModel.ReasonCancel === "") {
            isValid = false;
            validationMessage += "<p class='text-danger'>Vui lòng nhập lý do bạn muốn hủy yêu cầu này</p>";
        }

        // Display validation message
        var validationMessageDiv = document.getElementById('validationMessage');
        if (!isValid) {
            validationMessageDiv.innerHTML = validationMessage;
        } else {
            validationMessageDiv.innerHTML = ''
        }
        return isValid;
    }

    successCallbackConfirmCancel = (data) => {
        if (data.statusCode == 200 || data.statusCode == 201) {
            toastSuccess("Yêu cầu đã được xác nhận thành công")
            $('#modalCancel').modal('hide');
            reloadDataTable('registrationFormTableId');
        } else {
            toastSuccess(data.message)
        }
    }

    errorCallbackConfirmCancel = () => {
        toastError("Lỗi hệ thống, vui lòng thử lại sau")
    }
});
