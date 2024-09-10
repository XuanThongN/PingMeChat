using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    [AllowAnonymous]
    public class OrderTableController : BaseController
    {

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
