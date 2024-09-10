var orderId;
$(function () {

    // gán giá trị cho orderId;
    orderId = $('#orderId').val();

    var columns = [
        {
            targets: 0,
            data: null,
            sortable: false,
            searchable: false,
            render: function (data, type, row, meta) {
                return (meta.row + meta.settings._iDisplayStart + 1);
            }
        },
        //{
        //    targets: 1,
        //    data: "id",
        //    visible: false
        //},
        {
            targets: 1,
            title: 'Phiên bản',
            data: null,
            render: function (data, type, row, meta) {
                return "Version " + (meta.settings.json.recordsTotal - meta.row);
            }
        },
        {
            targets: 2,
            data: "code",
            className: "text-center",
            sortable: false
        },
        {
            targets: 3,
            data: 'orderDate',
            sortable: false,
            className: 'text-center',
            render: function (data, type, row) {
                return (data)
                    ? moment(data).format("DD/MM/YYYY HH:mm:ss")
                    : null;
            }
        },
        { targets: 4, data: "staffName", sortable: false, },
        {
            targets: 5,
            data: "totalAmountAllOrder",
            sortable: false,
            render: numberFormatCurrency()
        },
        {
            targets: 6,
            data: null, // Không chỉ định một cột cụ thể
            sortable: false,
            className: 'text-center',
            render: function (data, type, row, meta) {
                // Kiểm tra trường giảm giá % trước
                if (row.discountPercent && row.discountPercent !== '') {
                    return `- ${row.discountPercent}%`;
                }
                // Nếu (giảm giá %) trống, kiểm tra (giảm giá tiền)
                else if (row.discount && row.discount !== '') {
                    return `- ${numberFormatCurrency()(row.discount)}`;;
                }
                // Nếu cả hai đều trống
                else {
                    return '-'; // Trả về dấu gạch ngang'
                }
            }
        },
        {
            targets: 7,
            data: "customer",
            sortable: false,
            render: function (data, type, row) {
                return data && data.fullName ? data.fullName : '-';
            }
        },
        {
            targets: 8,
            data: "customer",
            sortable: false,
            render: function (data, type, row) {
                return data && data.phoneNumber ? data.phoneNumber : '-';
            }
        },

        {
            targets: 9, data: "status", className: "text-center", sortable: false, render: function (data, type, row, meta) {
                if (data == null) {
                    return '-'
                }
                //Set màu cho trạng thái
                if (data == 0) { // hoàn thành
                    return `<span class="badge badge-success">Hoàn thành</span>`;
                } else if (data == 1) { // hủy
                    return ` <div class="feature-item">
                        <button class="badge badge-danger change-status-btn" style="border: none;" data-bs-toggle="modal" data-bs-target="#changeStatusModal" data-code="${row.code}" data-id="${row.id}">Đã huỷ</button>
                    </div>`;
                }
                else // Đang nợ
                    return `<span class="badge badge-warning">Đang nợ</span>`;
            }
        },
        {
            targets: 10,
            data: null,
            sortable: false,
            autoWidth: true,
            width: "160px",
            defaultContent: '',
            render: (data, type, row, meta) => {
                if (row.status == 0) { // trạng thái hoàn thành
                    return `<div class="d-flex justify-content-center">
                               <a title="Xem chi tiết" class="btn btn-outline-info btn-sm btn-view-modal mx-1" href="/Order/BillView?OrderId=${row.orderId}&OrderParrentId=${row.parrentId ?? ''}" target="_blank" data-view-id="${row.id}" data-toggle="modal"><i class="fas fa-eye mx-1"></i></a>
                            </div>
                            `
                } else if (row.status == 1) { // hủy
                    return `
                        <div class="d-flex justify-content-center">
                            <a title="Xem chi tiết" class="btn btn-outline-info btn-sm btn-view-modal mx-1" href="/Order/BillView?OrderId=${row.orderId}&OrderParrentId=${row.parrentId ?? ''}" target="_blank" data-view-id="${row.id}" data-toggle="modal"><i class="fas fa-eye mx-1"></i></a>
                             <a title="Xem chi tiết lý do hủy" class="btn btn-outline-danger btn-sm btn-cancel-modal" data-cancel-id="${row.id}" data-bs-toggle="modal" data-bs-target="#cancelOrderModal"><i class="fas fa-info-circle mx-1"></i></a>

                        </div>
                         `
                } else { // Đang nợ
                    return `
                        <div class="d-flex justify-content-center">
                              <a title="xem chi tiết" class="btn btn-outline-info btn-sm btn-view-modal mx-1" href="/Order/BillView?OrderId=${row.orderId}&OrderParrentId=${row.parrentId ?? ''}" target="_blank" data-view-id="${row.id}" data-toggle="modal"><i class="fas fa-eye mx-1"></i></a>
                        </div>
                         `
                }
            }
        }
    ];
    const table = DataTableUtils.init('#OrderHistory-table', '/OrderHistory/Pagination', columns, '#form-filter');
    $(document).on('click', '.btn-reset', function () {
        DataTableUtils.resetFilters('#form-filter', table);
    });

    // Xử lý sự kiện click nút sửa
    $(document).on('click', '.btn-edit-modal', function () {
        var id = $(this).data('edit-id');
        // Thêm logic để mở modal sửa và load dữ liệu
    });

    // Xử lý sự kiện click nút xóa
    $(document).on('click', '.btn-delete-modal', function () {
        var id = $(this).data('delete-id');
        // Thêm logic để xác nhận và xóa bản ghi
    });

    // Xử lý sự kiện click nút xem
    $(document).on('click', '.btn-view-modal', function () {
        var id = $(this).data('view-id');
        // Thêm logic để mở modal xem chi tiết
    });


    $(document).on('click', '.btn-cancel-modal', function () {
        var orderId = $(this).data('cancel-id');
        fillCancelOrderModal(orderId);
    });

    function fillCancelOrderModal(orderId) {
        let url = '/OrderHistory/GetById'
        submitForm(url, JSON.stringify(orderId),
            function (response) {
                if (response.statusCode === 200 && response.result) {
                    var orderHis = response.result;

                    // Fill dữ liệu vào modal
                    $('#cancelOrderModal #orderCode').text(orderHis.code);
                    $('#cancelDescription').val(orderHis.cancelContent);
                    $('#createdBy').val(orderHis.createdBy);
                    $('#createdDate').val(moment(orderHis.createdDate).format("DD/MM/YYYY HH:mm:ss"));
                    $('#updatedBy').val(orderHis.updatedBy);
                    $('#updatedDate').val(moment(orderHis.updatedDate).format("DD/MM/YYYY HH:mm:ss"));

                    // Mở modal
                    $('#cancelOrderModal').modal('show');
                } else {
                    console.error('Error fetching order data');
                }
            }, function (xhr, status, error) {
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            })

    }
});