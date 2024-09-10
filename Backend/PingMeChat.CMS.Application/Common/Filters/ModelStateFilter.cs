using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.Shared.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace PingMeChat.CMS.Application.Common.Filters
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

                throw new AppException(Message.Error.InvalidRequest, errors, 400);
            }
            // Nếu ModelState hợp lệ, tiếp tục thực hiện action
            await next();
        }
    }
}
