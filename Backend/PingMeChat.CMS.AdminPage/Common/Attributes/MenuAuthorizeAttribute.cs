using PingMeChat.CMS.Application.Feature.Service.Menus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web;

namespace PingMeChat.CMS.AdminPage.Common.Attributes
{
    // Attribute này kiểm tra xem người dùng đã đăng nhập chưa thông qua user.Identity.IsAuthenticated.
    // Kế thừa từ AuthorizeAttribute và implement IAsyncAuthorizationFilter, cho phép bạn thực hiện kiểm tra quyền truy cập bất đồng bộ.
    //public class MenuAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    //{
    //    private readonly string _menuAccess;

    //    public MenuAuthorizeAttribute(string menuAccess)
    //    {
    //        _menuAccess = menuAccess;
    //    }
    //    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    //    {
    //        var user = context.HttpContext.User;

    //        //Nếu người dùng chưa đăng nhập: Trả về UnauthorizedResult (HTTP 401).
    //        if (!user.Identity.IsAuthenticated)
    //        {
    //            context.Result = new UnauthorizedResult();
    //            return;
    //        }

    //        var userId = user.FindFirstValue("userId");

    //        //Nếu đã đăng nhập nhưng không tìm thấy claim nào chứa userId: Trả về ForbidResult(HTTP 403).
    //        if (string.IsNullOrEmpty(userId))
    //        {
    //            context.Result = new ForbidResult();
    //            return;
    //        }

    //        //Sử dụng IMenuService để lấy danh sách menu của người dùng.
    //        var menuService = context.HttpContext.RequestServices.GetRequiredService<IMenuService>();
    //        var userMenus = await menuService.GetMenusByUserId(userId);

    //        if (!userMenus.Any(m => m.Access == _menuAccess))
    //        {
    //            //  Nếu đã đăng nhập nhưng không có quyền truy cập menu: Trả về ForbidResult(HTTP 403).
    //            context.Result = new ForbidResult();
    //            return;
    //        }

    //        // Nếu có quyền, không cần set context.Result, cho phép tiếp tục xử lý
    //    }
    //}
}
