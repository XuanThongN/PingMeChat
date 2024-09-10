using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Common.Dto
{
    public enum ResponseType
    {
        Success,
        ClientError,
        ServerError,
        Unauthorized,
        Forbidden,
        NotFound,
    }

    public class ApiResponseModel<T>
    {
        public static Type GetResponse(ResponseType responseType)
        {
            switch (responseType)
            {
                case ResponseType.Success:
                    return typeof(SuccessResponseDtoWithResult<T>);
                case ResponseType.ClientError:
                    return typeof(ClientErrorResponse);
                case ResponseType.ServerError:
                    return typeof(ServerErrorResponse);
                case ResponseType.Unauthorized:
                    return typeof(UnauthorizedResponse);
                case ResponseType.Forbidden:
                    return typeof(ForbiddenResponse);
                case ResponseType.NotFound:
                    return typeof(NotFoundResponse);
                default:
                    throw new ArgumentOutOfRangeException(nameof(responseType), responseType, null);
            }
        }
    }

    public abstract class BaseResponse
    {
        public string Version { get; set; } = "1.0.0.0";

        public int StatusCode { get; set; }

        public bool IsError { get; set; }
    }

    public class SuccessResponseDtoWithResult<T> : BaseResponse
    {
        public string Message { get; set; }

        public T? result { get; set; }
    }

    public class SuccessResponseDtoWithoutResult : BaseResponse
    {
        public string Message { get; set; }
    }

    public class ClientErrorResponse : BaseResponse
    {
        public string Message { get; set; }

        public ClientError Errors { get; set; }

        public class ClientError
        {
            public string Message { get; set; }

            public List<ClientErrorItem> details { get; set; }
        }

        public class ClientErrorItem
        {
            public string name { get; set; }

            public string Reason { get; set; }
        }
    }

    public class ServerErrorItem
    {
        public string Message { get; set; }

        public string StackTrace { get; set; }
    }

    public class ServerErrorResponse : BaseResponse
    {
        public ServerErrorItem errors { get; set; }
    }

    public class UnauthorizedResponse : BaseResponse
    {
        public UnauthorizedResponse()
        {
            Message = "Unauthorized";
            StatusCode = StatusCodes.Status401Unauthorized;
            IsError = true;
        }

        public string Message { get; set; }
    }

    public class ForbiddenResponse : BaseResponse
    {
        public ForbiddenResponse()
        {
            Message = "Forbidden";
            StatusCode = StatusCodes.Status403Forbidden;
            IsError = true;
        }

        public string Message { get; set; }
    }

    public class NotFoundResponse : BaseResponse
    {
        public class ErrorMessage
        {
            public string Message { get; set; }
        }

        public ErrorMessage Error { get; set; }

        public NotFoundResponse()
        {
            Error = new ErrorMessage
            {
                Message = "Not Found"
            };
            StatusCode = StatusCodes.Status404NotFound;
            IsError = true;
        }
    }

    public class NotImplementedResponse : BaseResponse
    {
        public NotImplementedErrorItem errors { get; set; }

        public class NotImplementedErrorItem
        {
            public string Message { get; set; }
        }
    }
}
