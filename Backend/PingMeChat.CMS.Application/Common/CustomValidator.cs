using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PingMeChat.CMS.Application
{
    public class CustomValidator
    {
        public static ErrorDetail ErrorDetail { get; set; } = new ErrorDetail();

        public static IActionResult MakeValidationResponse(ActionContext context)
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
            };
            foreach (var keyModelStatePair in context.ModelState)
            {
                var errors = keyModelStatePair.Value.Errors;
                if (errors != null && errors.Count > 0)
                {
                    if (errors.Count == 1)
                    {
                        var errorMessage = GetErrorMessage(errors[0]);
                        ErrorDetail.errors.Add(new Error()
                        {
                            errorCode = "Contract Violation",
                            message = errorMessage,
                            priority = "MEDIUM",
                            properties = new Properties()
                            {
                                name = "string",
                                value = "string"
                            }
                        });
                    }
                    else
                    {
                        var errorMessages = new string[errors.Count];
                        for (var i = 0; i < errors.Count; i++)
                        {
                            errorMessages[i] = GetErrorMessage(errors[i]);
                            ErrorDetail.errors.Add(new Error()
                            {
                                errorCode = "Contract Violation",
                                message = errorMessages[i],
                                priority = "MEDIUM"
                            });
                        }
                    }
                }
            }

            ErrorDetail.traceId = context.HttpContext.TraceIdentifier;
            ErrorDetail.timestamp = DateTime.Now;

            var result = new BadRequestObjectResult(ErrorDetail);

            result.ContentTypes.Add("application/problem+json");

            return result;
        }

        static string GetErrorMessage(ModelError error)
        {
            return string.IsNullOrEmpty(error.ErrorMessage) ? "The input was not valid." : error.ErrorMessage;
        }
    }

    public class ErrorDetail
    {
        public string traceId { get; set; }

        public DateTime timestamp { get; set; }

        public List<Error> errors { get; set; } = new List<Error>();
    }

    public class Error
    {
        public string errorCode { get; set; }

        public string message { get; set; }

        public string priority { get; set; }

        public Properties properties { get; set; } = new Properties();
    }

    public class Properties
    {
        public string name { get; set; }

        public string value { get; set; }
    }
}
