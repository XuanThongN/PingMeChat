function callAPI(url, type, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: type,
        data: data,
        contentType: 'application/json', // chỉ định loại dữ liệu gửi lên theo kiểu json ( nếu không có thường gặp lỗi 415)
        /*  "beforeSend": function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token);
 
         },*/
       /* "beforeSend": function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + getTokenFromCookie("UserLoginCookie"));

        },*/
        success: function (result) {
            successCallback(result);
        },
        error: function (xhr, status, error) {
            errorCallback(xhr, status, error);
        }
    });
}
function getTokenFromCookie(cname) { // lấy token từ js trên brower
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(";");
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == " ") {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function getTokenFromServer() {
    return new Promise((resolve, reject) => {
        $.ajax({
            type: 'GET',
            url: '/Admin/Account/GetAccessToken',
            success: function (data) {
                resolve(data.accessToken);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function submitForm(url, jsonData, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        data: jsonData,
        success: successCallback,
        error: errorCallback
    });
}

function GetData(url, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: 'GET',
        contentType: 'application/json',
        success: successCallback,
        error: errorCallback
    });
}

// trả về dạng Patial View trong .Net
/*function getPartialView(fromClass, toClass, url) {
    $(document).on('click', '.' + fromClass, function (e) {

        e.preventDefault();
        $.ajax({
            url: url,
            type: 'Get',
            dataType: 'html',
            success: function (content) {
                $('.' + toClass).html(content);
            },
            error: function (e) {

            }
        })
    });
}*/


function getPartialViewModal(url, successCallback, errorCallback) {
    $.ajax({
        type: "GET",
        url: url,
        dataType: 'html',
    }).done(function (content) {
        successCallback(content)
    }).fail(function (xhr, status, error) {
        errorCallback(xhr, status, error)
    });
}

    