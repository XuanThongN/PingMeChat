const MoneyUtils = (function () {
    // Hàm định dạng số thành tiền tệ
    function formatMoney(number, currency = 'VND') {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: currency
        }).format(number);
    }

    // Hàm chuyển đổi từ chuỗi tiền tệ sang số
    function parseMoneyToNumber(moneyString) {
        return parseFloat(moneyString.replace(/[^\d,-]/g, '').replace(',', '.'));
    }

    // Hàm khởi tạo Cleave.js cho input tiền tệ
    function initMoneyInput(input) {
        new Cleave(input, {
            numeral: true,
            numeralThousandsGroupStyle: 'thousand',
            numeralDecimalMark: ',',
            delimiter: '.'
        });

        $(input).on('input', function () {
            var rawValue = parseMoneyToNumber($(this).val());
            $(this).attr('data-raw-value', rawValue);
        });

        // Định dạng giá trị ban đầu nếu có
        var initialValue = $(input).attr('data-raw-value');
        if (initialValue) {
            $(input).val(formatMoney(initialValue));
        }
    }

    // Hàm lấy giá trị raw từ input tiền tệ
    function getRawValue(input) {
        return parseFloat($(input).attr('data-raw-value') || 0);
    }

    // Hàm set giá trị cho input tiền tệ
    function setMoneyValue(input, value) {
        $(input).val(formatMoney(value));
        $(input).attr('data-raw-value', value);
    }

    // Public API
    return {
        formatMoney: formatMoney,
        parseMoneyToNumber: parseMoneyToNumber,
        initMoneyInput: initMoneyInput,
        getRawValue: getRawValue,
        setMoneyValue: setMoneyValue
    };
})();