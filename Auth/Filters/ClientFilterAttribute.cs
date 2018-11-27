using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Auth.Filters
{
    public class ClientFilterAttribute : ActionFilterAttribute
    {
        private const string CLIENT_HEADER_NAME = "ropr-domain-client";

        private readonly IList<string> allowedClients;

        public ClientFilterAttribute(IList<string> allowedClients)
        {
            this.allowedClients = allowedClients;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Headers.TryGetValue(CLIENT_HEADER_NAME, out var client) && allowedClients.Contains(client))
            {
                await base.OnActionExecutionAsync(context, next);
            }
            else
            {
                context.HttpContext.Response.ContentType = "application/json";
                var error = new HeaderNotPresentError();
                context.HttpContext.Response.StatusCode = error.StatusCode;
                await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(error));
            }
        }
    }
}
