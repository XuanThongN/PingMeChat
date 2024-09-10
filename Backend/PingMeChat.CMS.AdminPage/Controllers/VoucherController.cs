using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    public class VoucherController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
