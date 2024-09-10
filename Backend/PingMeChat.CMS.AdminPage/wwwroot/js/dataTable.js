
function createDataTable(dataUrl, columns, tableId, reqData, dataSrc) {
    $('#' + tableId).DataTable({
        "lengthChange": false,
        "responsive": true,
        "searching": false,
        "processing": true,
        "autoWidth": false,
        "serverSide": true,
        //  "scrollY": "35vh",
        "ajax": {
            "url": dataUrl,
            "data": reqData,
            "dataSrc": dataSrc,  // Trường "data" trong phản hồi JSON chứa dữ liệu của bảng DataTable
            "type": "GET",
        },
        language: {
            "sProcessing": "Đang xử lý...",
            "sLengthMenu": "Xem _MENU_ mục",
            "sZeroRecords": "Không tìm thấy dòng nào phù hợp",
            "sInfo": "Đang xem từ _START_ đến _END_ trong tổng số _TOTAL_ mục",
            "sInfoEmpty": "Đang xem 0 đến 0 trong tổng số 0 mục",
            "sInfoFiltered": "(được lọc từ _MAX_ mục)",
            "sInfoPostFix": "",
            "sSearch": "Tìm kiếm:",
            "sUrl": "",
            "oPaginate": {
                "sFirst": "Đầu",
                "sPrevious": "Trước",
                "sNext": "Tiếp",
                "sLast": "Cuối"
            }
        },
        "columns": columns,
    });
}

function reloadDataTable(tableId) {
    var table = $('#' + tableId).DataTable();
    table.ajax.reload();
}

function updateDataTable(url, columns, tableId, reqData, dataSrc) {
    //var dataTable = $('#' + tableId).DataTable(); // Lấy đối tượng DataTable
    //dataTable.destroy();
    createDataTable(url, columns, tableId, reqData, dataSrc); // Vẽ lại DataTable với dữ liệu mới
}


//DataTable v2
const DataTableUtils = {
    init: function (tableId, url, columns, filterFormId) {
        const table = $(tableId).DataTable({
            processing: true,
            serverSide: true,
            ajax: {
                url: url,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    const filterData = DataTableUtils.getFilterData(filterFormId);
                    const sendData = {
                        draw: d.draw,
                        start: d.start,
                        length: d.length,
                        ...filterData
                    };
                    return JSON.stringify(sendData);
                },
                dataFilter: function (data) {
                    var json = JSON.parse(data);
                    json.recordsTotal = json.result.recordsTotal;
                    json.recordsFiltered = json.result.recordsFiltered;
                    json.data = json.result.data;
                    return JSON.stringify(json);
                }
            },
            columns: columns,
            searching: false,
            ordering: false,
            lengthChange: false,
            pageLength: 10,
            language: {
                "sProcessing": "Đang xử lý...",
                "sLengthMenu": "Xem _MENU_ mục",
                "sZeroRecords": "Không tìm thấy dòng nào phù hợp",
                "sInfo": "Đang xem từ _START_ đến _END_ trong tổng số _TOTAL_ mục",
                "sInfoEmpty": "Đang xem 0 đến 0 trong tổng số 0 mục",
                "sInfoFiltered": "(được lọc từ _MAX_ mục)",
                "sInfoPostFix": "",
                "sSearch": "Tìm kiếm:",
                "sUrl": "",
                "oPaginate": {
                    "sFirst": "Đầu",
                    "sPrevious": "Trước",
                    "sNext": "Tiếp",
                    "sLast": "Cuối"
                }
            }
        });

        DataTableUtils.bindFilterEvents(table, filterFormId);
        return table;
    },

    getFilterData: function (filterFormId) {
        const form = $(filterFormId);
        return form.convertFormToFormData() || {};
    },

    bindFilterEvents: function (table, filterFormId) {
        const form = $(filterFormId);

        form.on('submit', function (e) {
            e.preventDefault();
            table.ajax.reload();
        });

        form.find('select, input[type="date"], input[type="datetime-local"]').on('change', function () {
            table.ajax.reload();
        });

        //form.find('input[type="text"]').on('keyup', function () {
        //    setTimeout(function () {
        //        table.ajax.reload();
        //    }, 1000);
        //});
    },

    resetFilters: function (filterFormId, table) {
        const form = $(filterFormId);
        form[0].reset();
        form.find('.text-danger')?.text('');
        table.ajax.reload();
    }
};

//DataTable v3
const DataTableUtilsV3 = {
    init: function (tableId, url, columns, dataRequest) {
        const table = $(tableId).DataTable({
            processing: true,
            serverSide: true,
            ajax: {
                url: url,
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    const sendData = {
                        draw: d.draw,
                        start: d.start,
                        length: d.length,
                        ...dataRequest() // gọi hàm datarequest
                    };
                    return JSON.stringify(sendData);
                },
                dataFilter: function (data) {
                    var json = JSON.parse(data);
                    json.recordsTotal = json.result.recordsTotal;
                    json.recordsFiltered = json.result.recordsFiltered;
                    json.data = json.result.data;
                    return JSON.stringify(json);
                }
            },
            columns: columns,
            searching: false,
            ordering: false,
            lengthChange: false,
            pageLength: 10,
            language: {
                "sProcessing": "Đang xử lý...",
                "sLengthMenu": "Xem _MENU_ mục",
                "sZeroRecords": "Không tìm thấy dòng nào phù hợp",
                "sInfo": "Đang xem từ _START_ đến _END_ trong tổng số _TOTAL_ mục",
                "sInfoEmpty": "Đang xem 0 đến 0 trong tổng số 0 mục",
                "sInfoFiltered": "(được lọc từ _MAX_ mục)",
                "sInfoPostFix": "",
                "sSearch": "Tìm kiếm:",
                "sUrl": "",
                "oPaginate": {
                    "sFirst": "Đầu",
                    "sPrevious": "Trước",
                    "sNext": "Tiếp",
                    "sLast": "Cuối"
                }
            }
        });

      //  DataTableUtils.bindFilterEvents(table);
        return table;
    },

    //bindFilterEvents: function (table) {
    //    const form = $(filterFormId);

    //    form.on('submit', function (e) {
    //        e.preventDefault();
    //        table.ajax.reload();
    //    });

    //    form.find('select, input[type="date"]').on('change', function () {
    //        table.ajax.reload();
    //    });

   
    //},

    resetFilters: function (filterFormId, table) {
        const form = $(filterFormId);
        form[0].reset();
        form.find('.text-danger')?.text('');
        table.ajax.reload();
    }
};
