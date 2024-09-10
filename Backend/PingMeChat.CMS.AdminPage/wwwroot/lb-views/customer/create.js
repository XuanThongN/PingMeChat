


$(document).ready(function () {
    $('#submitForm').on('submit', function (e) {
        e.preventDefault();
        if (validateForm()) {
            var formData = $(this).serializeArray().reduce(function (obj, item) {
                obj[item.name] = item.value;
                return obj;
            }, {});

            var jsonData = JSON.stringify(formData);
            var url = "/customer/create";
            LoadingOverlay.show(); // Hiển thị loading
            submitForm(url, jsonData,
                function (response) {
                    LoadingOverlay.hide(); // Ẩn loading
                    // Success callback
                    alert("Tạo mới thành công");
                    resetFormAfterSubmit();
                },
                function (xhr, status, error) {
                    LoadingOverlay.hide(); // Ẩn loading
                    // Error callback
                    alert("Đã xảy ra lỗi. Vui lòng thử lại.");
                    console.error("Error:", error);
                }
            );
        }
    });


    // Tạo hàm để validate form
    function validateForm() {
        let isValid = true;

        // Validate tên khách hàng
        isValid = validateField($('#fullNameCustomer'), {
            required: true,
            minLength: 3,
            maxLength: 50
        }) && isValid;

        // Validate cho trường số điện thoại
        isValid = validateField($('#phoneNumberCustomer'), {
            required: true,
            phone: true,

        }) && isValid;

        // Validate cho trường địa chỉ
        isValid = validateField($('#Address'), {
            required: true,
            minLength: 3,
            maxLength: 225,
        }) && isValid;

        return isValid;
    }
});

function resetFormAfterSubmit() {
    $('#submitForm')[0].reset();
    $('.error-message').remove();
}