
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
                data: 'cost',
                title: 'Giá nhập',
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

    // filter
    $('#form-filter').on('submit', function (e) {
        e.preventDefault();
        // lấy giá trị trong form
        var form = $(this).serializeArray();
        var data = {};
        $.each(form, function (i, v) {
            data["" + v.name + ""] = v.value;
        });
        // Chuyển đổi dữ liệu thành query parameter
        var queryString = $.param(data);

        // Chuyển hướng trang với query parameter và cập nhật datatable
        var searchUrl = '/User/Search?' + queryString;
        updateDataTable(searchUrl, columns, tableId, reqData, dataSrc)
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
            var cost = MoneyUtils.getRawValue($('input[name="cost"]'));
            var quantity = $('input[name="quantity"]').val();

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
                    cost: parseFloat(cost),
                    total: parseFloat(cost) * parseInt(quantity),
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

        // Sử dụng setMoneyValue để đặt giá trị cho trường cost
        MoneyUtils.setMoneyValue($('#editCost')[0], data.cost);

        $('#editQuantity').val(data.quantity);

        $('#editProductModal').modal('show');
    });

    // Xử lý cập nhật sản phẩm
    $('#updateProduct').on('click', function () {
        if (validateEditForm()) {
            var productId = $('#editProductId').val();
            var cost = MoneyUtils.getRawValue($('#editCost')[0]);
            var quantity = $('#editQuantity').val();

            var updatedProduct = {
                id: productId,
                name: $('#editProductName').val(),
                quantity: parseInt(quantity),
                cost: parseFloat(cost),
                total: parseFloat(cost) * parseInt(quantity),
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
        MoneyUtils.initMoneyInput($('#editCost')[0]);
    });
    function updateOrderSummary() {
        var totalAmount = productList.reduce((sum, product) => sum + product.total, 0);
        $('.order-summary th:contains("Tổng tiền thanh toán")').next().text(totalAmount.toFixed(2));
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
        var url = "/Product/GetAll";

        GetData(url,
            function (response) {
                if (response.statusCode === 200) {
                    var services = response.result;

                    $select.empty().append('<option value="" selected disabled>Chọn sản phẩm</option>');
                    services.forEach(service => {
                        $select.append(`<option value="${service.id}" data-price="${service.price}">${service.name}</option>`);
                    });

                    $select.trigger('change');
                } else {
                    toastError(response.message);
                }
            },
            function (xhr, status, error) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }

    // valdate cho form thêm sản phẩm
    function validateForm() {
        let isValid = true;

        isValid = validateField($('#productId'), {
            required: true
        }) && isValid;

        isValid = validateField($('input[name="cost"]'), {
            required: true,
            number: true,
            min: 0
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

        isValid = validateField($('#editCost'), {
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
            number: $(this).attr('name') === 'cost' || $(this).attr('name') === 'quantity',
            min: $(this).attr('name') === 'cost' ? 0 : 1
        });
    });
    // Thêm sự kiện cho việc validate khi người dùng rời khỏi trường nhập liệu trong form chỉnh sửa
    $('#editProductForm input').on('blur', function () {
        validateField($(this), {
            required: true,
            number: $(this).attr('name') === 'editCost' || $(this).attr('name') === 'editQuantity',
            min: $(this).attr('name') === 'editCost' ? 0 : 1
        });
    });

    // Xử lý sự kiện nhấn nút thanh toán
    $('#payment').on('click', function () {
        if (validatePaymentForm()) {
            var inventoryImportData = {
                partnerName: $('#partnerName').val(),
                inventoryDetailsImports: productList.map(function (product) {
                    return {
                        quantity: product.quantity,
                        cost: product.cost,
                        productId: product.id,
                        ProductName:  product.name
                    };
                })
            };
            LoadingOverlay.show();

            var url = '/InventoryImport/Create';
            submitForm(url, JSON.stringify(inventoryImportData),
                function (response) {
                    LoadingOverlay.hide();

                    if (response.statusCode == 200) {
                        toastSuccess('Đơn nhập hàng đã được tạo thành công');
                        // Chuyển hướng đến trang danh sách hoặc chi tiết đơn nhập hàng
                        window.location.href = '/Inventory/Index';
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
            toastError('Vui lòng nhập tên đối tác');
            isValid = false;
        }

        // Có thể thêm các kiểm tra khác ở đây (ví dụ: ngày nhập, ghi chú, ...)

        return isValid;
    }

});







