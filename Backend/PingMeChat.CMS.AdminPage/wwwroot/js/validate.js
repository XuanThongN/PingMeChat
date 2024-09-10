function validateTime(startTimeId, endTimeId, inputId) {
    const startDate = convertToDate(document.getElementById(startTimeId).value);
    const endDate = convertToDate(document.getElementById(endTimeId).value);
    const inputElement = document.getElementById(inputId);
    const errorMessage = document.createElement("p");
    errorMessage.classList.add("text-danger"); // add class "text-danger"

    // Remove previous error messages
    const previousErrorMessages = inputElement.parentNode.querySelectorAll("p.text-danger");
    previousErrorMessages.forEach(message => message.remove());

    if (!document.getElementById(startTimeId).value && !document.getElementById(endTimeId).value) {
        return false;
    }

    if (startDate == null) {
        errorMessage.innerText = "Ngày bắt đầu không hợp lệ";
        inputElement.parentNode.insertBefore(errorMessage, inputElement.nextSibling);
        return true;
    }

    if (endDate == null) {
        errorMessage.innerText = "Ngày kết thúc không hợp lệ";
        inputElement.parentNode.insertBefore(errorMessage, inputElement.nextSibling);
        return true;
    }

    if (startDate > endDate) {
        errorMessage.innerText = "Ngày bắt đầu không thể lớn hơn ngày kết thúc";
        inputElement.parentNode.insertBefore(errorMessage, inputElement.nextSibling);
        return true;
    }

    return false;
}

function validateDates() {
    const startDateInput = document.getElementById("startDate");
    const endDateInput = document.getElementById("endDate");
    const errorElement = document.getElementById("dateError");

    const startDate = new Date(startDateInput.value);
    const endDate = new Date(endDateInput.value);

    // Kiểm tra tính hợp lệ của ngày
    if (isNaN(startDate.getTime()) && isNaN(endDate.getTime())) {
        errorElement.textContent = "Ngày không hợp lệ";
        return false;
    }

    // Kiểm tra nếu ngày bắt đầu lớn hơn ngày kết thúc
    if (startDate > endDate) {
        errorElement.textContent = "Ngày bắt đầu không được lớn hơn ngày kết thúc.";
        return false;
    } else {
        errorElement.textContent = "";
        return true;
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const startDateInput = document.getElementById("startDate");
    const endDateInput = document.getElementById("endDate");

    startDateInput?.addEventListener("change", validateDates);
    endDateInput?.addEventListener("change", validateDates);
});

function convertToDate(dateString) {
    const parts = dateString.split('/');
    if (parts.length !== 3) {
        return null;
    }
    const year = parseInt(parts[2]);
    const month = parseInt(parts[1]) - 1;
    const day = parseInt(parts[0]);
    return new Date(year, month, day);
}

