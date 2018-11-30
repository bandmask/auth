using Auth.Controllers;
using Auth.Errors;
using NUnit.Framework;

namespace Auth.Tests.Controllers
{
    public class BaseControllerUT
    {
        private BaseController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new BaseController();
        }

        [Test]
        public void ApiErro_with_NO_parameters_Should_return_basic_result()
        {
            AssertEquality(new ApiError(), _controller.ApiError());
        }

        [Test]
        public void ApiErro_with_statusCode_and_description_Should_error_result()
        {
            var statusCode = 300;
            var statusDescription = "some random description";

            AssertEquality(new ApiError(statusCode, statusDescription), _controller.ApiError(statusCode, statusDescription));
        }

        [Test]
        public void ApiErro_with_statusCode_and_description_and_message_Should_return_result()
        {
            var statusCode = 300;
            var statusDescription = "some random description";
            var message = "some random error message";

            AssertEquality(new ApiError(statusCode, statusDescription, message), _controller.ApiError(statusCode, statusDescription, message));
        }

        private void AssertEquality(ApiError expected, ApiErrorResult actual)
        {
            var actualError = actual.Value as ApiError;
            Assert.AreEqual(expected.StatusCode, actualError.StatusCode);
            Assert.AreEqual(expected.StatusDescription, actualError.StatusDescription);
            Assert.AreEqual(expected.Message, actualError.Message);
        }
    }
}
