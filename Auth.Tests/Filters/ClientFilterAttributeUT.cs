using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Auth.Filters;
using Auth.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Auth.Tests.Filters
{
    public class ClientFilterAttributeUT
    {
        private ClientFilterAttribute _clientFilterAttribute;

        [SetUp]
        public void Setup()
        {
            var allowedClients = new List<string>
            {
                "ropr",
                "daysOfRum"
            };

            _clientFilterAttribute = new ClientFilterAttribute(allowedClients);
        }

        [Test]
        public async Task When_NO_headers_exists_Should_block()
        {
            var httpContext = new DefaultHttpContext();

            var context = GetContext(GetActionContext(httpContext));

            using(var responseBody = new MemoryStream())
            {
                httpContext.Response.Body = responseBody;

                await _clientFilterAttribute.OnActionExecutionAsync(context, null);

                Assert.IsNotNull(httpContext.Response);
                Assert.AreEqual(403, httpContext.Response.StatusCode);
                Assert.AreEqual("application/json", httpContext.Response.ContentType);

                var body = await GetBodyAsString(httpContext.Response.Body);

                Assert.IsNotEmpty(body);
                Assert.AreEqual(JsonConvert.SerializeObject(new HeaderNotPresentError()), body);
            }
        }

        [Test]
        public async Task When_NO_valid_headers_exists_Should_block()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("ropr-domain-client", "someInvalidHeader");

            var context = GetContext(GetActionContext(httpContext));

            using(var responseBody = new MemoryStream())
            {
                httpContext.Response.Body = responseBody;

                await _clientFilterAttribute.OnActionExecutionAsync(context, null);

                Assert.IsNotNull(httpContext.Response);
                Assert.AreEqual(403, httpContext.Response.StatusCode);
                Assert.AreEqual("application/json", httpContext.Response.ContentType);

                var body = await GetBodyAsString(httpContext.Response.Body);

                Assert.IsNotEmpty(body);
                Assert.AreEqual(JsonConvert.SerializeObject(new HeaderNotPresentError()), body);
            }
        }

        [Test]
        public async Task When_valid_headers_exists_Should_NOT_block()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("ropr-domain-client", "ropr");

            var context = GetContext(GetActionContext(httpContext));

            using(var responseBody = new MemoryStream())
            {
                httpContext.Response.Body = responseBody;

                var expectedResult = new SuccessResult { Success = true };
                await _clientFilterAttribute.OnActionExecutionAsync(context, async() =>
                {
                    var innerContext = new ActionExecutedContext(GetActionContext(httpContext), new List<IFilterMetadata>(), new Mock<Controller>().Object);
                    await innerContext.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(expectedResult));
                    innerContext.HttpContext.Response.StatusCode = 200;
                    return innerContext;
                });

                Assert.IsNotNull(httpContext.Response);
                Assert.AreEqual(200, httpContext.Response.StatusCode);

                var body = await GetBodyAsString(httpContext.Response.Body);

                Assert.IsNotEmpty(body);
                Assert.AreEqual(JsonConvert.SerializeObject(expectedResult), body);
            }
        }

        private ActionContext GetActionContext(HttpContext httpContext)
        {
            var actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };
            return actionContext;
        }

        private ActionExecutingContext GetContext(ActionContext actionContext)
        {
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object);

            return context;
        }

        private async Task<string> GetBodyAsString(Stream bodyStream)
        {
            bodyStream.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(bodyStream).ReadToEndAsync();
            bodyStream.Seek(0, SeekOrigin.Begin);
            return body;
        }

        private class SuccessResult
        {
            public bool Success { get; set; }
        }
    }
}
