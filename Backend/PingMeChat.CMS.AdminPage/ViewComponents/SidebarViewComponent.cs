using Microsoft.AspNetCore.Mvc;
using PingMeChat.CMS.AdminPage.Common;
using PingMeChat.CMS.AdminPage.Models;
using PingMeChat.CMS.Application.Feature.Service.Menus;
using System.Web.Mvc;

namespace PingMeChat.CMS.AdminPage.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        public SidebarViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            var sidebars = new List<SidebarMenu>();

            sidebars.Add(ModuleHelper.AddModule(ModuleHelper.Module.Home));
            sidebars.Add(ModuleHelper.AddTree("Danh mục", "fas fa-list-ul"));
            sidebars.Last().TreeChild = new List<SidebarMenu>()
            {
                ModuleHelper.AddModule(ModuleHelper.Module.BidaTableType),
                ModuleHelper.AddModule(ModuleHelper.Module.BidaTable),

            };
            sidebars.Add(ModuleHelper.AddTree("Bán hàng", "fas fa-shopping-cart"));
            sidebars.Last().TreeChild = new List<SidebarMenu>()
            {
                ModuleHelper.AddModule(ModuleHelper.Module.Order),
                //ModuleHelper.AddModule(ModuleHelper.Module.Voucher),

            }; sidebars.Add(ModuleHelper.AddTree("Đối tác", "fas fa-people-arrows"));
            sidebars.Last().TreeChild = new List<SidebarMenu>()
            {
                ModuleHelper.AddModule(ModuleHelper.Module.Customer),
                //ModuleHelper.AddModule(ModuleHelper.Module.CustomerAgencyType),
                //ModuleHelper.AddModule(ModuleHelper.Module.CustomerAgency),

            };
            sidebars.Add(ModuleHelper.AddTree("Sản phẩm, dịch vụ", "fab fa-servicestack"));
            sidebars.Last().TreeChild = new List<SidebarMenu>()
            {
                 //ModuleHelper.AddModule(ModuleHelper.Module.Producttype),
                ModuleHelper.AddModule(ModuleHelper.Module.Product),
            };

            sidebars.Add(ModuleHelper.AddTree("Quản lý kho", "fas fa-box"));
            sidebars.Last().TreeChild = new List<SidebarMenu>()
            {
                 ModuleHelper.AddModule(ModuleHelper.Module.Inventory),
                ModuleHelper.AddModule(ModuleHelper.Module.InventoryImport),
                ModuleHelper.AddModule(ModuleHelper.Module.InventoryExport),
            };

            //sidebars.Add(ModuleHelper.AddTree("Quản lý thu chi", "fas fa-money-check-alt"));
            //sidebars.Last().TreeChild = new List<SidebarMenu>()
            //{
            //     ModuleHelper.AddModule(ModuleHelper.Module.Cashbook),
            //    ModuleHelper.AddModule(ModuleHelper.Module.Transactioncategory),
            //    ModuleHelper.AddModule(ModuleHelper.Module.Transaction),
            //    ModuleHelper.AddModule(ModuleHelper.Module.Moneyreport),
            //};
            sidebars.Add(ModuleHelper.AddModule(ModuleHelper.Module.Report));


            //sidebars.Add(ModuleHelper.AddTree("Nhân viên", "fas fa-user-check"));
            //sidebars.Last().TreeChild = new List<SidebarMenu>()
            //{
            //     ModuleHelper.AddModule(ModuleHelper.Module.StaffGroup),
            //    ModuleHelper.AddModule(ModuleHelper.Module.StaffAccount),
            //    ModuleHelper.AddModule(ModuleHelper.Module.StaffRole),
            //    ModuleHelper.AddModule(ModuleHelper.Module.StaffTimeWorking),
            //};
            
            sidebars.Add(ModuleHelper.AddTree("Quản trị", "fa fa-id-card"));
            sidebars.Last().TreeChild = new List<SidebarMenu>()
            {
                // ModuleHelper.AddModule(ModuleHelper.Module.Branch),
                //ModuleHelper.AddModule(ModuleHelper.Module.HistoryAction),
                //ModuleHelper.AddModule(ModuleHelper.Module.Asset),
                //ModuleHelper.AddModule(ModuleHelper.Module.DeviceManeger),
                ModuleHelper.AddModule(ModuleHelper.Module.Account),
                ModuleHelper.AddModule(ModuleHelper.Module.Role),
            };

            // lọc từ danh sách role người dùng
            if (User.IsInRole("SuperAdmin"))
            {
                sidebars.Add(ModuleHelper.AddTree("Quản trị", "fas fa-user-shield"));
                sidebars.Last().TreeChild = new List<SidebarMenu>()
                {
                    ModuleHelper.AddModule(ModuleHelper.Module.User),
                    ModuleHelper.AddModule(ModuleHelper.Module.Role),
                  //  ModuleHelper.AddModule(ModuleHelper.Module.LogEvent),
                };
            }

            return View(sidebars);
        }
    }


}
