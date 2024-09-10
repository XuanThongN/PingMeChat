function submitForm(url, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: "POST",
        data: data,
        contentType: 'application/json',
        success: function (response) {
            if (successCallback && typeof successCallback === 'function') {
                successCallback(response);
            }
        },
        error: function (xhr, status, error) {
            if (errorCallback && typeof errorCallback === 'function') {
                errorCallback(xhr, status, error);
            }
        }
    });
}




//$(function () {
//    // khởi tạo ngày gửi biểu mẫu
//    var currentDate = new Date();
//    var dateSubmit = `Huế, ${formatDate(currentDate)}`
//    $("#dateSubmit").html(dateSubmit)

//    // refresh mã captch
//    $("#refreshCaptcha").click(function () {
//        var captchaImage = $("#captchaImage");
//        var timestamp = new Date().getTime(); // Tạo timestamp mới để tránh cache ảnh
//      //  captchaImage.attr("src", "@Url.Action("GetCaptchaImage", "Captcha")" + "?timestamp=" + timestamp);
//        captchaImage.attr("src", "/captcha/get-captcha" + "?timestamp=" + timestamp);
//    });
//})

//function submitForm(url, data, idForm) {
//    var checkSuccess = true;
//    $.ajax({
//        url: url,
//        type: "POST",
//        data: data,
//        contentType: 'application/json',
//        success: function (response){
//           // checkSuccess = successCallback(response, idForm)
//            console.log("checkSuccess", checkSuccess)
//        },     
//        error: function (request, status, error) {
//           // toastError("Đăng ký đơn mới không thành công")
//            checkSuccess =  false;
//        }

//    });
//    return checkSuccess;
//}
//successCallback = (response, idForm ) => {
//    if (response.statusCode == 201) {
//        $(`${idForm}`)[0].reset();
//        Swal.fire({
//            icon: 'success',
//            title: 'Yêu cầu thành công!',
//            text: 'Bạn có muốn chuyển hướng tới trang quản lý biểu mẫu?',
//            showCancelButton: false,
//            confirmButtonText: 'Ok',
//        }).then((result) => {
//            if (result.isConfirmed) {
//                window.location.href = '/Admin/Form';
//            }
//            else {
//                return true;
//            }
//        })
//        return true;

//    } else if (response.statusCode == 400) {
//        toastError(response.message)
//        var captchaImage = document.getElementById("captchaImage");
//        captchaImage.src = "/captcha/get-captcha?" + new Date().getTime();

//        $('html, body').animate({
//            scrollTop: $('#captchaImage').offset().top
//        }, 1000);

//        return false;
//    }
//    else {
//        toastError(response.message)
//        var captchaImage = document.getElementById("captchaImage");
//        captchaImage.src = "/captcha/get-captcha?" + new Date().getTime();

//        $('html, body').animate({
//            scrollTop: $('#captchaImage').offset().top
//        }, 1000);
//        return false;

//    }
//}