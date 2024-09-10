var currentTableId;
var currentTableCode;
var customerId;
var model = {};
var services = [];
var servicesSelected = [];
var discountType = 0;
var discountNumber = 0;

function updateTableId(id, code) {
    currentTableId = id;
    currentTableCode = code;
    $('#hidd-id').text(currentTableId);
    $('#hidd-code').text(currentTableCode);
    //calculateEstimatePrice();
}

function getAllServiceInTableSession() {
    var url = "/BidaTable/GetAllServiceSession";
    var jsonData = JSON.stringify({ BidaTableId: currentTableId });

    submitForm(url, jsonData,
        function (response) {
            if (response.statusCode === 200) {
                var data = response.result;
                //$('#totalPrice').text(formatCurrency(data.totalPrice));
                //$('#price').text(formatCurrency(data.price) + '/giờ');
                //$('#totalTime').text(data.totalTimeString);
                //$('#totalAmount').text(formatCurrency(data.totalPrice + data.totalServicePrice));
                // Đưa dữ liệu vào biến model để sử dụng ở các hàm khác
                data.sessionServiceDtos.forEach(e => {
                    if (e.productId != null && e.productId !== '') {
                        servicesSelected.push({
                            productId: e.productId,
                            productName: e.productName,
                            quantity: e.quantity,
                            price: e.price,
                            maxQuantity: e.quantity // Số lượng tối đa dịch vụ đã chọn
                        });
                    }
                });
                updateServiceList(data.sessionServiceDtos);
            } else {
                toastError(response.message);
            }
        },
        function (xhr, status, error) {
            toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
        }
    );
}

function updateServiceList(sessionServices) {
    const $serviceDetailsTable = $('#serviceDetails');
    const $serviceTotalElement = $('#totalServicePrice');
    const $selectedServices = $('#selectedServices');
    const $codeNameTable = $("#code-name-table");

    $serviceDetailsTable.empty();
    $selectedServices.empty();
    let serviceTotal = 0;

    if (sessionServices && sessionServices.length > 0) {
        sessionServices.forEach(service => {
            const rowTotal = service.quantity * service.price;
            serviceTotal += rowTotal;

            $serviceDetailsTable.append(`
                <tr>
                    <td>${service.productName}</td>
                    <td>${service.quantity} x ${service.price.toLocaleString('vi-VN')} = ${rowTotal.toLocaleString('vi-VN')}</td>
                </tr>
            `);

            $selectedServices.append(`
                <tr data-code="${service.productId}" data-product-id="${service.productId}" data-product-name="${service.productName}">
                    <td>${service.productName}</td>
                    <td><input type="number" class="form-control form-control-sm quantity-input" value="${service.quantity}" min="1"></td>
                    <td>${service.price.toLocaleString('vi-VN')}</td>
                    <td class="row-total">${rowTotal.toLocaleString('vi-VN')}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-danger remove-service">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `);
        });

        $('#service-session').removeClass('d-none');
    } else {
        $('#service-session').addClass('d-none');
    }

    $serviceTotalElement.text(formatCurrency(serviceTotal));
    $codeNameTable.text(currentTableCode);

    updateModalTotal();
}

function updateModalTotal() {
    const total = servicesSelected.reduce((sum, service) => sum + service.quantity * service.price, 0);
    $('#modalTotalAmount').text(formatCurrency(total));
}

// Hàm render chi tiết phiên chơi của bàn hiện tại
function renderDetailTablePlaying() {
    var url = "/BidaTable/GetDetailTablePlaying";
    var jsonData = JSON.stringify({ BidaTableId: currentTableId });
    submitForm(url, jsonData,
        function (response) {
            $('#featureModal').find('.modal-dialog').html(response);
        },
        function (xhr, status, error) {
            toastError(`Đã xảy ra lỗi: ${xhr.statusText}. Vui lòng thử lại.`);
        }
    );
}

