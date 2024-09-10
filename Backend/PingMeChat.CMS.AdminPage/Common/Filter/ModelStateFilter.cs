using PingMeChat.CMS.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Common.Filter
{
    public class ModelStateFilter: IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", errors, 400);
            }
            // Nếu ModelState hợp lệ, tiếp tục thực hiện action
            await next();
        }
    }
}
