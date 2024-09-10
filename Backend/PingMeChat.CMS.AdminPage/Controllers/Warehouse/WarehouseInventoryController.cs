using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.AdminPage.Controllers.Warehouse
{
    // tồn kho
    [AllowAnonymous]

    public class WarehouseInventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
