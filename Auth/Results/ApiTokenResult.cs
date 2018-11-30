using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Auth.Results
{
    public class ApiTokenResult : ObjectResult
    {
        public ApiTokenResult() : base(new ApiToken()) { }
        public ApiTokenResult(object value) : base(value) { }
        public ApiTokenResult(string token) : base(new ApiToken(token)) { }
    }

    public class ApiToken : BaseApiResult
    {
        public int StatusCode { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Token { get; set; }

        public ApiToken()
        {
            StatusCode = 500;
        }

        public ApiToken(string token)
        {
            StatusCode = 200;
            Token = token;
        }
    }
}
