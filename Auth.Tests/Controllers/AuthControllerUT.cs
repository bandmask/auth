using System.Threading.Tasks;
using Auth.Controllers;
using Auth.Models;
using Auth.Results;
using Auth.Settings;
using Auth.Tests.Mocks;
using Auth.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Auth.Tests.Controllers
{
    public abstract class AuthControllerUT
    {
        protected const string TOKEN = "bearer-token";
        protected Mock<ITokenFactory> _tokenFactory;
        protected Mock<UserManager<AppUser>> _userManager;
        protected Mock<SignInManager<AppUser>> _signInManager;

        public virtual void Setup()
        {
            _tokenFactory = new Mock<ITokenFactory>();
            _tokenFactory.Setup(factory => factory.CreateToken(It.IsAny<JwtIssuerOptions>(), It.IsAny<string>(), It.IsAny<AppUser>())).Returns(TOKEN);

            _userManager = MockHelper.MockUserManager<AppUser>();
            _signInManager = MockHelper.MockSignInManager<AppUser>(_userManager.Object);
        }

        public virtual void Teardown()
        {
            _userManager.Reset();
            _signInManager.Reset();
        }

        public AuthController GetController()
        {
            var mockController = new Mock<AuthController>(_tokenFactory.Object, _userManager.Object, _signInManager.Object, new JwtIssuerOptions());
            mockController.Setup(controller => controller.GetHttpContext()).Returns(GetHttpContext());
            return mockController.Object;
        }

        private HttpContext GetHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("ropr-domain-client", "client");
            return context;
        }
    }

    public class When_user_NOT_found : AuthControllerUT
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _userManager.Setup(manager => manager.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser) null);
        }

        [TearDown]
        public override void Teardown()
        {
            base.Teardown();
        }

        [Test]
        public async Task Should_return_error()
        {
            var authController = GetController();
            var apiErrorResult = (await authController.Login(new CredentialsModel())).Result as ApiErrorResult;
            Assert.IsNotNull(apiErrorResult);

            var apiError = apiErrorResult.Value as ApiError;
            Assert.IsNotNull(apiError);
            Assert.AreEqual(500, apiError.StatusCode);
            Assert.AreEqual("User not found", apiError.StatusDescription);
        }
    }

    public class When_login_NOT_successful : AuthControllerUT
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _userManager.Setup(manager => manager.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new AppUser());
            _signInManager.Setup(manager =>
                    manager.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
        }

        [TearDown]
        public override void Teardown()
        {
            base.Teardown();
        }

        [Test]
        public async Task Should_return_error()
        {
            var authController = GetController();
            var apiErrorResult = (await authController.Login(new CredentialsModel())).Result as ApiErrorResult;
            Assert.IsNotNull(apiErrorResult);

            var apiError = apiErrorResult.Value as ApiError;
            Assert.IsNotNull(apiError);
            Assert.AreEqual(500, apiError.StatusCode);
            Assert.AreEqual("Error signing in", apiError.StatusDescription);
        }
    }

    public class When_login_successful : AuthControllerUT
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _userManager.Setup(manager => manager.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new AppUser());
            _signInManager.Setup(manager =>
                    manager.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        }

        [TearDown]
        public override void Teardown()
        {
            base.Teardown();
        }

        [Test]
        public async Task Should_return_token()
        {
            var authController = GetController();
            var apiTokenReuslt = (await authController.Login(new CredentialsModel())).Result as ApiTokenResult;
            Assert.IsNotNull(apiTokenReuslt);

            var apiToken = apiTokenReuslt.Value as ApiToken;
            Assert.IsNotNull(apiToken);

            Assert.AreEqual(200, apiToken.StatusCode);
            Assert.AreEqual(TOKEN, apiToken.Token);
        }
    }

    public class When_register_NOT_successful : AuthControllerUT
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _userManager.Setup(manager => manager.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());
        }

        [TearDown]
        public override void Teardown()
        {
            base.Teardown();
        }

        [Test]
        public async Task Should_return_token()
        {
            var authController = GetController();
            var apiErrorResult = (await authController.Register(new CredentialsModel())).Result as ApiErrorResult;
            Assert.IsNotNull(apiErrorResult);

            var apiError = apiErrorResult.Value as ApiError;
            Assert.IsNotNull(apiError);
            Assert.AreEqual(500, apiError.StatusCode);
            Assert.AreEqual("Error registering", apiError.StatusDescription);
        }
    }

    public class When_register_login_NOT_successful : AuthControllerUT
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _userManager.Setup(manager => manager.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(manager => manager.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser) null);
        }

        [TearDown]
        public override void Teardown()
        {
            base.Teardown();
        }

        [Test]
        public async Task Should_return_token()
        {
            var authController = GetController();
            var apiErrorResult = (await authController.Register(new CredentialsModel())).Result as ApiErrorResult;
            Assert.IsNotNull(apiErrorResult);

            var apiError = apiErrorResult.Value as ApiError;
            Assert.IsNotNull(apiError);
            Assert.AreEqual(500, apiError.StatusCode);
            Assert.AreEqual("Error signing in", apiError.StatusDescription);
        }
    }

    public class When_register_successful : AuthControllerUT
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _userManager.Setup(manager => manager.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(manager => manager.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new AppUser());
        }

        [TearDown]
        public override void Teardown()
        {
            base.Teardown();
        }

        [Test]
        public async Task Should_return_token()
        {
            var authController = GetController();
            var apiTokenResult = (await authController.Register(new CredentialsModel())).Result as ApiTokenResult;
            Assert.IsNotNull(apiTokenResult);

            var apiToken = apiTokenResult.Value as ApiToken;
            Assert.IsNotNull(apiToken);
            Assert.AreEqual(200, apiToken.StatusCode);
            Assert.AreEqual(TOKEN, apiToken.Token);
        }
    }
}
