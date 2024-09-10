//var connection = new signalR.HubConnectionBuilder().withUrl("/hub-container").build();

//function showModalDetailNotifi(title, content) {
//    console.log("noti", title + content)
//    toastMessage(title, content)
//}

//connection.on("NotificationForAdmin", function (data) {
//    // Xử lý dữ liệu nhận được
//    console.log("data", data);
//    let content = '';
//    data.forEach(e => {
//        if (e.status == 4) {
//            content += `
//          <a  class="dropdown-item" onclick="showModalDetailNotifi('${e.title}', '${e.content}')">
//                <i class="fas fa-envelope mr-2 text-warning"></i> ${e.title}
//                <p class="ms-4 my-2 text-success"><b> -- ${e.statusName} --</b></p>
//            </a>
//            <div class="dropdown-divider"></div>
//    `
//        } else if (e.status == 5) {
//            content += `
//                  <a  class="dropdown-item" onclick="showModalDetailNotifi('${e.title}', '${e.content}')">

//                <i class="fas fa-envelope mr-2 text-warning"></i> ${e.title}
//                <p class="ms-4 my-2 text-danger"><b> -- ${e.statusName} --</b></p>
//            </a>
//            <div class="dropdown-divider"></div>
//    `
//        } else {
//            content += `
//                  <a  class="dropdown-item" onclick="showModalDetailNotifi('${e.title}', '${e.content}')">

//                <i class="fas fa-envelope mr-2 text-warning"></i> ${e.title}
//                <p class="ms-4 my-2 text-primary"><b> -- ${e.statusName} --</b></p>
//            </a>
//            <div class="dropdown-divider"></div>
//    `
//        }
//    })

//    $("#notificationId").html(content)
//});

//connection.start().catch(function (err) {
//    console.error(err.toString());
//});

//Initialize Select2 Elements
$(function () {
    //Initialize Select2 Elements
    $('.select2').select2({
        placeholder: "Select an option",
        allowClear: true,
        search: true,  // Đảm bảo tính năng tìm kiếm được bật
        searchInputPlaceholder: 'Tìm kiếm ở đây...'
    });
    //let url = "/Notification/Pagination";
    //callAPI(url, "GET", null, successCallback, errorCallback)

})
//successCallback = (data) => {
//    console.log("data", data)

//    if (data.statusCode == 200) {
//        let notifications = data.result.data;
//        let content = '';
//        notifications.forEach(e => {
//            if (e.status == 4) {
//                content += `
//                 <a  class="dropdown-item" onclick="showModalDetailNotifi('${e.title}', '${e.content}')">
//                <i class="fas fa-envelope mr-2 text-warning"></i> ${e.title}
//                <p class="ms-4 my-2 text-success"><b> -- ${e.statusName} --</b></p>
//            </a>
//            <div class="dropdown-divider"></div>
//    `
//            } else if (e.status == 5) {
//                content += `
//                  <a  class="dropdown-item" onclick="showModalDetailNotifi('${e.title}', '${e.content}')">
//                <i class="fas fa-envelope mr-2 text-warning"></i> ${e.title}
//                <p class="ms-4 my-2 text-danger"><b> -- ${e.statusName} --</b></p>
//            </a>
//            <div class="dropdown-divider"></div>
//    `
//            } else {
//                content += `
//                   <a  class="dropdown-item" onclick="showModalDetailNotifi('${e.title}', '${e.content}')">
//                <i class="fas fa-envelope mr-2 text-warning"></i> ${e.title}
//                <p class="ms-4 my-2 text-primary"><b> -- ${e.statusName} --</b></p>
//            </a>
//            <div class="dropdown-divider"></div>
//    `
//            }

//        })

//        $("#notificationId").html(content)
//    }
//}
//errorCallback = (error) => {
//    toastError("Có lỗi khi khỏi tạo thông báo cho ứng dụng")
//}


