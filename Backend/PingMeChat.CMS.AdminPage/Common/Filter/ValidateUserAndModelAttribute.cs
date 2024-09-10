using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.AdminPage.Common.Filter
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
            await _modelStateFilter.OnActionExecutionAsync(context, next);
            await _infoUserNotFoundFilter.OnActionExecutionAsync(context, next);
            //if (context.Result == null)
            //{
               
            //}

         
        }
    }
}
