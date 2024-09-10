using PingMeChat.CMS.AdminPage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace PingMeChat.CMS.AdminPage.ViewComponents
{
    public class MenuUserViewComponent : ViewComponent
    {

        public MenuUserViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            return View();
        }
    }
}
