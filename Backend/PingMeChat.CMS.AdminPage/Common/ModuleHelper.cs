using PingMeChat.CMS.AdminPage.Models;
using System;
using System.Collections.Generic;

namespace PingMeChat.CMS.AdminPage.Common
{
    /// <summary>
    /// This is where you customize the navigation sidebar
    /// </summary>
    public static class ModuleHelper
    {
        public enum Module
        {
            Home,
            User,
            Role,
            LogEvent,
            Student,
            Staff, // nhân viên
            StaffGroup, // nhoms nhân viên
            StaffAccount, // tài khoản nhân viên,
            StaffRole, // vai trò
            StaffTimeWorking, // thời gian làm việc
            Customer,
            CustomerAgency, // nhà  cung cấp
            CustomerAgencyType, // nhóm nhà  cung cấp
            Sale,
            Order,
            Voucher,
            Producttype,
            Product,
            Inventory, // quản lý kho
            InventoryImport, // quản lý phiếu nhập hàng
            InventoryExport, // quản lý phiếu xuất hàng
            Asset,
            Category, // đối tác
            Cashbook, // danh mục sổ quỹ
            Transactioncategory, // nhóm thu chi
            Transaction, //  sổ quỹ
            Moneyreport, // báo cáo thu chi
            Report, //báo cáo
            Account,// quản trị
            Branch, // chi nhánh
            HistoryAction, // lịch sử thao tác
            DeviceManeger, // quản lý thiết bị
            BidaTable,
            BidaTableType,

        }

        public static SidebarMenu AddHeader(string name)
        {
            return new SidebarMenu
            {
                Type = SidebarMenuType.Header,
                Name = name,
            };
        }

        public static SidebarMenu AddTree(string name, string iconClassName = "fa fa-link")
        {
            return new SidebarMenu
            {
                Type = SidebarMenuType.Tree,
                IsActive = false,
                Name = name,
                IconClassName = iconClassName,
                URLPath = "#",
            };
        }

