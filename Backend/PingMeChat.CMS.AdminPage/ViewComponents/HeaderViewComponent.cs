using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.AdminPage.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {

        public HeaderViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            return View();
        }
    }
}
