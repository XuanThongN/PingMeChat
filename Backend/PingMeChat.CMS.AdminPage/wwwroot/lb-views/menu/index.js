$(document).ready(function () {

    // lấy danh sách menu - quyền - sử dụng ajax
    function getAllMenuPermisstion() {
        let url = "/menu/getall";
        LoadingOverlay.show();
        let requestData = {
            Title: $("#keyword").val(),
            Status: ($("#selectStatus").val() === "1" || $("#selectStatus").val() === "") ? true : false,
        
        }
        callAPI(url, "POST", JSON.stringify(requestData),
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode !== 200) {
                    toastError(response.message);
                    return;
                }
                renderTable(response.result);
               //let  menuDatas = response.result;
               // let tableContent = '';
               // menuDatas.forEach(item => {
               //     tableContent += renderMenuItem(item);
               // });

               // $('#menuTable').html(tableContent);
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }

    // thực thi
    getAllMenuPermisstion();

    function renderTable(menuDatas) {
        let tableContent = '';
        if (!menuDatas || menuDatas.length === 0) {
            tableContent = `
            <tr>
                <td colspan="8" class="text-center">Không có dữ liệu</td>
            </tr>
        `;
        } else {
            menuDatas.forEach(item => {
                tableContent += renderMenuItem(item);
            });
        }
        $('#menuTable').html(tableContent);
    }
   // hiện thị danh sách menu - quyền sau khi gọi api ra table
    function renderMenuItem(item, level = 0) {
        let hasChildren = item.children && item.children.length > 0;
        let toggleIcon = hasChildren ?
            `<i class="fas fa-plus-square toggle-icon" data-id="${item.id}"></i>` :
            '<span class="toggle-icon"></span>';

        let html = `
                    <tr class="${level > 0 ? 'child-row' : ''}" data-level="${level}" data-parent="${item.parentId || ''}">
                        <td><input type="checkbox"></td>
                        <td>
                            <div class="title-container">
                                ${'&nbsp;'.repeat(level * 4)}
                                ${toggleIcon}
                                ${level > 0 ? '' : ''}
                                <span>${item.title}</span>
                            </div>
                        </td>
                        <td>${item.icon ? `<i class="${item.icon}"></i>` : ''}</td>
                        <td>${item.url || ''}</td>
                        <td>
                            ${item.menuType ?
                '<span class="menu-icon blue"></span>Quyền' :
                '<span class="menu-icon green"></span>Menu'}
                        </td>
                        <td>${item.sortOrder}</td>
                        <td>
                            ${item.isActive ?
                '<i class="fas fa-check-circle text-success status-icon"></i> Sử dụng' :
                '<i class="fas fa-times-circle text-danger status-icon"></i> Không sử dụng'}
                        </td>
                        <td>
                            <i class="fas fa-edit text-primary action-icon"></i>
                            <i class="fas fa-trash-alt text-danger action-icon"></i>
                        </td>
                    </tr>
                `;

        if (hasChildren) {
            item.children.forEach(child => {
                html += renderMenuItem(child, level + 1);
            });
        }

        return html;
    }

    $('#selectAll').change(function () {
        $('tbody input[type="checkbox"]').prop('checked', this.checked);
    });

    // tìm kiếm
    $(document).on('submit', '#form-filter', function (e) {
        e.preventDefault();
        getAllMenuPermisstion();
    });

    // Xử lý sự kiện ẩn/hiện menu con
    $('#menuTable').on('click', '.toggle-icon', function () {
        let $icon = $(this);
        let itemId = $icon.data('id');
        let $childRows = $('tr[data-parent="' + itemId + '"]');

        if ($icon.hasClass('fa-plus-square')) {
            $icon.removeClass('fa-plus-square').addClass('fa-minus-square');
            $childRows.show();
        } else {
            $icon.removeClass('fa-minus-square').addClass('fa-plus-square');
            $childRows.hide();
            // Ẩn tất cả các menu con cấp dưới
            hideAllChildren($childRows);
        }
    });

    function hideAllChildren($rows) {
        $rows.each(function () {
            let $row = $(this);
            let $icon = $row.find('.toggle-icon');
            if ($icon.hasClass('fa-minus-square')) {
                $icon.removeClass('fa-minus-square').addClass('fa-plus-square');
                let childId = $icon.data('id');
                let $childRows = $('tr[data-parent="' + childId + '"]');
                $childRows.hide();
                hideAllChildren($childRows);
            }
        });
    }
    
    // phục vụ phần create
    // Khai báo một mảng để lưu trữ các mục đã chọn trong checkbox
    let selectedItems = [];
    // Xử lý xử kiện khi modal create hiện thị
    $('#CreateModal').on('show.bs.modal', function (event) {
        getMenuTree();
        getMenuTreeCheckbox();

        $('.select2').select2({
            dropdownParent: $('#CreateModal'),
            width: '100%'
        });
    });

    // lấy danh sách quyền - menu
    function getMenuTree() {
        let url = "/menu/getAllTree";
        LoadingOverlay.show();
        GetData(url,
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode !== 200) {
                    toastError(response.message);
                    return;
                } 

                let menuDatas = response.result;
                var selectOptions = '<option value="">Thuộc quyền</option>' + buildHierarchy(menuDatas);
                $('#hierarchicalSelect').html(selectOptions);
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }

    // lấy danh sách quyền - menu - checkbox
    function getMenuTreeCheckbox() {
        let url = "/menu/getControllerByCondition";
        LoadingOverlay.show();
        GetData(url,
            function (response) {
                LoadingOverlay.hide();
                if (response.statusCode !== 200) {
                    toastError(response.message);
                    return;
                }

                let menuTreeDatas = response.result;
                // Tạo cây checkbox và chèn vào DOM
                const treeHtml = createCheckboxTree(menuTreeDatas);
                $('#checkboxTree').html(treeHtml);
            },
            function (xhr, status, error) {
                LoadingOverlay.hide();
                toastError("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        );
    }

    // Xử lý sự kiện submit form tạo mới menu
    $(document).on('submit', '#CreateForm', function (e) {
        e.preventDefault();
       
        if (validateForm()) {
            let url = "/menu/add"
            let type = "POST"
            let data = $(this).serializeArray().reduce(function (obj, item) {
                if (item.name === "SortOrder") {
                    obj[item.name] = parseInt(item.value);
                } 
                else {
                    obj[item.name] = item.value;
                }
                return obj;
            }, {});

            // Thêm Access vào data
            data.Access = selectedItems.length > 0 ? JSON.stringify(selectedItems) : "";

            // Kiểm tra và thêm menuType vào data
            let menuType = !$('#mainCheckbox').is(':checked');
            data.MenuType = menuType;

            // kiểm tra và thêm isActived vào data
            let isActived = $('#IsActive').is(':checked');
            data.IsActive = isActived;

            // kiểm tra và set lại giá trị cho parrentId - nếu nó là chuỗi rỗng thì set là null
            let parentId = $('#hierarchicalSelect').val();
            data.ParentId = parentId === "" ? null : parentId;

            LoadingOverlay.show();
            callAPI(url, type, JSON.stringify(data), (res) => {
                LoadingOverlay.hide(); // Ẩn loading

                //ẩn modal và ẩn luôn modal-backdrop
                $("#CreateModal").modal('hide')
/*                $("#CreateBidaTableTypeModal .btn-close").click()*/
                table.ajax.reload()
                resetFormAfterSubmit();

                // Reset selectedItems sau khi submit thành công
                selectedItems = [];

            }, (xhr, status, error) => {
                LoadingOverlay.hide(); // Ẩn loading
                console.error("Error:", error);
            })
        }
    });

    // cách thức hiện dữ liệu dạng tree ở phần select 2.
    function buildHierarchy(items, depth = 0) {
        var options = '';
        items.forEach(function (item) {
            var padding = '\u00A0'.repeat(depth * 4);
            options += '<option value="' + item.id + '">' + padding + item.title + '</option>';

            if (item.children && item.children.length > 0) {
                options += buildHierarchy(item.children, depth + 1);
            }
        });
        return options;
    }
    // Initialize Select2
    $('#hierarchicalSelect').select2({
        placeholder: "Thuộc quyền",
        allowClear: true
    });


    // Hàm tạo cấu trúc HTML cho cây checkbox
    function createCheckboxTree(data) {
        let html = '<ul>';
        data.forEach(controller => {
            html += `
            <li>
                <i class="fas fa-plus-square toggle-icon"></i>
                <label>
                    <input type="checkbox" class="controller-checkbox" data-id="${controller.id}">
                    ${controller.name}
                </label>
                <ul class="hidden">
        `;
            controller.actions.forEach(action => {
                html += `
                <li>
                    <label>
                        <input type="checkbox" class="action-checkbox" data-id="${action.id}">
                        ${action.name}
                    </label>
                </li>
            `;
            });
            html += '</ul></li>';
        });
        html += '</ul>';
        return html;
    }

    // Sử dụng event delegation cho các sự kiện
    $(document).ready(function () {
        // Xử lý sự kiện khi checkbox chính thay đổi
        $('#mainCheckbox').change(function () {
            $('#checkboxTree').toggleClass('hidden', !this.checked);
        });

        // Xử lý sự kiện khi click vào biểu tượng mở rộng/thu gọn
        $(document).on('click', '.toggle-icon', function () {
            //$(this).text($(this).text() === '▼' ? '►' : '▼');
            $(this).siblings('ul').toggleClass('hidden');
        });

        // Xử lý sự kiện khi checkbox của controller thay đổi
        $(document).on('change', '.controller-checkbox', function () {
            const isChecked = this.checked;
            const controllerId = $(this).data('id');
            const controllerName = $(this).closest('label').text().trim();

            // Cập nhật trạng thái của tất cả checkbox con
            $(this).closest('li').find('.action-checkbox').prop('checked', isChecked);

            // Thêm hoặc xóa controller
            updateSelectedItems({ id: controllerId, name: controllerName }, isChecked);

            // Thêm hoặc xóa tất cả action của controller
            $(this).closest('li').find('.action-checkbox').each(function () {
                const actionId = $(this).data('id');
                const actionName = $(this).closest('label').text().trim();
                updateSelectedItems({ id: actionId, name: actionName, controllerId: controllerId }, isChecked);
            });
        });

        // Xử lý sự kiện khi checkbox của action thay đổi
        $(document).on('change', '.action-checkbox', function () {
            const isChecked = this.checked;
            const actionId = $(this).data('id');
            const actionName = $(this).closest('label').text().trim();
            const controllerId = $(this).closest('ul').siblings('label').find('.controller-checkbox').data('id');

            updateSelectedItems({ id: actionId, name: actionName, controllerId: controllerId }, isChecked);

            // Cập nhật trạng thái của checkbox cha (controller)
            const $controllerCheckbox = $(this).closest('ul').siblings('label').find('.controller-checkbox');
            const allChecked = $(this).closest('ul').find('.action-checkbox:checked').length === $(this).closest('ul').find('.action-checkbox').length;
            $controllerCheckbox.prop('checked', allChecked);

            // Nếu tất cả action được chọn, thêm controller vào danh sách. Ngược lại, xóa nó.
            if (allChecked) {
                const controllerName = $controllerCheckbox.closest('label').text().trim();
                updateSelectedItems({ id: controllerId, name: controllerName }, true);
            } else {
                updateSelectedItems({ id: controllerId }, false);
            }
        });
    });


    // Hàm để thêm hoặc xóa một mục khỏi mảng selectedItems
    function updateSelectedItems(item, isChecked) {
        if (isChecked) {
            // Thêm mục nếu nó chưa có trong mảng
            if (!selectedItems.some(i => i.id === item.id)) {
                selectedItems.push(item);
            }
        } else {
            // Xóa mục khỏi mảng
            selectedItems = selectedItems.filter(i => i.id !== item.id);
        }
    } 

    // Tạo hàm để validate form
    function validateForm() {
        let isValid = true;

        // Validate mã bàn
        isValid = validateField($('#Title'), {
            required: true,
            minLength: 3,
            maxLength: 50
        }) && isValid;

        // Validate mã bàn
        isValid = validateField($('#Icon'), {
            required: true,
            minLength: 3,
            maxLength: 50
        }) && isValid;

        return isValid;
    }

    function resetFormAfterSubmit() {
        $('#CreateForm')[0].reset();
        $('.error-message').remove();
    }
});