using PingMeChat.CMS.AdminPage.Models;
using Microsoft.AspNetCore.Mvc;
using PingMeChat.CMS.Application;

namespace PingMeChat.CMS.AdminPage.ViewComponents
{
    public class MenuNotificationViewComponent : ViewComponent
    {

        public MenuNotificationViewComponent()
        {
           
        }

        public async Task<IViewComponentResult> InvokeAsync(string filter)
        {
          
            return View();
        }

        
    }
}
