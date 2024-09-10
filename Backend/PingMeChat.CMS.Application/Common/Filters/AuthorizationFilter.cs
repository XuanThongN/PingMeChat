using PingMeChat.CMS.Application.Feature.Service.Menus;
using PingMeChat.CMS.Application.Feature.Service.Users;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using PingMeChat.CMS.Entities;
using System.Reflection;

namespace PingMeChat.CMS.Application.Common.Filters
{
    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        private IMenuService _menuService;
        private IUserService _userService;
        private const string SuperAdminUsername = "superadmin";

        public AuthorizationFilter(IMenuService menuService, IUserService userService)
        {
            _menuService = menuService;
            _userService = userService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Bỏ qua kiểm tra cho các action được đánh dấu AllowAnonymous
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
                return;

            // Kiểm tra xem action có cần bảo vệ không
            if (!IsProtectedAction(context))
                return;

            // Kiểm tra xác thực người dùng
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Cho phép truy cập đối với superadmin
            if (context.HttpContext.User.Identity.Name.Equals(SuperAdminUsername, StringComparison.OrdinalIgnoreCase))
                return;

            // Lấy thông tin action hiện tại
            var actionId = GetActionId(context);

            // Lấy thông tin người dùng hiện tại
            var userName = context.HttpContext.User.Identity.Name;
            var currentUser = await _userService.Find(x => x.UserName == userName);

            // Lấy danh sách menu mà người dùng có quyền truy cập
            var menus = await _menuService.GetRolesByUserId(currentUser.Id);

            // Kiểm tra quyền truy cập
            foreach (var menu in menus)
            {
                if (!menu.Access.Any() || menu.Access.Count() == 0)
                    continue;

             //   var accessList = JsonConvert.DeserializeObject<IEnumerable<MvcActionInfo>>(menu.Access);
                if (menu.Access.Any(a => a.Id == actionId))
                    return; // Cho phép truy cập nếu tìm thấy quyền
            }

            // Từ chối truy cập nếu không tìm thấy quyền phù hợp
            context.Result = new ForbidResult();
        }

        // Kiểm tra xem action có cần bảo vệ không
        private bool IsProtectedAction(AuthorizationFilterContext context)
        {
            // Bỏ qua nếu action được đánh dấu AllowAnonymous
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
                return false;

            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;
            var actionMethodInfo = controllerActionDescriptor.MethodInfo;

            // Kiểm tra Authorize attribute ở cấp Controller
            if (controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>() != null)
                return true;

            // Kiểm tra Authorize attribute ở cấp Action
            if (actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>() != null)
                return true;

            return false;
        }

        // Tạo ID duy nhất cho mỗi action
        private string GetActionId(AuthorizationFilterContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var area = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue ?? string.Empty;
            var controller = controllerActionDescriptor.ControllerName;
            var action = controllerActionDescriptor.ActionName;
            return $"{area}:{controller}:{action}";
        }
    }
}
