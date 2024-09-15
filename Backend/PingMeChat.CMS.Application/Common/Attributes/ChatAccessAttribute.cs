using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PingMeChat.CMS.Application.Feature.Service.Chats;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ChatAccessAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var chatId = context.ActionArguments["chatId"] as string;
            if (string.IsNullOrEmpty(chatId))
            {
                context.Result = new BadRequestObjectResult("ChatId is required."); 
                return;
            }

            var userId = context.HttpContext.User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var chatService = context.HttpContext.RequestServices.GetRequiredService<IChatService>();
            var canAccess = await chatService.CanUserAccessChat(chatId, userId);

            if (!canAccess)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
