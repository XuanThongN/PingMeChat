using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.AdminPage.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {

        public FooterViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            return View();
        }
    }
}
