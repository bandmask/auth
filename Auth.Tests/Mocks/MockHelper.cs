using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace Auth.Tests.Mocks
{
    public static class MockHelper
    {
        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var logger = new Mock<ILogger<UserManager<TUser>>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, new IdentityErrorDescriber(), null, logger.Object);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
            return mgr;
        }

        public static Mock<SignInManager<TUser>> MockSignInManager<TUser>(UserManager<TUser> userManager) where TUser : class
        {
            var context = new Mock<HttpContext>();

            return new Mock<SignInManager<TUser>>(userManager,
                new HttpContextAccessor { HttpContext = context.Object },
                new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
                null, null, null) { CallBase = true };
        }
    }
}
