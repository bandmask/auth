using System.Threading.Tasks;
using Auth.Models;
using Auth.Settings;
using Auth.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public partial class AuthController : BaseController
    {
        private const string CLIENT_HEADER_NAME = "ropr-domain-client";

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JwtIssuerOptions _jwtIssuerOptions;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, JwtIssuerOptions jwtIssuerOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtIssuerOptions = jwtIssuerOptions;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CredentialsModel credentials)
        {
            var user = await _userManager.FindByEmailAsync(credentials.Email);
            if (user == null)
            {
                return HandleError("User not found");
            }

            var result = await _signInManager.PasswordSignInAsync(user, credentials.Password, true, false);
            if (!result.Succeeded)
            {
                return HandleError("Error signing in");
            }

            return Ok($"Bearer { TokenFactory.CreateToken(_jwtIssuerOptions, GetClient(HttpContext), user) }");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CredentialsModel credentials)
        {
            var user = new AppUser { UserName = credentials.UserName, Email = credentials.Email };
            var result = await _userManager.CreateAsync(user, credentials.Password);
            if (!result.Succeeded)
            {
                return HandleError("Error registering");
            }
            // Not a good solution - prefer confirmation mail or the sorts - just for debugging purposes
            var signedInUser = await _userManager.FindByEmailAsync(credentials.Email);
            if (signedInUser == null)
            {
                return HandleError("Error signing in");
            }

            return Ok($"Bearer { TokenFactory.CreateToken(_jwtIssuerOptions, GetClient(HttpContext), signedInUser) }");
        }

        [HttpPost("validate")]
        public IActionResult Validate()
        {
            return Ok(new { StatusCode = 200 });
        }

        [HttpPost("validaterole")]
        [Authorize(Roles = "asd")]
        public IActionResult ValidateRole()
        {
            return Ok(new { StatusCode = 200 });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { StatusCode = 200 });
        }

        private IActionResult HandleError(string reason)
        {
            return ApiError(500, reason);
        }

        private string GetClient(HttpContext httpContext)
        {
            return httpContext.Request.Headers[CLIENT_HEADER_NAME];
        }
    }
}
