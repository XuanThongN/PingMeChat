
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
        { targets: 1, data: "id", visible: false },
        { targets: 2, data: "code", sortable: false },
        { targets: 3, data: "createdBy", sortable: false, },
        {
            targets: 4, data: "createdDate",
            sortable: false,
            render: function (data, type, row, meta) {
                return fomartDateTimeServer(data)
            }
        },
        { targets: 5, data: "partnerName", sortable: false, },
        {
            targets: 6, data: "totalAmount", sortable: false,
            render: function (data, type, row, meta) {
                return formatCurrency(data)
            }
        },
        {
            targets: 7, data: "inventoryType", sortable: false, className: 'text-center', render: function (data, type, row, meta) {
                if (data == 0) {
                    return `<div class="d-flex justify-content-center">
                         <button class="badge badge-warning text-center text-light changeStatus" style="border: none;" data-bs-toggle="modal" data-bs-target="#changeStatusModal" data-id="${row.id}">
                        <i class="far fa-check-circle mx-1"></i>  ${row.inventoryTypeName}
                        </button>
                    </div> `;

                } else if (data == 1) {
                    return `<div class="d-flex justify-content-center">
                         <button class="badge badge-success text-center text-light changeStatus" style="border: none;"  data-bs-toggle="modal" data-bs-target="#changeStatusModal" data-id="${row.id}" >
                          <i class="fas fa-check-circle mx-1"></i>   ${row.inventoryTypeName}
                        </button>
                    </div> `;
                }

                else {
                    return `<div class="d-flex justify-content-center">
                         <button class="badge badge-danger text-center text-light changeStatus" style="border: none;"  data-bs-toggle="modal" data-bs-target="#changeStatusModal" data-id="${row.id}" >
                         <i class="fas fa-window-close mx-1"></i>  ${row.inventoryTypeName}
                        </button>
                    </div> `;
                }
            }
        },

        {
            targets: 8,
            data: null,
            sortable: false,
            className: 'text-center',
            defaultContent: '',
            render: (data, type, row, meta) => {
                return `
                    <div class="text-center">
                       <a  title="Xem chi tiết" class="btn btn-outline-info btn-sm btn-view-modal" data-view-id="${row.id}" data-bs-toggle="modal" data-bs-target="#ViewDetailInventoryExportModal"><i class="fas fa-eye mx-1"></i></a>
                        `
                //< a title = "Lịch sử" class="btn btn-outline-success btn-sm  me-1" href = "/OrderHistory/Index?OrderId=${row.id}" > <i class="fas fa-history mx-1"></i></a >
                //   <a  title="Hủy" class="btn btn-outline-danger btn-sm btn-remove-modal" ><i class="fas fa-window-close mx-1"></i></a>
                //</div>
            }
        }
    ];

    const table = DataTableUtils.init('#inventoryExport-table', '/inventoryExport/Pagination', columns, '#form-filter');

    $(document).on('click', '.btn-reset', function () {
        DataTableUtils.resetFilters('#form-filter', table);
    });


    // Xử lý sự kiện click nút xem
    $(document).on('click', '.btn-view-modal', function () {
        var id = $(this).data('view-id');
        let url = `/InventoryExport/Detail/${id}`
        let type = "GET"
        let data = { id: id }
        callAPI(url, type, data, (res) => {
            $("#ViewDetailInventoryExportModal .modal-dialog").html(res)

        }, () => {
            console.log("Error");
        })

    });






});


