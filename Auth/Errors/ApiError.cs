using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Auth.Errors
{
    public class ApiErrorResult : ObjectResult
    {
        public ApiErrorResult() : this(new ApiError()) { }

        public ApiErrorResult(object value) : base(value) { }
    }

    public class ApiError
    {
        public int StatusCode { get; private set; }
        public string StatusDescription { get; private set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; private set; }

        public ApiError() : this(500, "Something went wrong") { }

        public ApiError(int statusCode, string statusDescription)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }

        public ApiError(int statusCode, string statusDescription, string message) : this(statusCode, statusDescription)
        {
            Message = message;
        }
    }

    public class HeaderNotPresentError : ApiError
    {
        public HeaderNotPresentError() : base(403, "Client header not present or invalid", "Provide a valid client header. No further information at the moment.") { }
    }
}
