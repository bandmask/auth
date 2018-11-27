using Auth.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    public class BaseController : Controller
    {
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
