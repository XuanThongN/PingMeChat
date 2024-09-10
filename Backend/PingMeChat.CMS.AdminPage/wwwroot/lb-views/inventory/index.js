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
        { targets: 1, data: "productName", sortable: false },
        { targets: 2, data: "quantity", sortable: false, },
        { targets: 3, data: "costFormat", sortable: false, },
        { targets: 4, data: "priceFormat", sortable: false, },
        {
            targets: 5,
            data: null,
            sortable: false,
            className: 'text-center',
            defaultContent: '',
            render: (data, type, row, meta) => {
                return `
                    <div class="text-center">
                       <a  title="Xem chi tiết" class="btn btn-outline-info btn-sm btn-view-modal" data-view-id="${row.id}" data-bs-toggle="modal" data-bs-target="#ViewDetailInventoryModal"><i class="fas fa-eye mx-1"></i></a>
                    </div>
               `

            }
        }
    ];


    const table = DataTableUtils.init('#inventory-table', '/inventory/Pagination', columns, '#form-filter');

    $(document).on('click', '.btn-reset', function () {
        DataTableUtils.resetFilters('#form-filter', table);
    });


    // Xử lý sự kiện click nút xem
    $(document).on('click', '.btn-view-modal', function () {
        var id = $(this).data('view-id');
        let url = `/Inventory/Detail/${id}`
        let type = "GET"
        let data = { id: id }
        callAPI(url, type, data, (res) => {
            $("#ViewDetailInventoryModal .modal-dialog").html(res)

        }, () => {
            console.log("Error");
        })

    });
});