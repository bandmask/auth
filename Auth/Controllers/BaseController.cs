using Auth.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    public class BaseController : Controller
    {
        public virtual HttpContext GetHttpContext()
        {
            return base.HttpContext;
        }

        public ApiTokenResult ApiToken()
        {
            return new ApiTokenResult();
        }

        public ApiTokenResult ApiToken(string token)
        {
            return new ApiTokenResult(token);
        }

        public ApiErrorResult ApiError()
        {
            return new ApiErrorResult();
        }

        public ApiErrorResult ApiError(int statusCode, string statusDescription)
        {
            return ApiError(statusCode, statusDescription, null);
        }

        public ApiErrorResult ApiError(int statusCode, string statusDescription, string message)
        {
            return new ApiErrorResult(new ApiError(statusCode, statusDescription, message));
        }
    }
}
