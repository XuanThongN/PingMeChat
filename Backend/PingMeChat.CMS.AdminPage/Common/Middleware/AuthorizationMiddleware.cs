using PingMeChat.CMS.Application.App.IRepositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Common.Middleware
{
    public class AuthorizationMiddleware
    {
        //private readonly RequestDelegate _next;
        //private readonly IMenuRepository _menuRepository;
        //private readonly IUsersMenuRepository _usersMenuRepository;

        //public AuthorizationMiddleware(RequestDelegate next,
        //    IMenuRepository menuRepository,
        //    IUsersMenuRepository usersMenuRepository)
        //{
        //    _next = next;
        //    _menuRepository = menuRepository;
        //    _usersMenuRepository = usersMenuRepository;
        //}

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    var endpoint = context.GetEndpoint();
        //    if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        //    {
        //        await _next(context);
        //        return;
        //    }

        //    var user = context.User;
        //    if (!user.Identity.IsAuthenticated)
        //    {
        //        context.Response.StatusCode = 401;
        //        return;
        //    }

        //    var requestPath = context.Request.Path.Value.ToLower();
        //    var menu = await _menuRepository.GetByUrlAsync(requestPath);

        //    if (menu == null)
        //    {
        //        await _next(context);
        //        return;
        //    }

        //    var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    var hasAccess = await _userMenuRepository.UserHasAccessToMenuAsync(userId, menu.Id);

        //    if (!hasAccess)
        //    {
        //        context.Response.StatusCode = 403;
        //        return;
        //    }

        //    await _next(context);
        //}
    }
}
