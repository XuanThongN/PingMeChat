
$(function () {
    var productList = [];
    var $select;
    // Khởi tạo các input tiền tệ
    $('.money-input').each(function () {
        MoneyUtils.initMoneyInput(this);
    });
    // Khởi tạo DataTable
    var table = $('#tableId').DataTable({
        columns: [
            {
                // Cột số thứ tự
                data: null,
                title: 'STT',
                orderable: false,
                searchable: false,
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'name', title: 'Tên sản phẩm' },
            {
                data: 'quantity',
                title: 'Số lượng',
                render: function (data, type, row) {
                    return type === 'display' ? data.toLocaleString('vi-VN') : data;
                }
            },
            {
                data: 'price',
                title: 'Giá xuất',
                render: function (data, type, row) {
                    return type === 'display' ? MoneyUtils.formatMoney(data) : data;
                }
            },
            {
                data: 'total',
                title: 'Tổng giá',
                render: function (data, type, row) {
                    return type === 'display' ? MoneyUtils.formatMoney(data) : data;
                }
            },
            {
                data: null,
                title: 'Thao tác',
                orderable: false,
                searchable: false,
                render: function (data, type, row) {
                    return '<button class="btn btn-sm btn-warning edit-product">Sửa</button> ' +
                        '<button class="btn btn-sm btn-danger delete-product">Xóa</button>';
                }
            }
        ]
    });

    // reset form
    $(document).on('click', '.btn-reset', function () {
        $('#form-filter')[0].reset();
        updateDataTable(url, columns, tableId, reqData, dataSrc)
    })



    // Xử lý thêm sản phẩm vào đơn
    $('#addToOrder').on('click', function () {
        if (validateForm()) {
            var productId = $('#productId').val();
            var productName = $('#productId option:selected').text();
            var price = MoneyUtils.getRawValue($('input[name="price"]'));
            var quantity = $('input[name="quantity"]').val();

            //Kiểm tra số lượng sản phẩm còn trong kho có đủ không
            if(!checkQuantity(quantity)) return;

            // Kiểm tra xem sản phẩm đã tồn tại trong danh sách chưa
            var existingProduct = productList.find(item => item.id === productId);

            if (existingProduct) {
                // Nếu sản phẩm đã tồn tại, hiển thị thông báo
                toastError("Sản phẩm đã tồn tại trong danh sách!");
            } else {
                // Nếu sản phẩm chưa tồn tại, thêm vào danh sách
                var newProduct = {
                    id: productId,
                    name: productName,
                    quantity: parseInt(quantity),
                    price: parseFloat(price),
                    total: parseFloat(price) * parseInt(quantity),
                };

                productList.push(newProduct);
                table.row.add(newProduct).draw();
                updateOrderSummary();
                $('#addProductModal').modal('hide');
                $('#addProductForm')[0].reset();
                toastSuccess("Đã thêm sản phẩm vào danh sách.");
            }
        }
    });

    //Kiểm tra người dùng xuất số lượng sản phẩm lớn hơn số lượng tồn kho
    function checkQuantity(inputQuantity) {
        var selectedElement = $("#addProductModal").find('.select2-selection__rendered');
        if (selectedElement.length) {
            var productName = selectedElement.find('.service-name').text();
            var quantity = selectedElement.find('.service-quantity').attr("data-service-quantity");

            if (inputQuantity > parseInt(quantity)) {
                toastError(`Số lượng sản phẩm ${productName} trong kho không đủ!`);
                return false;
            }
        }
        return true
    }
    

    // Xử lý xóa sản phẩm
    $('#tableId').on('click', '.delete-product', function () {
        var row = table.row($(this).parents('tr'));
        var data = row.data();
        productList = productList.filter(item => item.id !== data.id);
        row.remove().draw();
        updateOrderSummary();
    });

    // Xử lý sửa sản phẩm
    $('#tableId').on('click', '.edit-product', function () {
        var row = table.row($(this).parents('tr'));
        var data = row.data();

        $('#editProductId').val(data.id);
        $('#editProductName').val(data.name);

        // Sử dụng setMoneyValue để đặt giá trị cho trường price
        MoneyUtils.setMoneyValue($('#editPrice')[0], data.price);

        $('#editQuantity').val(data.quantity);

        $('#editProductModal').modal('show');
    });

    // Xử lý cập nhật sản phẩm
    $('#updateProduct').on('click', function () {
        if (validateEditForm()) {
            var productId = $('#editProductId').val();
            var price = MoneyUtils.getRawValue($('#editPrice')[0]);
            var quantity = $('#editQuantity').val();

            var updatedProduct = {
                id: productId,
                name: $('#editProductName').val(),
                quantity: parseInt(quantity),
                price: parseFloat(price),
                total: parseFloat(price) * parseInt(quantity),
            };

            var index = productList.findIndex(item => item.id === productId);
            if (index !== -1) {
                productList[index] = updatedProduct;
            }

            table.row(function (idx, data, node) {
                return data.id === productId;
            }).data(updatedProduct).draw();

            updateOrderSummary();
            $('#editProductModal').modal('hide');
            toastSuccess("Đã cập nhật sản phẩm.");
        }
    });

    // Đảm bảo khởi tạo input tiền tệ trong modal chỉnh sửa
    $('#editProductModal').on('show.bs.modal', function () {
        MoneyUtils.initMoneyInput($('#editPrice')[0]);
    });
    function updateOrderSummary() {
        var totalAmount = productList.reduce((sum, product) => sum + product.total, 0);
        $('.order-summary th:contains("Tổng tiền thanh toán")').next().text(formatCurrency(totalAmount.toFixed(2)));
        // Cập nhật các giá trị khác nếu cần
    }

    // Khởi tạo Select2 và lấy danh sách sản phẩm
    $('#addProductModal').on('show.bs.modal', function (event) {
        $select = $('.select2').select2({
            dropdownParent: $('#addProductModal'),
            width: '100%',
            placeholder: 'Chọn sản phẩm',
            allowClear: true
        });

        getAllProduct();
    });

    // lấy danh sách sản phẩm để hiện thị ở select 2
    function getAllProduct() {
        var url = "/Inventory/GetAll";

        GetData(url,
            function (response) {
                if (response.statusCode === 200) {
                    var services = response.result;

                    $select.empty();

                    $select.select2({
                        placeholder: 'Chọn sản phẩm',
                        data: services.map(service => ({
                            id: service.productId,
                            text: service.productName,
                            price: service.price,
                            quantity: service.quantity
                        })),
                        templateResult: formatService,
                        templateSelection: formatService
                    });

                    $select.val(null).trigger('change');
                } else {
                    toastError(response.message);
                }
            },
            function (xhr, status, error) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }

    function formatService(service) {
        if (!service.id) {
            return service.text;
        }
        return $(`
        <div class="d-flex justify-content-between align-items-center w-100">
            <span class="service-name">${service.text}</span>
            <span class="service-quantity" data-service-text="${service.text}" data-service-quantity="${service.quantity}">Còn: ${service.quantity}</span>
        </div>
    `);
    }


    // valdate cho form thêm sản phẩm
    function validateForm() {
        let isValid = true;

        isValid = validateField($('#productId'), {
            required: true
        }) && isValid;

        isValid = validateField($('input[name="price"]'), {
            required: true,
            money: true,
            min: 1000
        }) && isValid;

        isValid = validateField($('input[name="quantity"]'), {
            required: true,
            number: true,
            min: 1
        }) && isValid;

        return isValid;
    }

    // validate cho form cập nhật thông tin sản phẩm
    function validateEditForm() {
        let isValid = true;

        isValid = validateField($('#editPrice'), {
            required: true,
            number: true,
            min: 0
        }) && isValid;

        isValid = validateField($('#editQuantity'), {
            required: true,
            number: true,
            min: 1
        }) && isValid;

        return isValid;
    }

    // Thêm sự kiện cho việc validate khi người dùng rời khỏi trường nhập liệu
    $('#addProductForm input, #addProductForm select').on('blur', function () {
        validateField($(this), {
            required: true,
            number: $(this).attr('name') === 'price' || $(this).attr('name') === 'quantity',
            min: $(this).attr('name') === 'price' ? 0 : 1
        });
    });
    // Thêm sự kiện cho việc validate khi người dùng rời khỏi trường nhập liệu trong form chỉnh sửa
    $('#editProductForm input').on('blur', function () {
        validateField($(this), {
            required: true,
            number: $(this).attr('name') === 'editPrice' || $(this).attr('name') === 'editQuantity',
            min: $(this).attr('name') === 'editPrice' ? 0 : 1
        });
    });

    // Xử lý sự kiện nhấn nút thanh toán
    $('#payment').on('click', function () {
        if (validatePaymentForm()) {
            var inventoryImportData = {
                partnerName: $('#partnerName').val(),
                inventoryDetailsExports: productList.map(function (product) {
                    return {
                        quantity: product.quantity,
                        price: product.price,
                        productId: product.id,
                        productName: product.name
                    };
                })
            };
            LoadingOverlay.show();

            var url = '/InventoryExport/Create';
            submitForm(url, JSON.stringify(inventoryImportData),
                function (response) {
                    LoadingOverlay.hide();

                    if (response.statusCode == 200) {
                        toastSuccess('Đơn nhập hàng đã được tạo thành công');
                        // Chuyển hướng đến trang danh sách hoặc chi tiết đơn nhập hàng
                        window.location.href = '/InventoryExport/Index';
                    } else {
                        toastError('Có lỗi xảy ra: ' + response.message);
                    }
                }, function (xhr, status, error) {
                    toastError('Có lỗi xảy ra khi gửi yêu cầu');
                })
        }
    });

    function validatePaymentForm() {
        let isValid = true;

        // Kiểm tra xem có sản phẩm nào trong danh sách không
        if (productList.length === 0) {
            toastError('Vui lòng thêm ít nhất một sản phẩm vào đơn hàng');
            isValid = false;
        }

        // Kiểm tra tên đối tác
        if ($('#partnerName').val().trim() === '') {
            toastError('Vui lòng nhập tên của bên nhận');
            isValid = false;
        }

        // Có thể thêm các kiểm tra khác ở đây (ví dụ: ngày nhập, ghi chú, ...)

        return isValid;
    }

});







