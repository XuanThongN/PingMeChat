using PingMeChat.CMS.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Common.Filter
{
    public class InfoUserNotFoundFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string? email = context.HttpContext.User.FindFirstValue(ClaimTypes.Email);

            if (String.IsNullOrEmpty(email))
                throw new AppException("Không tìm thấy email tài khoản đang hoạt động", 404);

            var route = context.HttpContext.Request.Path.Value;
            if (String.IsNullOrEmpty(route))
                throw new AppException("Yêu cầu tài nguyên từ url không tìm thấy", 404);

            var userId = context.HttpContext.User.FindFirstValue("UserId");
            if (String.IsNullOrEmpty(userId))
                throw new AppException("Không tìm thấy tài khoản trong hệ thống", 404);

            var userName = context.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            if (String.IsNullOrEmpty(userName))
                throw new AppException("Không tìm thấy tài khoản trong hệ thống", 404);


            // Nếu tất cả các kiểm tra đều pass, tiếp tục thực hiện action
            await next();
        }
    }
}
