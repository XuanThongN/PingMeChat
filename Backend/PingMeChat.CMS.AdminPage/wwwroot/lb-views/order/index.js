var currentOrderId;
var orderParrentId;

$(function () {

    var columns = [
        {
            targets: 0,
            data: null,
            sortable: false,
            searchable: false,
            render: function (data, type, row, meta) {
                return meta.row + meta.settings._iDisplayStart + 1;
            }
        },
        {
            targets: 1,
            data: "id",
            visible: false
        },
        {
            targets: 2,
            data: "code",
            className: "text-center",
            sortable: false
        },
        {
            targets: 3,
            data: 'orderDateString',
            sortable: false,
            className: 'text-center',
            //render: function (data, type, row) {
            //    return (data)
            //        ? moment(data).format("DD/MM/YYYY HH:mm")
            //        : null;
            //}
        },
        { targets: 4, data: "staffName", sortable: false, },
        { targets: 5, data: "totalAmountAllOrder", className: "text-end", sortable: false, render: numberFormatCurrency(), },
        {
            targets: 6,
            data: "customer",
            sortable: false,
            render: function (data, type, row) {
                return data && data.fullName ? data.fullName : '-';
            }
        },
        {
            targets: 7,
            data: "customer",
            sortable: false,
            render: function (data, type, row) {
                return data && data.phoneNumber ? data.phoneNumber : '-';
            }
        },

        {
            targets: 8, data: "statusName", className: "text-center", sortable: false, render: function (data, type, row, meta) {
                if (data == null) {
                    return '-'
                }
                //Set màu cho trạng thái
                if (row.status == 0) { // hoàn thành
                    return `<span class="badge badge-success">${data}</span>`;
                } else if (row.status == 1) { // đã hủy
                    return ` <div class="feature-item">
                        <button class="badge badge-danger change-status-btn changeInfomation" style="border: none;" data-bs-toggle="modal" data-bs-target="#changeInfoModal" data-code="${row.code}"  data-parrentid="${row.parrentId}" data-id="${row.id}"  >${data}</button>
                    </div>`;
                }
                else if (row.status == 2) // đang nợ
                    return ` <div class="feature-item">
                        <button class="badge badge-warning change-status-btn" style="border: none;" data-bs-toggle="modal" data-bs-target="#changeStatusModal" data-id="${row.id}">${data}</button>
                    </div>`;
                else //chờ xác nhận
                    return ` <div class="feature-item">
                        <span class="badge badge-info" style="border: none;">${data}</span>
                    </div>`;
            }
        },
        {
            targets: 9,
            data: null,
            sortable: false,
            className: 'text-center',
            autoWidth: true,
            defaultContent: '',
            render: (data, type, row, meta) => {
                if (row.status == 0) { // trạng thái hoàn thành
                    return `<div class="d-flex justify-content-center">
                               <a title="Xem chi tiết" class="btn btn-outline-info btn-sm btn-view-modal mx-1" href="/Order/BillView?OrderId=${row.id}&OrderParrentId=${row.parrentId ?? ''}" target="_blank" data-view-id="${row.id}" data-toggle="modal"><i class="fas fa-eye mx-1"></i></a>
                                <a title="Lịch sử" class="btn btn-outline-success btn-sm  me-1" href="/OrderHistory/Index?OrderId=${row.id}"><i class="fas fa-history mx-1"></i></a>
                               <a title="Hủy" class="btn btn-outline-danger btn-sm btn-cancel-modal" data-delete-id="${row.id}" data-toggle="modal" data-target="#cancelOrderModal"><i class="fas fa-ban mx-1"></i></a>
                            
                            </div>
                            `
                } else if (row.status == 1) { // hủy
                    return `
                        <div class="d-flex justify-content-center">
                            <a title="Xem chi tiết" class="btn btn-outline-info btn-sm btn-view-modal mx-1" href="/Order/BillView?OrderId=${row.id}&OrderParrentId=${row.parrentId ?? ''}" target="_blank" data-view-id="${row.id}" data-toggle="modal"><i class="fas fa-eye mx-1"></i></a>
                            <a title="Lịch sử" class="btn btn-outline-success btn-sm  me-1" href="/OrderHistory/Index?OrderId=${row.id}"><i class="fas fa-history mx-1"></i></a>
                        </div>
                         `
                } else { // Đang nợ
                    return `
                        <div class="d-flex justify-content-center">
                              <a title="xem chi tiết" class="btn btn-outline-info btn-sm btn-view-modal mx-1" href="/Order/BillView?OrderId=${row.id}&OrderParrentId=${row.parrentId ?? ''}" target="_blank" data-view-id="${row.id}" data-toggle="modal"><i class="fas fa-eye mx-1"></i></a>
                                <a title="Lịch sử" class="btn btn-outline-success btn-sm  me-1" href="/OrderHistory/Index?OrderId=${row.id}"><i class="fas fa-history mx-1"></i></a>
                               <a title="Hủy" class="btn btn-outline-danger btn-sm btn-cancel-modal" data-delete-id="${row.id}" data-toggle="modal" data-target="#cancelOrderModal"><i class="fas fa-ban mx-1"></i></a>
                            
                        </div>
                         `
                }
            }
        }
    ];
    const table = DataTableUtils.init('#order-table', '/Order/Pagination', columns, '#form-filter');

    $(document).on('click', '.btn-reset', function () {
        DataTableUtils.resetFilters('#form-filter', table);
    });

    // Xử lý sự kiện khi click vào nút xóa
    $(document).on('click', '.btn-cancel-modal', function () {
        const orderId = $(this).data('delete-id');
        $('#cancelOrderModal').modal('show');
        $('#cancelOrderModal').find('#orderId').val(orderId);
    });

    // Tạo hàm để validate form
    function validateForm() {
        let isValid = true;

        // Validate mã bàn
        isValid = validateField($('#cancelDescription'), {
            required: true,
            minLength: 3,
            maxLength: 225
        }) && isValid;

        return isValid;
    }

    // Xử lý sự kiện khi click vào nút xác nhận hủy bill
    $('#cancelOrderModal').on('click', '.save-button ', function () {
        if (!validateForm()) return;
        const url = '/Order/CancelOrder';
        const OrderId = $('#orderId').val();
        const CancelContent = $('#cancelDescription').val();

        const jsonData = JSON.stringify({ OrderId, CancelContent });
        LoadingOverlay.show();
        if (validateForm()) {
            submitForm(url, jsonData, function (response) {
                LoadingOverlay.hide();
                if (response.statusCode === 200) {
                    toastSuccess(response.message); // Hiển thị thông báo thành công
                    $('#cancelOrderModal').modal('hide'); // Ẩn modal
                    table.ajax.reload(); // Load lại dữ liệu
                }
            }, function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            });
        }

    });

    //Khi modal #cancelOrderModal được ẩn đi thì reset form
    $('#cancelOrderModal').on('hidden.bs.modal', function () {
        $('#cancelOrderModal').find('form')[0].reset();
    });

    //$(document).on('click', '.change-status-btn', function () {
    //    var code = $(this).data('code');
    //    $('#orderCodeConfirm').text(code);
    //});
    $(document).on('click', '.change-status-btn', function () {
        var code = $(this).data('code');
        currentOrderId = $(this).data('id');
        $('#orderCodeConfirm').text(code);
    });
    // Xử lý sự kiện khi nhấn nút xác nhận thay đổi trạng thái
    $('#changeStatusBtn').on('click', function () {
        var url = "/order/changedStatus";  // Sử dụng đường dẫn tương đối
        var jsonData = JSON.stringify({ Id: currentOrderId, Status: 0 });
        LoadingOverlay.show();
        submitForm(url, jsonData,
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode !== 200) {
                    toastError(response.message);
                    return;
                }
                toastSuccess(response.message);
                $('#changeStatusModal .btn-close').click(); // Đóng modal
                table.ajax.reload();
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    });


    // phục vụ modal
    $(document).on('click', '.changeInfomation', function () {
        currentOrderId = $(this).data('id');
        orderParrentId = $(this).data('parrentid');

        loadOrderData(currentOrderId, orderParrentId);
        $('#changeInfoModal').modal('show');
    });

    $('#changeInfoModal').on('show.bs.modal', function (event) {

        $('.select2').select2({
            dropdownParent: $('#changeInfoModal'),
            width: '100%'
        });
    });
    function loadOrderData(orderId, orderParrentId) {
        var url = '/order/getbyid'

        var jsonData = JSON.stringify({ OrderId: orderId, OrderParrentId: orderParrentId });

        LoadingOverlay.show();

        submitForm(url, jsonData, function (response) {
            LoadingOverlay.hide();

            let orders = response.result;
            if (response.statusCode != 200) {
                toastError(response.message);
                return;
            }
            fillOrderForm(orders);
        }, function (xhr, status, error) {
            LoadingOverlay.hide();
            toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
        });
    }
    function fillOrderForm(orders) {
        $('#orderTabs').empty();
        $('#orderTabContent').empty();
        let grandTotal = 0;

        orders.forEach((order, index) => {
            // Tạo tab
            let tabHtml = `
            <li class="nav-item" role="presentation">
                <button class="nav-link ${index === 0 ? 'active' : ''}" id="order-tab-${index}" data-bs-toggle="tab" data-bs-target="#order-${index}" type="button" role="tab" aria-controls="order-${index}" aria-selected="${index === 0 ? 'true' : 'false'}">
                    Hóa đơn ${index + 1}
                </button>
            </li>
        `;
            $('#orderTabs').append(tabHtml);

            // Tạo nội dung tab
            let orderHtml = `
            <div class="tab-pane fade ${index === 0 ? 'show active' : ''}" id="order-${index}" role="tabpanel" aria-labelledby="order-tab-${index}">
                <div class="order-section mb-4" data-order-id="${order.id}">
                    <div class="row mb-3">
                        <div class="col-md-4">
                            <label class="form-label">Mã đơn hàng:</label>
                            <input type="text" class="form-control" value="${order.code}" readonly>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Mã bàn:</label>
                            <input type="text" class="form-control" value="${order.bidaTableCode}" readonly>
                        </div>
                       
                        <div class="col-md-4">
                            <label class="form-label">Nhân viên:</label>
                            <input type="text" class="form-control" value="${order.staffName}" readonly>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-md-6">
                            <label class="form-label">Giờ bắt đầu:</label>
                            <input type="text" class="form-control" value="${formatDateTime(order.startTime, 'datetime-short')}" readonly>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Giờ kết thúc:</label>
                            <input type="text" class="form-control" value="${formatDateTime(order.endTime, 'datetime-short')}" readonly>
                        </div>
                      
                    </div>

                   <table class="table table-bordered order-details-table">
                        <thead>
                            <tr>
                                <th class="col-4">Mặt hàng</th>
                                <th class="text-center">SL</th>
                                <th>Giá</th>
                                <th>Thành tiền</th>
                                <th>Hành động</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${order.orderDetails.map((detail, detailIndex) => `
                                <tr data-product-id="${detail.productId}" data-product-isnew="false" data-product-name="${detail.productName}">
                                    <td>${detail.productName}</td>
                                    <td>
                                        <div class="d-flex gap-2">
                                            <button class="btn btn-outline-secondary decrease-quantity" type="button">-</button>
                                            <input type="number" class="form-control product-quantity" value="${detail.quantity}" min="1" style="max-width: 70px;">
                                            <button class="btn btn-outline-secondary increase-quantity" type="button">+</button>
                                        </div>
                                    </td>
                                    <td class="product-price">${formatCurrency(parseInt(detail.unitPrice))}</td>
                                    <td class="subtotal">${formatCurrency(detail.subTotal)}</td>
                                    <td>
                                        <button type="button" class="btn btn-danger btn-sm remove-product">Xóa</button>
                                    </td>
                                </tr>
                            `).join('')}
                        </tbody>
                    </table>
                    <button type="button" class="btn btn-success btn-sm add-product" data-order-index="${index}">Thêm sản phẩm</button>
                </div>
            </div>
        `;
            $('#orderTabContent').append(orderHtml);

            grandTotal += order.totalAmount;
        });

        $('#grandTotal').text(grandTotal.toFixed(2));

        // Khởi tạo lại Select2 cho các select trong tab mới
        $('.select2').select2({
            dropdownParent: $('#changeInfoModal'),
            width: '100%'
        });
    }

    // Giảm số lượng sản phẩm
    $(document).on('click', '.decrease-quantity', function () {
        let input = $(this).siblings('input.product-quantity');
        input.val(function (i, oldval) {
            return Math.max(1, parseInt(oldval, 10) - 1);
        });
        updateProductSubtotal($(this).closest('tr'));
    });

    // Tăng số lượng sản phẩm
    $(document).on('click', '.increase-quantity', function () {
        let input = $(this).siblings('input.product-quantity');
        input.val(function (i, oldval) {
            return parseInt(oldval, 10) + 1;
        });
        updateProductSubtotal($(this).closest('tr'));
    });

    // Cập nhật khi thay đổi số lượng trực tiếp
    $(document).on('change', '.product-quantity', function () {
        updateProductSubtotal($(this).closest('tr'));
    });

    // Xóa sản phẩm
    $(document).on('click', '.remove-product', function () {
        $(this).closest('tr').remove();
        updateOrderTotal($(this).closest('.order-section'));
    });

    // Thêm sản phẩm mới
    $(document).on('click', '.add-product', function () {
        const orderIndex = $(this).closest('.tab-pane').index();
        const newRow = `
        <tr data-product-id="" data-product-isnew="true">
            <td> 
                 <select class="select2 product-select" style="width: 100%;">
                        <option value="" selected disabled>Chọn sản phẩm</option>
                 </select>
            </td>
            <td>
                <div class="d-flex gap-2">
                    <button class="btn btn-outline-secondary decrease-quantity" type="button">-</button>
                    <input type="number" class="form-control product-quantity" value="1" min="1" style="max-width: 70px;">
                    <button class="btn btn-outline-secondary increase-quantity" type="button">+</button>
                </div>
            </td>
            <td class="product-price">0.00</td>
            <td class="subtotal">0.00</td>
            <td>
                <button type="button" class="btn btn-danger btn-sm remove-product">Xóa</button>
            </td>
        </tr>
    `;
        $(this).closest('.order-section').find('.order-details-table tbody').append(newRow);

        // Khởi tạo Select2 cho sản phẩm mới thêm vào
        let $newSelect = $(this).closest('.order-section').find('.order-details-table tbody tr:last-child .select2');
        $newSelect.select2({
            dropdownParent: $('#changeInfoModal'),
            width: '100%'
        });

        let existProducts = getProductListInOrder(false); // Lấy danh sách sản phẩm đã có trong bảng
        // Gọi hàm để render danh sách sản phẩm
        renderServiceList($newSelect, existProducts);
    });

    //Lấy danh sách sản phẩm đã có (được thêm mới hoặc không) trong bảng chi tiết hoá đơn
    function getProductListInOrder(isNew) {
        let productList = [];
        $('.order-details-table tbody tr').each(function () {
            let $row = $(this);
            let productIsNew = $row.data('product-isnew') === true || $row.data('product-isnew') === 'true';
            let productId = $row.attr('data-product-id');

            // Chỉ thêm vào danh sách nếu có id và trạng thái isNew phù hợp
            if (productId && productIsNew === isNew) {
                productList.push(productId);
            }
        });

        return [...new Set(productList)]; // Lọc ra các sản phẩm trùng lặp
    }




    function updateOrderTotal(orderSection) {
        let total = 0;
        orderSection.find('.subtotal').each(function () {
            total += parseInt($(this).text());
        });
        orderSection.find('input[name$="totalServiceAmount"]').val(total.toFixed(2));

        const bidaTableAmount = parseInt(orderSection.find('input[name$="bidaTableAmount"]').val()) || 0;
        const discount = parseInt(orderSection.find('input[name$="discount"]').val()) || 0;
        const tax = parseInt(orderSection.find('input[name$="tax"]').val()) || 0;

        const orderTotal = total + bidaTableAmount - discount + tax;
        orderSection.find('input[name$="totalAmount"]').val(orderTotal.toFixed(2));

        updateGrandTotal();
    }

    function updateGrandTotal() {
        let grandTotal = 0;
        $('.order-section').each(function () {
            grandTotal += parseInt($(this).find('input[name$="totalAmount"]').val()) || 0;
        });
        $('#grandTotal').text(grandTotal.toFixed(2));
    }

    // Thêm sự kiện để khởi tạo lại Select2 khi chuyển tab
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $('.select2').select2({
            dropdownParent: $('#changeInfoModal'),
            width: '100%'
        });
    });

    $('#updateModalBtn').click(function () {
        const activeTabId = $('.tab-pane.active').attr('id');
        const activeOrderSection = $(`#${activeTabId} .order-section`);
        updateOrder(activeOrderSection);
    });

    $('#updateAndChangeStatusModalBtn').click(function () {
        const activeTabId = $('.tab-pane.active').attr('id');
        const activeOrderSection = $(`#${activeTabId} .order-section`);
        updateAndChangeStatus(activeOrderSection);
    });


    // Hàm để render danh sách sản phẩm
    function renderServiceList($select, existProducts) {
        var url = "/Product/GetAll";
        GetData(url,
            function (response) {
                if (response.statusCode === 200) {
                    services = response.result;
                    services = services?.filter(service => !existProducts.includes(service.id));
                    $select.empty().append('<option value="" selected disabled>Chọn sản phẩm</option>');
                    services.forEach(service => {
                        $select.append(`<option value="${service.id}" data-name="${service.name}" data-price="${service.price}">${service.name}</option>`);
                    });

                    // Cập nhật Select2 sau khi thêm options
                    //$select.trigger('change');
                } else {
                    toastError(response.message);
                }
            },
            function (xhr, status, error) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }

    // Sự kiện khi chọn sản phẩm
    $(document).on('change', '.product-select', function () {
        let $select = $(this);
        let $selectedOption = $select.find('option:selected');
        let selectedValue = $selectedOption.val();
        let $row = $select.closest('tr');
        let existingProducts = getProductListInOrder(true);

        if (existingProducts.includes(selectedValue)) {
            // Xử lý sản phẩm đã tồn tại
            let $existingRow = $row.siblings(`tr[data-product-id="${selectedValue}"]`);
            let $quantityElement = $existingRow.find('.product-quantity');
            let quantity = parseInt($quantityElement.val(), 10) || 0;
            $quantityElement.val(quantity + 1);
            $row.remove();
            $row = $existingRow; // Gán lại row
        } else {
            // Cập nhật sản phẩm mới
            let price = parseInt($selectedOption.data('price')) || 0;
            $row.find('td:eq(2)').text(formatCurrency(price.toFixed(2)));
            $row.attr('data-product-id', selectedValue);
            $row.find('td:eq(0)').text($selectedOption.data('name'));
        }

        updateProductSubtotal($row);
    });

    function updateProductSubtotal(row) {
        let quantity = parseInt(row.find('.product-quantity').val()) || 0;
        let price = parseInt(row.find('td:eq(2)').text().replace(/[.,]/g, '')) || 0; // Đổi từ input sang text
        let subtotal = quantity * price;
        row.find('.subtotal').text(formatCurrency(subtotal.toFixed(2)));
        updateOrderTotal(row.closest('.order-section'));
    }

    function updateOrderTotal(orderSection) {
        let total = 0;
        orderSection.find('.subtotal').each(function () {
            total += parseInt($(this).text());
        });
        orderSection.find('input[name$="totalServiceAmount"]').val(total.toFixed(2));

        const bidaTableAmount = parseInt(orderSection.find('input[name$="bidaTableAmount"]').val()) || 0;
        const discount = parseInt(orderSection.find('input[name$="discount"]').val()) || 0;
        const tax = parseInt(orderSection.find('input[name$="tax"]').val()) || 0;

        const orderTotal = total + bidaTableAmount - discount + tax;
        orderSection.find('input[name$="totalAmount"]').val(orderTotal.toFixed(2));

        updateGrandTotal();
    }

    function updateGrandTotal() {
        let grandTotal = 0;
        $('.order-section').each(function () {
            grandTotal += parseInt($(this).find('input[name$="totalAmount"]').val()) || 0;
        });
        $('#grandTotal').text(grandTotal.toFixed(2));
    }

    function updateOrder(orderSection) {
        const orderId = orderSection.data('order-id');
        const orderData = {
            Id: orderId,
            OrderParrentId: orderParrentId,
            OrderDetails: []
        };
        getOrderDetails(orderSection, orderData) // Lấy thông tin chi tiết đơn hàng
        submitForm('/order/UpdateServiceDetailsInBill', JSON.stringify(orderData),
            function (response) {
                $('#updateModal .btn-close').click(); // Đóng modal form cập nhật
                $('#changeInfoModal .btn-close').click() // Đóng modal confirm
                if (response.statusCode === 200) {
                    toastSuccess(response.message);
                    table.ajax.reload(); // load lại table
                    // Cập nhật lại dữ liệu trong tab
                    //updateTabData(orderSection, orderData);
                } else {
                    toastError('Lỗi: ' + response.message);
                }
            }, function (xhr, status, error) {
                if (xhr.responseJSON.statusCode == 500) {
                    toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                    return;
                }
                toastError(xhr.responseJSON.Result);
            })
    }
    function updateAndChangeStatus(orderSection) {
        const orderId = orderSection.data('order-id');
        const orderData = {
            Id: orderId,
            OrderParrentId: orderParrentId,
            OrderDetails: []
        };
        getOrderDetails(orderSection, orderData) // Lấy thông tin chi tiết đơn hàng
        submitForm('/order/UpdateServiceDetailsInBillAndChangeStatus', JSON.stringify(orderData),
            function (response) {
                if (response.statusCode === 200) {
                    $('#changeInfoModal .btn-close').click(); // Đóng modal form cập nhật
                    $('#updateAndChangeStatusModal .btn-close').click(); // Đóng modal confirm

                    toastSuccess(response.message);

                    // load lại table
                    table.ajax.reload();
                    return;
                }
                toastError('Lỗi: ' + response.message);
            }, function (xhr, status, error) {
                if (xhr.responseJSON.statusCode == 500) {
                    toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
                    return;
                }
                toastError(xhr.responseJSON.Result);
            })

    }
    function getOrderDetails(orderSection, orderData) {
        orderSection.find('.order-details-table tbody tr').each(function () {
            const $row = $(this);
            let detail;
            let isNew = $row.data('product-isnew'); // Kiểm tra xem sản phẩm có phải mới thêm vào không
            let productId = $row.attr('data-product-id');
            if (!productId) return;
            detail = {
                ProductId: productId,
                ProductName: isNew ? $row.find('td:eq(0)').text() : $row.data('product-name'),
                Quantity: parseInt($row.find('.product-quantity').val().replace(/[.,]/g, '')) || 0,
                UnitPrice: parseInt($row.find('.product-price').text().replace(/[.,]/g, '')) || 0,
                SubTotal: parseInt($row.find('.subtotal').text().replace(/[.,]/g, '')) || 0
            };

            orderData.OrderDetails.push(detail);
        });
    }
});