function addServiceSessionInSession(servicesSelected) {
    const serviceModal = $('#serviceModal');
    var url = "/BidaTable/AddServiceSessionInSession";
    var jsonData = JSON.stringify({
        BidaTableId: currentTableId,
        SessionServiceCreateDtos: [...new Map(servicesSelected.map(item => [item.productId, item])).values()] // Loại bỏ các dịch vụ trùng lặp
    });

    submitForm(url, jsonData,
        function (response) {
            if (response.statusCode === 200) {
                toastSuccess(response.message);
                serviceModal.modal('hide'); // Ẩn modal
                servicesSelected = []; // Xoá dữ liệu đã chọn

                // Cập nhật lại dữ liệu trên giao diện
                renderDetailTablePlaying();
                return;
            }
            toastError(response.message);
        },
        function (xhr, status, error) {
            toastError(xhr.responseJSON.Result);
        }
    );
}

function getEmptyTables() {
    var url = "/BidaTable/GetEmptyTables";

    GetData(url, function (response) {
        if (response.statusCode === 200) {
            let data = response.result;
            if (data.length > 0) {
                // Nếu có dữ liệu
                var html = response.result.map(item => `<option value="${item.id}">${item.bidaTableTypeName} - ${item.code}</option>`).join('');
                $('#listTableActiveId').html(html);
                return
            }
            $('#confirmMoveTableModal').modal('hide')
            toastError("Không còn bàn trống để chuyển tới.");

        } else {
            toastError(response.message);
        }
    }, function (xhr, status, error) {
        toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
    });
}