//định dạng bộ lọc theo ngày tháng năm theo định dạng Việt Nam
$(function () {
    //$('#reservationdate').datetimepicker({
    //    format: 'L',
    //    // chuyển đổi sang ngôn ngữ tiếng việt
    //    locale: 'vi',
    //    //  hiển thị ngày tháng năm và giờ phút giây
    //    format: 'DD/MM/YYYY '
    //});
    $('#reservationtime').daterangepicker({
        locale: {
            format: 'DD/MM/YYYY HH:mm A',
            applyLabel: 'Chọn',
            cancelLabel: 'Hủy',
            fromLabel: 'Từ',
            toLabel: 'Đến',
            customRangeLabel: 'Tùy chỉnh',
            daysOfWeek: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
            monthNames: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
            firstDay: 1

        },
        //startDate: moment().startOf('month'),
        //endDate: moment().endOf('month'),
        startDate: moment().startOf('day'),
        endDate: moment().endOf('day'),
        timePicker: true,
        timePickerIncrement: 1,
    });
});

// chuyển đổi kiểu DateTime từ server thành định dạng ngày/tháng/năm
function fomartDateTimeServer(data) {
    var date = new Date(data);
    var formattedDate = date.toLocaleDateString('vi-VN', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
    });

    return formattedDate
}

// format ngày tháng năm
function formatDate(date) {
    var day = date.getDate().toString().padStart(2, '0');
    var month = (date.getMonth() + 1).toString().padStart(2, '0');
    var year = date.getFullYear();

    return "ngày " + day + " tháng " + month + " năm " + year;
}

// Đối tượng ngôn ngữ cho Việt Nam
//Date range picker in filter
var vietnameseLocale = {
    format: 'DD/MM/YYYY',
    separator: ' - ',
    applyLabel: 'Áp dụng',
    cancelLabel: 'Hủy',
    fromLabel: 'Từ',
    toLabel: 'Đến',
    customRangeLabel: 'Tùy chỉnh',
    weekLabel: 'Tuần',
    daysOfWeek: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
    monthNames: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
    firstDay: 1
};

function tablePagination(pageNumber, totalPage) {
    // previous - next
    if (pageNumber <= 1) {
        $("#previous").addClass("disabled")
    } else {
        $("#previous").removeClass("disabled")
    }
    if (pageNumber == totalPage || totalPage <= 0) {
        $("#next").addClass("disabled")
    } else {
        $("#next").removeClass("disabled")
    }
}

// hide modal
$(document).on('click', '.closeModal', function () {
    $('.modal').modal('hide');
});

// hide -show filter
// Toggle form-filter visibility on button click
$(document).on('click', '#btn-filter', function () {
    $('#form-filter').toggle(); // Toggle visibility
});

// Format number to currency VND
function numberFormatCurrency() {
    return function (data, type, row) {
        return (data)
            ? new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(data)
            : null;
    }
}

function formatDateTime(dateTimeString, format) {
    // Kiểm tra nếu dateTimeString là null hoặc undefined
    if (!dateTimeString) return '';

    // Chuyển đổi chuỗi thành đối tượng Date
    const date = new Date(dateTimeString);

    // Kiểm tra xem date có hợp lệ không
    if (isNaN(date.getTime())) return 'Invalid Date';

    // Hàm để thêm số 0 phía trước nếu số nhỏ hơn 10
    const pad = (num) => (num < 10 ? '0' + num : num);

    // Lấy các thành phần của ngày
    const day = pad(date.getDate());
    const month = pad(date.getMonth() + 1); // Tháng bắt đầu từ 0
    const year = date.getFullYear();
    const hours = pad(date.getHours());
    const minutes = pad(date.getMinutes());
    const seconds = pad(date.getSeconds());

    // Định dạng theo yêu cầu
    switch (format) {
        case 'date':
            return `${day}/${month}/${year}`;
        case 'datetime':
            return `${day}/${month}/${year}, ${hours}:${minutes}:${seconds}`;
        case 'datetime-short':
            return `${day}/${month}/${year}, ${hours}:${minutes}`;
        default:
            return `${day}/${month}/${year}, ${hours}:${minutes}:${seconds}`;
    }
}

