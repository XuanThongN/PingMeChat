(function ($) {

    // Clear dữ liệu form
    $.fn.clearForm = function () {
        var $this = $(this);
        $this.validate().resetForm();
        $('[name]', $this).each((i, obj) => {
            $(obj).removeClass('is-invalid error-message');
        });

        // Clear các input tiền tệ
        $('[data-raw-value]', $this).each((i, obj) => {
            MoneyUtils.setMoneyValue(obj, 0);  // Reset value về 0
        });

        $this[0].reset();
    };



    // Chuyển form thành FormData
    $.fn.convertFormToFormData = function () {
        const formData = this.serializeArray();
        let filterData = {};

        $.each(formData, function (_, field) {
            const element = $(`[name="${field.name}"]`);
            let value = field.value.trim();

            // Bỏ qua các trường không có giá trị
            if (value === '') return;

            // Chuyển đổi giá trị từ select thành số nguyên và kiểm tra element có attribute là data-enum hay không
            if (element.is('select') && !element.attr('data-enum')) {
                value = parseInt(value, 10);
                if (isNaN(value)) return; // Bỏ qua nếu không phải số hợp lệ
            }

            // Chuyển đổi giá trị từ input money thành số
            if (element.hasClass('money-input')) {
                value = MoneyUtils.parseMoneyToNumber(value);
            }

            filterData[field.name] =  value;
        });

        return filterData;
    };

})(jQuery);