// Hàm dùng chung để validate các trường
function validateField(field, rules) {
    let isValid = true;
    let errorMessage = '';
    const fieldValue = field.val()?.trim();
    const isSelectElement = field.is('select');
    
    if (rules.required) {
        if (isSelectElement) {
            if (fieldValue === null || fieldValue === '' || fieldValue === undefined) {
                isValid = false;
                errorMessage = 'Vui lòng chọn một mục.';
            }
        } else if (fieldValue === '') {
            isValid = false;
            errorMessage = 'Trường này là bắt buộc.';
        }
    }

    if (isValid && fieldValue !== '') {
        if (rules.minLength && fieldValue.length < rules.minLength) {
            isValid = false;
            errorMessage = `Độ dài tối thiểu là ${rules.minLength} ký tự.`;
        } else if (rules.maxLength && fieldValue.length > rules.maxLength) {
            isValid = false;
            errorMessage = `Độ dài tối đa là ${rules.maxLength} ký tự.`;
        } else if (rules.pattern && !rules.pattern.test(fieldValue)) {
            isValid = false;
            errorMessage = 'Giá trị không đúng định dạng.';
        } else if (rules.number) {
            const numberValue = parseFloat(fieldValue);
            if (isNaN(numberValue)) {
                isValid = false;
                errorMessage = 'Vui lòng nhập một số hợp lệ.';
            } else {
                if (rules.min !== undefined && numberValue < rules.min) {
                    isValid = false;
                    errorMessage = `Giá trị tối thiểu là ${rules.min}.`;
                } else if (rules.max !== undefined && numberValue > rules.max) {
                    isValid = false;
                    errorMessage = `Giá trị tối đa là ${rules.max}.`;
                }
            }
        } else if (rules.money) {
            // Loại bỏ các ký tự không phải số và ký tự phân cách (vd: ₫, ., ,)
            const cleanValue = fieldValue.replace(/[^\d,-]/g, '').replace(',', '.');
            const moneyPattern = /^\d+(\.\d{1,2})?$/;
            if (!moneyPattern.test(cleanValue)) {
                isValid = false;
                errorMessage = 'Vui lòng nhập số tiền hợp lệ (ví dụ: 1000 hoặc 1000.50).';
            } else {
                const moneyValue = parseFloat(fieldValue.replace(/\./g, ''));
                if (rules.min !== undefined && moneyValue < rules.min) {
                    isValid = false;
                    errorMessage = `Số tiền tối thiểu là ${rules.min}.`;
                } else if (rules.max !== undefined && moneyValue > rules.max) {
                    isValid = false;
                    errorMessage = `Số tiền tối đa là ${rules.max}.`;
                }
            }
        } else if (rules.email) {
            const emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
            if (!emailPattern.test(fieldValue)) {
                isValid = false;
                errorMessage = 'Vui lòng nhập một địa chỉ email hợp lệ.';
            }
        } else if (rules.phone) {
            const phonePattern = /^(0|\+84)(\s|\.)?((3[2-9])|(5[689])|(7[06-9])|(8[1-689])|(9[0-46-9]))(\d)(\s|\.)?(\d{3})(\s|\.)?(\d{3})$/;
            if (!phonePattern.test(fieldValue)) {
                isValid = false;
                errorMessage = 'Vui lòng nhập một số điện thoại hợp lệ (10 số, bắt đầu bằng 0 hoặc +84).';
            }
        }
        else if (rules.username) {
            const usernamePattern = /^[a-zA-Z0-9_]+$/;
            if (!usernamePattern.test(fieldValue)) {
                isValid = false;
                errorMessage = 'Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới.';
            }
        }
        else if (rules.password) {
            const uppercasePattern = /[A-Z]/;
            const lowercasePattern = /[a-z]/;
            const specialCharPattern = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/;

            if (!uppercasePattern.test(fieldValue)) {
                isValid = false;
                errorMessage = 'Mật khẩu phải chứa ít nhất một chữ cái hoa.';
            } else if (!lowercasePattern.test(fieldValue)) {
                isValid = false;
                errorMessage = 'Mật khẩu phải chứa ít nhất một chữ cái thường.';
            } else if (!specialCharPattern.test(fieldValue)) {
                isValid = false;
                errorMessage = 'Mật khẩu phải chứa ít nhất một ký tự đặc biệt.';
            }
        }
    }


    // Thêm kiểm tra mật khẩu trùng khớp
    if (rules.matchPassword) {
        const passwordField = document.querySelector(rules.matchPassword);
        console.log("fieldValue", fieldValue)
        console.log("passwordField.val().trim()", passwordField)
        if (passwordField.length > 0 && fieldValue !== passwordField.value.trim()) {
            isValid = false;
            errorMessage = 'Mật khẩu không khớp.';
        }
    }

    // Hiển thị hoặc ẩn thông báo lỗi
    let errorElement = field.next('.error-message');
    if (errorElement.length === 0) {
        errorElement = $(`<div class="error-message text-danger"></div>`);
        field.after(errorElement);
    }
    errorElement.text(errorMessage);
    errorElement.toggle(!isValid);

    return isValid;
}