function moveBillView(orderId, orderParrentId) {
    let url = '/Order/BillView?';

    if (orderId != null && orderId !== undefined) {
        url += `OrderId=${encodeURIComponent(orderId)}`;
    }

    if (orderParrentId != null && orderParrentId !== undefined) {
        url += url.endsWith('?') ? '' : '&';
        url += `OrderParrentId=${encodeURIComponent(orderParrentId)}`;
    }

    window.open(url, '_blank');
    window.location.reload();
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

$(document).ready(function () {
    const serviceModal = new bootstrap.Modal(document.getElementById('serviceModal'));

    $('#confirmPlayBtn').on('click', function () {
        var currentTableId = $('#hidd-id').text();
        var url = "/BidaTable/StartPlay";
        var jsonData = JSON.stringify({ bidaTableId: currentTableId });

        LoadingOverlay.show();
        submitForm(url, jsonData,
            function (response) {
                LoadingOverlay.hide();
                toastSuccess("Bàn " + currentTableCode + " đã được chọn. Chúc bạn chơi vui vẻ!");
                $('#confirmPlayModal').modal('hide');
                location.reload();
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    });

    $('#confirmMoveTableBtn').on('click', function () {
        var url = "/BidaTable/ChangeTable";

        if ($('#listTableActiveId').val() === null) {
            toastError("Vui lòng chọn bàn để chuyển tới.");
            return;
        }
        var jsonData = JSON.stringify({ BidaTableId: currentTableId, BidaTableChangeId: $('#listTableActiveId').val() });

        LoadingOverlay.show();

        submitForm(url, jsonData,
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode === 200) {
                    toastSuccess("Chuyển bàn thành công, chúc quý khách vui vẻ!");
                    $('#confirmMoveTableModal').modal('hide');
                    location.reload();
                } else {
                    toastError(response.message);
                }
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    });

    $('#confirmSpiltHourBtn').on('click', function () {
        var url = "/BidaTable/SplitHour";
        var jsonData = JSON.stringify({ BidaTableId: currentTableId });

        LoadingOverlay.show();

        submitForm(url, jsonData,
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode === 200) {
                    toastSuccess("Chuyển bàn thành công, chúc quý khách vui vẻ!");
                    $('#confirmMoveTableModal .btn-close').click();
                    $('#featureModal .btn-close').click();
                    location.reload();
                } else {
                    toastError(response.message);
                }
            },
            function (xhr, status, error) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    });

    $('#confirmPaymentBtn').on('click', function () {
        var currentTableId = $('#hidd-id').text();
        var currentTableCode = $('#hidd-code').text();
        var paymentMethod = parseInt($("#paymentMethodId").val()) || 0;
        var discountType = parseInt($('#confirmPaymentModal input[name="discountType"]').val()) || 0;
        var discountNumber = parseInt($('#confirmPaymentModal input[name="discountNumber"]').val()) || 0; // Lưu chung giá trị giảm giá dưới dạng số (không phân biệt % hay số tiền)
        var url = "/BidaTable/Payment";
        var data = {
            bidaTableId: currentTableId,
            Description: "",
            PaymentMethod: paymentMethod,
            DiscountType: discountType, // 0: giảm theo %, 1: giảm theo số tiền
            DiscountNumber: discountType != 0 ? discountNumber : 0, // Nếu giảm theo số tiền thì giá trị giảm giá sẽ là 0
            DiscountPercent: discountType == 0 ? discountNumber : 0 // Nếu giảm theo % thì giá trị giảm giá sẽ là 0
        };
        var jsonData = JSON.stringify(data);
        LoadingOverlay.show();
        submitForm(url, jsonData,
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode !== 200) {
                    toastError(response.message);
                    return;
                }
                toastSuccess("Thanh toán thành công cho bàn " + currentTableCode + ". Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!");
                //clear form
                $('#confirmPlayModal input').val('')
                $('#confirmPlayModal').modal('hide');
                moveBillView(response.result.orderId, response.result.orderParrentId);
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    });

    $('#confirmPaymentModal').on('show.bs.modal', function (event) {
        var currentTableCode = $('#hidd-code').text();
        $(this).find('.tableCodeForPayment').text(currentTableCode);
        $('.select2').select2({
            dropdownParent: $('#confirmPaymentModal'),
            width: '100%'
        });
    });

    $('#confirmMoveTableModal').on('show.bs.modal', function (event) {
        var currentTableCode = $('#hidd-code').text();
        $(this).find('.moveTableModal').text(currentTableCode);
        $('.select2').select2({
            dropdownParent: $('#confirmMoveTableModal'),
            width: '100%'
        });
        getEmptyTables();
    });

    $('#confirmSpiltHourModal, #confirmDebtModal').on('show.bs.modal', function (event) {
        var currentTableCode = $('#hidd-code').text();
        $(this).find('.moveTableModal').text(currentTableCode);
    });

    $('#confirmDebtModal').on('show.bs.modal', function () {
        $('#infoCustomer, #searchResults').hide();
    });

    $('#showNewCustomerForm').on('click', function (e) {
        e.preventDefault();
        clearCustomerForm();
        $('#infoCustomer').slideDown();
        $('#searchResults').hide();
    });

    $('#searchCustomerBtn').on('click', function () {
        var keyword = $('#customerKeywordSearch').val().trim();
        if (!keyword) return;
        searchCustomers(keyword);
    });

    $('#confirmDebtBtn').on('click', function () {
        var url = "/bidaTable/Debt";
        if (!validateForm()) return;
        var fullName = $('#fullName').val();
        var phoneNumber = $('#phoneNumber').val();
        var address = $('#address').val();
        var discountType = parseInt($('#confirmPaymentModal input[name="discountType"]').val()) || 0;
        var discountNumber = parseInt($('#confirmPaymentModal input[name="discountNumber"]').val()) || 0; // Lưu chung giá trị giảm giá dưới dạng số (không phân biệt % hay số tiền)

        var debtData = {
            CustomerId: customerId,
            Customer: null,
            OrderPay: {
                BidaTableId: currentTableId,
                Description: '',
                PaymentMethod: 0,
                DiscountType: discountType, // 0: giảm theo %, 1: giảm theo số tiền
                DiscountNumber: discountType != 0 ? discountNumber : 0, // Nếu giảm theo số tiền thì giá trị giảm giá sẽ là 0
                DiscountPercent: discountType == 0 ? discountNumber : 0, // Nếu giảm theo % thì giá trị giảm giá sẽ là 0
                OrderParrentId: null
            }
        };

        if (!customerId) {
            debtData.Customer = {
                FullName: fullName,
                PhoneNumber: phoneNumber,
                Address: address
            };
        }

        LoadingOverlay.show();
        submitForm(url, JSON.stringify(debtData),
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode !== 200) {
                    toastError(response.message);
                    return;
                }
                toastSuccess("Ghi nợ thành công");
                $('#confirmDebtModal').modal('hide');
                location.reload();
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    });

    function validateForm() {
        let isValid = true;

        isValid = validateField($('#infoCustomer #fullName'), {
            required: true,
            min: 3,
            max: 225,
        }) && isValid;

        isValid = validateField($('#infoCustomer #phoneNumber'), {
            required: true,
            phone: true
        }) && isValid;

        isValid = validateField($('#infoCustomer #address'), {
            required: true,
            min: 3,
            max: 225
        }) && isValid;

        return isValid;
    }

    function searchCustomers(keyword) {
        var url = "/customer/search";
        var jsonData = JSON.stringify({ keyword: keyword });
        LoadingOverlay.show();
        submitForm(url, jsonData,
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode !== 200) {
                    toastError(response.message);
                    return;
                }
                displaySearchResults(response.result);
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }

    function displaySearchResults(results) {
        var $customerList = $('#customerList');
        $customerList.empty();

        if (results.length > 0) {
            results.forEach(function (customer) {
                $customerList.append(
                    `<li class="list-group-item" data-id="${customer.id}">
                        ${customer.fullName} - ${customer.phoneNumber}
                    </li>`
                );
            });
            $('#searchResults').show();
        } else {
            $('#searchResults').hide();
        }
    }

    function clearCustomerForm() {
        $('#fullName, #phoneNumber, #address').val('');
        customerId = null;
    }

    $('#customerList').on('click', 'li', function () {
        customerId = $(this).data('id');
        var customerDetail = {
            id: customerId,
            fullName: $(this).text().split('-')[0].trim(),
            phoneNumber: $(this).text().split('-')[1].trim(),
            address: "Địa chỉ mẫu"
        };
        displayCustomerInfo(customerDetail);
    });

    function displayCustomerInfo(customer) {
        $('#fullName').val(customer.fullName);
        $('#phoneNumber').val(customer.phoneNumber);
        $('#address').val(customer.address);
        $('#infoCustomer').show();
        $('#searchResults').hide();
    }

    function updatePlayTime() {
        const now = new Date();
        $('.card[data-table-id]').each(function () {
            const $card = $(this);
            const $startTimeStr = $card.find('.play-time[data-start-time]').attr('data-start-time');
            if ($startTimeStr?.length) {
                const startTime = new Date($startTimeStr);

                if (!isNaN(startTime.getTime())) {
                    const diff = now - startTime;
                    const hours = Math.floor(diff / (1000 * 60 * 60));
                    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));

                    $card.find('.hours').text(hours.toString().padStart(2, '0'));
                    $card.find('.minutes').text(minutes.toString().padStart(2, '0'));
                }
            }
        });
        requestAnimationFrame(updatePlayTime);
    }

    updatePlayTime();

    function renderServiceList() {
        var url = "/Inventory/GetAll";
        GetData(url,
            function (response) {
                if (response.statusCode === 200) {
                    services = response.result;
                    const serviceListHtml = services.map(service =>
                        renderAservice(service) // Gọi hàm renderAservice để render từng dịch vụ
                    ).join('');
                    $('#serviceList').html(serviceListHtml);
                } else {
                    toastError(response.message);
                }
            },
            function (xhr, status, error) {
                toastError(`Đã xảy ra lỗi: ${xhr.statusText}. Vui lòng thử lại.`);
            }
        );
    }

    // Hàm render một dịch vụ
    function renderAservice(service) {
        return `<a href="#" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center p-3 border-bottom" 
                           data-price="${service.price}" data-product-id="${service.productId}" data-code="${service.productId}">
                            <div class="d-flex flex-column">
                                <h5 class="mb-1 text-primary">${service.productName}</h5>
                            </div>
                            <div class="d-flex flex-column align-items-end">
                                <span class="badge bg-success rounded-pill mb-2">${service.priceFormat}</span>
                                <span class="text-muted small">Tồn kho: <strong>${service.quantity}</strong></span>
                            </div>
                        </a>
                        `
    }
    function addService(service) {
        const $existingRow = $(`#selectedServices tr[data-code="${service.productId}"]`);
        const currentStock = service.quantity;

        if ($existingRow.length) {
            const $quantityInput = $existingRow.find('.quantity-input');
            let newQuantity = parseInt($quantityInput.val()) + 1;
            // Cập nhật số lượng dịch vụ
            let existService = servicesSelected.find(s => s.productId == service.productId);
            existService.quantity = newQuantity;
            $quantityInput.val(newQuantity).change(); // Gọi sự kiện change để cập nhật tổng tiền
        } else {
            const $newRow = $(`
                <tr data-code="${service.productId}" data-product-id="${service.productId}" data-product-name="${service.productName}">
                    <td>${service.productName}</td>
                    <td><input type="number" class="form-control form-control-sm quantity-input" value="1" min="1"></td>
                    <td>${service.price.toLocaleString('vi-VN')}</td>
                    <td class="row-total">${service.price.toLocaleString('vi-VN')}</td>
                    <td> <button class="btn btn-sm btn-outline-danger remove-service">
                    <i class="bi bi-trash"></i>
                    </button>
                    </td>
                </tr>
            `);
            $('#selectedServices').append($newRow); // Thêm dịch vụ vào UI
            servicesSelected.push(
                {
                    productId: service.productId,
                    productName: service.productName,
                    quantity: 1,
                    price: service.price
                }); // Thêm dịch vụ vào biến lưu trữ serviceSelected
        }
        updateModalTotal();
    }
    // Xử lý sự kiện thay đổi số lượng dịch vụ
    $(document).on('change', '#selectedServices .quantity-input', function () {
        updateRowTotal($(this).closest('tr'));
    });

    function updateRowTotal($row) {
        const rowData = $row.data();
        const service = services.find(s => s.productId === rowData.code);
        const selectedService = servicesSelected.find(s => s.productId === rowData.code);
        if (!service || !selectedService) {
            toastError("Dịch vụ không tồn tại hoặc không thể tìm thấy thông tin sản phẩm");
            return;
        }
        const quantityInput = $row.find('.quantity-input');
        let quantity = parseInt(quantityInput.val());
        if (quantity < 1) {
            quantity = 1;
        } 
        quantityInput.val(quantity); // Cập nhật số lượng trong input mà không kích hoạt sự kiện change
        selectedService.quantity = quantity; // Cập nhật số lượng dịch vụ trong biến lưu trữ
        const price = parseInt($row.find('td:nth-child(3)').text().replace(/\D/g, ''));
        const total = quantity * price;
        $row.find('.row-total').text(total.toLocaleString('vi-VN'));
        updateModalTotal();
    }


    $(document).on('click', '#openServiceModal', function () {
        renderServiceList();
        getAllServiceInTableSession();
        serviceModal.show();
    });

    // Clear hết dữ liệu service và servicesSelected khi đóng modal
    $('#serviceModal').on('hidden.bs.modal', function () {
        services = [];
        servicesSelected = [];
        $('#selectedServices').empty();
    });

    $(document).on('click', '#serviceList .list-group-item', function (e) {
        e.preventDefault();
        const serviceId = $(this).data('code');
        const service = services.find(s => s.productId === serviceId);
        if (service) {
            addService(service);
        }
    });

    $('#selectedServices').on('click', '.remove-service', function () {
        const row = $(this).closest('tr');
        const productId = row.data('code');
        let service = servicesSelected.find(s => s.productId === productId); // Tìm dịch vụ trong danh sách dịch vụ đã chọn
        if (service) {
            servicesSelected = servicesSelected.filter(s => s.productId !== productId); // Xoá dịch vụ khỏi danh sách dịch vụ đã chọn
        }
        row.remove(); // Xoá dòng dịch vụ khỏi UI
        updateModalTotal();
    });

    $('#searchService').on('input', function () {
        const searchTerm = $(this).val().toLowerCase();
        const filteredServices = services.filter(service =>
            service.productName.toLowerCase().includes(searchTerm) || service.productId.toLowerCase().includes(searchTerm)
        );
        const serviceListHtml = filteredServices.map(service =>
            renderAservice(service)
        ).join('');
        $('#serviceList').html(serviceListHtml);
    });

    $(document).on('click', '#updateServices', function () {
        addServiceSessionInSession(servicesSelected);
    });

    $('#featureModal').on('shown.bs.modal', function () { // Hiển thị chi tiết về phiên chơi của bàn hiện tại và các thông tin liên quan ở modal Feature
        renderDetailTablePlaying();
    });

    $('#featureModal').on('hidden.bs.modal', function () {
        $(this).find('.modal-dialog').empty()   // Xoá nội dung modal khi đóng
    });

    // Bắt sự kiện ẩn/hiện nội dung trong các lịch sử hoá đơn
    $(document).on('click', '#order-history .list-group-item', function () {
        $(this).find('.details').toggle();
    });

    // Bắt sự kiện nhập giá trị giảm giá
    $(document).on('input', '#discountValue, #discountType', function () {
        let $discountValue = $('#discountValue');
        let $discountType = $('#discountType');
        let discountNumber = parseFloat($discountValue.val()) || 0;
        let totalPrice = parseFloat($('#totalAmount').data('value')) || 0;
        let discountType = parseInt($discountType.val()) || 0;
        let totalAmount = totalPrice;

        if (discountNumber < 0) {
            discountNumber = 0;
            $discountValue.val(discountNumber).change();
            toastError("Giảm giá không thể là số âm");
        }

        if (discountType === 0) { // Nếu là giảm theo %
            discountNumber = Math.min(discountNumber, 100);
            if (discountNumber === 100) {
                $discountValue.val(discountNumber).change();
                toastError("Giảm giá không thể lớn hơn 100%");
            }
            totalAmount = totalPrice * (1 - discountNumber / 100);
        } else { // Nếu là giảm theo số tiền
            discountNumber = Math.min(discountNumber, totalPrice);
            if (discountNumber === totalPrice) {
                $discountValue.val(discountNumber).change();
                toastError("Số tiền giảm giá không thể lớn hơn tổng giá trị đơn hàng");
            }
            totalAmount = totalPrice - discountNumber;
        }

        // Gán giá trị giảm giá vào 2 input trong modal confirm payment để dành cho việc thanh toán
        $('#confirmPaymentModal input[name="discountType"]').val(discountType);
        $('#confirmPaymentModal input[name="discountNumber"]').val(discountNumber);
        $('#totalAmount').text(formatCurrency(totalAmount));
    });


    // Clear mọi giá trị giảm giá khi đóng 2 modal confirm payment và confirm debt
    $('#confirmPaymentModal').on('hidden.bs.modal', function () {
        $(this).find('input').val('');
    });
    $('#confirmDebtModal').on('hidden.bs.modal', function () {
        $('#confirmPaymentModal').find('input').val('');
    });


});
