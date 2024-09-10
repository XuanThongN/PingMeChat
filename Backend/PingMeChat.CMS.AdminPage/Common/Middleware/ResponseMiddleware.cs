using System.Net;

namespace PingMeChat.CMS.AdminPage.Common.Middleware
{
    public class ResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
            context.Request.EnableBuffering();

            //if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            //{
            //    await context.Response.WriteAsJsonAsync(new UnauthorizedResponse());
            //}

            //if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            //{
            //    await context.Response.WriteAsJsonAsync(new ForbiddenResponse());
            //}

            //if (context.Response.StatusCode == (int)StatusCodes.Status404NotFound)
            //{
            //    await context.Response.WriteAsJsonAsync(new NotFoundResponse());
            //}
        }
    }
}
