using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.Application.Common.Filters
{
    public class ValidateUserAndModelAttribute : ServiceFilterAttribute
    {
        public ValidateUserAndModelAttribute() : base(typeof(ValidateUserAndModelFilter))
        {
        }
    }

    public class ValidateUserAndModelFilter : IAsyncActionFilter
    {
        private readonly InfoUserNotFoundFilter _infoUserNotFoundFilter;
        private readonly ModelStateFilter _modelStateFilter;

        public ValidateUserAndModelFilter(InfoUserNotFoundFilter infoUserNotFoundFilter, ModelStateFilter modelStateFilter)
        {
            _infoUserNotFoundFilter = infoUserNotFoundFilter;
            _modelStateFilter = modelStateFilter;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Tạo một delegate giả không làm gì cả
            ActionExecutionDelegate noOpNext = () => Task.FromResult(new ActionExecutedContext(context, context.Filters, context.Controller));

            // Chạy các filter mà không thực sự gọi next()
            await _modelStateFilter.OnActionExecutionAsync(context, noOpNext);

            // Nếu ModelState filter đã set Result, không cần chạy tiếp
            if (context.Result != null)
            {
                return;
            }

            await _infoUserNotFoundFilter.OnActionExecutionAsync(context, noOpNext);

            // Nếu không có Result nào được set, tiếp tục với action thực sự
            if (context.Result == null)
            {
                await next();
            }


        }
    }
}