// Giả sử dateTimeString là chuỗi DateTime nhận từ server
//let dateTimeString = "2023-07-24T15:30:45.123Z";

//// Các cách gọi hàm
//console.log(formatDateTime(dateTimeString, 'date')); // Kết quả: 24/07/2023
//console.log(formatDateTime(dateTimeString, 'datetime')); // Kết quả: 24/07/2023, 15:30:45
//console.log(formatDateTime(dateTimeString, 'datetime-short')); // Kết quả: 24/07/2023, 15:30
//console.log(formatDateTime(dateTimeString)); // Mặc định: 24/07/2023, 15:30:45


function formatCurrency(amount) {
    // Kiểm tra nếu amount không phải là số
    if (isNaN(amount)) {
        return 'Invalid Amount';
    }

    // Chuyển đổi số thành chuỗi và làm tròn đến 2 chữ số thập phân
    let formattedAmount = Number(amount).toFixed(2);

    // Tách phần nguyên và phần thập phân
    let parts = formattedAmount.split('.');
    let integerPart = parts[0];
    let decimalPart = parts[1];

    // Thêm dấu chấm để phân tách hàng nghìn
    integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, '.');

    // Kết hợp lại và thêm đơn vị tiền tệ
    let result = integerPart;

    // Nếu có phần thập phân và nó khác '00', thêm vào kết quả
    if (decimalPart && decimalPart !== '00') {
        result += ',' + decimalPart;
    }

    return result + ' VNĐ';
}

//console.log(formatCurrency(17000000)); // Kết quả: 17.000.000 VNĐ
//console.log(formatCurrency(1234567.89)); // Kết quả: 1.234.567,89 VNĐ
//console.log(formatCurrency(1000)); // Kết quả: 1.000 VNĐ
//console.log(formatCurrency(1000.5)); // Kết quả: 1.000,50 VNĐ
//console.log(formatCurrency('abc')); // Kết quả: Invalid Amount


function convertTotalTimeToHoursAndMinutes(totalTimeInMinutes) {
    const hours = Math.floor(totalTimeInMinutes / 60);
    const minutes = Math.floor(totalTimeInMinutes % 60);
    return `${hours} giờ ${minutes} phút`;
}


// Sử dụng hàm
//const totalTime = 287; // Giá trị TotalTime từ server trả về
//const formattedTime = convertTotalTimeToHoursAndMinutes(totalTime);
//console.log(formattedTime); // Kết quả: 4 giờ 47 phút


function formatProductivityUsing(decimalValue) {
    // Làm tròn đến 2 chữ số thập phân
    const roundedValue = decimalValue.toFixed(2);
    // Thêm ký hiệu phần trăm
    return `${roundedValue} %`;
}

// Sử dụng hàm
//const productivity = 0.1007; // Giá trị decimal từ server trả về
//const formattedProductivity = formatProductivityUsing(productivity);
//console.log(formattedProductivity); // Kết quả: "10.07 %"