        public static SidebarMenu AddModule(Module module, Tuple<int, int, int> counter = null)
        {
            if (counter == null)
                counter = Tuple.Create(0, 0, 0);

            switch (module)
            {
                case Module.Home:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Bảng điều khiển",
                        IconClassName = "fa fa-home",
                        URLPath = "/Home",
                        LinkCounter = counter,
                    };
                case Module.User:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Tài khoản",
                        IconClassName = "fa fa-users",
                        URLPath = "/User",
                        LinkCounter = counter,
                    };
                case Module.Role:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Phân quyền",
                        IconClassName = "fa fa-user-tag",
                        URLPath = "/Role",
                        LinkCounter = counter,
                    };
                case Module.Sale:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Bán hàng",
                        IconClassName = "fas fa-shopping-cart",
                        URLPath = "/sale",
                        LinkCounter = counter,
                    };
                case Module.Order:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Hóa đơn",
                        IconClassName = "fab fa-first-order-alt",
                        URLPath = "/order",
                        LinkCounter = counter,
                    };
                case Module.Voucher:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Mã voucher",
                        IconClassName = "fab fa-salesforce",
                        URLPath = "/voucher",
                        LinkCounter = counter,
                    };
                case Module.Category:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Đối tác",
                        IconClassName = "fas fa-people-arrows",
                        URLPath = "",
                        LinkCounter = counter,
                    };
                case Module.Customer:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Khách hàng",
                        IconClassName = "fas fa-user-friends",
                        URLPath = "/customer",
                        LinkCounter = counter,
                    };
                case Module.CustomerAgencyType:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Nhóm nhà cung cấp",
                        IconClassName = "fas fa-luggage-cart",
                        URLPath = "/customeragencytype",
                        LinkCounter = counter,
                    };
                case Module.Product:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Sản phẩm, dịch vụ",
                        IconClassName = "fab fa-product-hunt",
                        URLPath = "/product",
                        LinkCounter = counter,
                    };
                case Module.Producttype:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Nhóm sản phẩm, dịch vụ",
                        IconClassName = "fas fa-project-diagram",
                        URLPath = "/product",
                        LinkCounter = counter,
                    };
                case Module.Inventory:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Hàng tồn kho",
                        IconClassName = "fas fa-box",
                        URLPath = "/inventory",
                        LinkCounter = counter,
                    };
                case Module.InventoryImport:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Phiếu nhập hàng",
                        IconClassName = "fas fa-people-carry",
                        URLPath = "/inventoryImport",
                        LinkCounter = counter,
                    };
                case Module.InventoryExport:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Phiếu xuất hàng",
                        IconClassName = "fas fa-shipping-fast",
                        URLPath = "/inventoryExport",
                        LinkCounter = counter,
                    };
                case Module.Cashbook:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Danh mục sổ quỹ",
                        IconClassName = "fas fa-wallet",
                        URLPath = "/package",
                        LinkCounter = counter,
                    };
                case Module.Transactioncategory:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Nhóm thu chi",
                        IconClassName = "fas fa-receipt",
                        URLPath = "/package",
                        LinkCounter = counter,
                    };
                case Module.Transaction:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Sổ quỹ",
                        IconClassName = "fas fa-funnel-dollar",
                        URLPath = "/package",
                        LinkCounter = counter,
                    };
                case Module.Moneyreport:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Nhóm báo cáo",
                        IconClassName = "fas fa-chart-line",
                        URLPath = "/package",
                        LinkCounter = counter,
                    };
                case Module.CustomerAgency:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Nhà cung cấp",
                        IconClassName = "fas fa-truck-moving",
                        URLPath = "/customeragency",
                        LinkCounter = counter,
                    };
                case Module.Report:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Báo cáo",
                        IconClassName = "fas fa-chart-bar",
                        URLPath = "/report",
                        LinkCounter = counter,
                    };
                case Module.Staff:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Nhân viên",
                        IconClassName = "fas fa-chart-bar",
                        URLPath = "/package",
                        LinkCounter = counter,
                    };
                case Module.StaffGroup:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Nhóm nhân viên",
                        IconClassName = "fas fa-users",
                        URLPath = "/package",
                        LinkCounter = counter,
                    };
                case Module.StaffRole:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Thiết lập vai trò",
                        IconClassName = "fas fa-user-tag",
                        URLPath = "/package",
                        LinkCounter = counter,
                    }; ;
                case Module.StaffAccount:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Tài khoản nhân viên",
                        IconClassName = "fas fa-user-circle",
                        URLPath = "/package",
                        LinkCounter = counter,
                    };
                case Module.StaffTimeWorking:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Thời gian làm việc",
                        IconClassName = "fas fa-user-clock",
                        URLPath = "/package",
                        LinkCounter = counter,
                    };
                case Module.HistoryAction:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Lịch sử thao tác",
                        IconClassName = "fas fa-history",
                        URLPath = "/package",
                        LinkCounter = counter,
                    };
                case Module.DeviceManeger:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Quản lý thiết bị",
                        IconClassName = "fas fa-laptop-house",
                        URLPath = "/package",
                        LinkCounter = counter,
                    };
                case Module.Branch:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Danh sách chi nhánh",
                        IconClassName = "fas fa-store",
                        URLPath = "/package",
                        LinkCounter = counter,
                    };
                case Module.Asset:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Tài sản",
                        IconClassName = "fas fa-hands",
                        URLPath = "/package",
                        LinkCounter = counter,
                    };
                case Module.BidaTable:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Bàn bida",
                        IconClassName = "fas fa-bowling-ball",
                        URLPath = "/bidatable",
                        LinkCounter = counter,
                    };
                case Module.BidaTableType:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Loại bàn bida",
                        IconClassName = "fas fa-table",
                        URLPath = "/bidatabletype",
                        LinkCounter = counter,
                    };
                case Module.Account:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Tài khoản",
                        IconClassName = "fas fa-user-circle",
                        URLPath = "/user",
                        LinkCounter = counter,
                    };
                case Module.LogEvent:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Nhật ký log",
                        IconClassName = "fa fa-history",
                        URLPath = "/LogEvent",
                        LinkCounter = counter,
                    };


                default:
                    break;
            }

            return null;
        }
    }
}
