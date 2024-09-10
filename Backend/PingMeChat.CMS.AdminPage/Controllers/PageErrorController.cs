using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    [AllowAnonymous]

    public class PageErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Error401() {
            return View();
        }

        public IActionResult Error404()
        {
            return View();
        }
    }
}