// tính công suất sử dụng
function calculateProductivityUsing(startDate, endDate, totalPlayMinutes) {
    // Chuyển đổi chuỗi ngày thành đối tượng Date
    const start = new Date(startDate);
    const end = new Date(endDate);

    // Kiểm tra tính hợp lệ của ngày
    if (isNaN(start.getTime()) || isNaN(end.getTime())) {
        throw new Error("Ngày không hợp lệ");
    }

    // Kiểm tra nếu ngày kết thúc trước ngày bắt đầu
    if (end < start) {
        throw new Error("Ngày kết thúc phải sau ngày bắt đầu");
    }

    // Thiết lập thời gian của ngày bắt đầu là đầu của ngày
    start.setHours(0, 0, 0, 0);

    // Thiết lập thời gian của ngày kết thúc là cuối của ngày
    end.setHours(23, 59, 59, 999);

    // Tính tổng số phút có thể sử dụng
    const totalPossibleMinutes = (end - start) / (1000 * 60);

    // Tính công suất sử dụng
    let productivityPercentage = (totalPlayMinutes / totalPossibleMinutes) * 100;

    // Làm tròn đến 2 chữ số thập phân và giới hạn trong khoảng 0-100
    productivityPercentage = Math.min(100, Math.max(0, parseFloat(productivityPercentage.toFixed(2))));

    return productivityPercentage;
}

// Ví dụ sử dụng
//try {
//    const startDate = "2024-07-01T00:00:00";
//    const endDate = "2024-07-07T23:59:59";
//    const totalPlayMinutes = 3600; // Ví dụ: 60 giờ chơi (3600 phút)

//    const productivity = calculateProductivityUsing(startDate, endDate, totalPlayMinutes);
//    console.log(`Công suất sử dụng: ${productivity}%`);
//} catch (error) {
//    console.error("Lỗi:", error.message);
//}

function convertDateRangeToHoursAndMinutes(startDate, endDate) {
    // Chuyển đổi chuỗi ngày thành đối tượng Date
    const start = new Date(startDate);
    const end = new Date(endDate);

    // Kiểm tra tính hợp lệ của ngày
    if (isNaN(start.getTime()) || isNaN(end.getTime())) {
        console.log("Ngày không hợp lệ");
        return "Ngày không hợp lệ";
    }

    // Thiết lập thời gian của ngày bắt đầu là đầu của ngày
    start.setHours(0, 0, 0, 0);

    // Thiết lập thời gian của ngày kết thúc là cuối của ngày
    end.setHours(23, 59, 59, 999);

    // Kiểm tra nếu ngày bắt đầu và kết thúc bằng nhau
    if (start.toDateString() === end.toDateString()) {
        return "23 giờ 59 phút";
    }

    // Tính tổng số phút giữa hai ngày
    const totalMinutes = Math.floor((end - start) / (1000 * 60));

    // Tính số giờ và số phút
    const hours = Math.floor(totalMinutes / 60);
    const minutes = totalMinutes % 60;

    // Định dạng kết quả
    const hoursStr = hours.toString().padStart(2, '0');
    const minutesStr = minutes.toString().padStart(2, '0');

    return `${hoursStr} giờ ${minutesStr} phút`;
}

// Ví dụ sử dụng
//try {
//    const startDate = "2024-07-01T10:30:00";
//    const endDate = "2024-07-03T14:45:00";

//    const result = convertDateRangeToHoursAndMinutes(startDate, endDate);
//    console.log(`Thời gian giữa hai ngày: ${result}`);
//} catch (error) {
//    console.error("Lỗi:", error.message);
//}

// Định dạng số điện thoại Việt Nam
function formatVietnamesePhoneNumber(phoneNumber) {
    // Loại bỏ mọi ký tự không phải số
    phoneNumber = phoneNumber.replace(/\D/g, '');

    // Kiểm tra độ dài số điện thoại
    if (phoneNumber.length === 10) {
        // Định dạng số di động
        return phoneNumber.replace(/(\d{3})(\d{3})(\d{4})/, '$1 $2 $3');
    } else if (phoneNumber.length === 11) {
        // Định dạng số cố định
        return phoneNumber.replace(/(\d{2})(\d{3})(\d{4})(\d{2})/, '$1 $2 $3 $4');
    } else {
        // Số điện thoại không hợp lệ
        return 'Số điện thoại không hợp lệ';
    }
}


// Bắt sự kiện khi focus vào tất cả các input
$(document).on('focus', 'input[type="text"], input[type="number"]', function () {
    $(this).select();
});