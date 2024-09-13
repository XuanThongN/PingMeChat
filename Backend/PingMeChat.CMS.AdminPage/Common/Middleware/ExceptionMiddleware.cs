using AutoWrapper.Exceptions;
using AutoWrapper.Models;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.ErrorLogs;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Module;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;

namespace PingMeChat.CMS.AdminPage.Common.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ILogErrorRepository logErrorRepository, IUnitOfWork unitOfWork)
        {
            try
            {
                await _next(context);
            }
            catch (AppException appExcep)
            {
                await HandleAppExceptionAsync(context, appExcep, logErrorRepository, unitOfWork);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, logErrorRepository, unitOfWork);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogErrorRepository logErrorRepository,  IUnitOfWork unitOfWork)
        {
            _logger.LogError(exception, "An unexpected error occurred.");

            // save errorLog
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var controllerAction = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (controllerAction != null)
                {
                    var controllerName = controllerAction.ControllerName;
                    var actionName = controllerAction.ActionName;

                    // save log in database
                    var errorLog = new ErrorLog
                    {
                        ControllerName = controllerName,
                        ActionName = actionName,
                        IsError = true,
                        ErrorMessage = exception.Message,
                        Exception = typeof(Exception).Name,
                    };
                    await logErrorRepository.Add(errorLog);
                    await unitOfWork.SaveChangeAsync();

                }
            }

            context.Response.StatusCode = 500;

            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                var response = new ApiResponse("Có sự cố về hệ thống, vui lòng thử lại sau", context.Response.StatusCode);
                var json = JsonConvert.SerializeObject(response);

                await context.Response.WriteAsync(json);
            }
        }
        private async Task HandleAppExceptionAsync(HttpContext context, AppException appException, ILogErrorRepository logErrorRepository, IUnitOfWork unitOfWork)
        {
            _logger.LogError(appException, appException.Message);

            // save errorLog
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var controllerAction = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (controllerAction != null)
                {
                    var controllerName = controllerAction.ControllerName;
                    var actionName = controllerAction.ActionName;

                    // save log in database
                    var errorLog = new ErrorLog
                    {
                        ControllerName = controllerName,
                        ActionName = actionName,
                        IsError = true,
                        ErrorMessage = appException.Message,
                        Exception = typeof(AppException).Name,
                    };
                    await logErrorRepository.Add(errorLog);
                    await unitOfWork.SaveChangeAsync();
                }
            }

            context.Response.StatusCode = appException.Status;

            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                var response = new ApiResponse(appException.Message, context.Response.StatusCode);
                var json = JsonConvert.SerializeObject(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
