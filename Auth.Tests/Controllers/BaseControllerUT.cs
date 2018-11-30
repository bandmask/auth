using Auth.Controllers;
using Auth.Results;
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
        public void ApiToken_with_NO_parameters_Should_return_basic_result()
        {
            AssertTokenEquality(new ApiToken(), _controller.ApiToken());
        }

        [Test]
        public void ApiToken_with_token_Should_return_token_result()
        {
            var temptoken = "temptoken";
            AssertTokenEquality(new ApiToken(temptoken), _controller.ApiToken(temptoken));
        }

        [Test]
        public void ApiError_with_NO_parameters_Should_return_basic_result()
        {
            AssertErrorEquality(new ApiError(), _controller.ApiError());
        }

        [Test]
        public void ApiError_with_statusCode_and_description_Should_error_result()
        {
            var statusCode = 300;
            var statusDescription = "some random description";

            AssertErrorEquality(new ApiError(statusCode, statusDescription), _controller.ApiError(statusCode, statusDescription));
        }

        [Test]
        public void ApiError_with_statusCode_and_description_and_message_Should_return_result()
        {
            var statusCode = 300;
            var statusDescription = "some random description";
            var message = "some random error message";

            AssertErrorEquality(new ApiError(statusCode, statusDescription, message), _controller.ApiError(statusCode, statusDescription, message));
        }

        private void AssertErrorEquality(ApiError expected, ApiErrorResult actual)
        {
            var actualError = actual.Value as ApiError;
            Assert.AreEqual(expected.StatusCode, actualError.StatusCode);
            Assert.AreEqual(expected.StatusDescription, actualError.StatusDescription);
            Assert.AreEqual(expected.Message, actualError.Message);
        }

        private void AssertTokenEquality(ApiToken expected, ApiTokenResult actual)
        {
            var actualToken = actual.Value as ApiToken;
            Assert.AreEqual(expected.StatusCode, actualToken.StatusCode);
            Assert.AreEqual(expected.Token, actualToken.Token);
        }
    }
}
