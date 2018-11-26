using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Auth.DataAccess.Stores;
using Auth.Helpers;
using Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtIssuerOptions;

        public AuthController(UserManager<AppUser> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtIssuerOptions)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtIssuerOptions = jwtIssuerOptions.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody] CredentialsModel credentials)
        {
            var identity = await GetClaimsIdentity(credentials);
            if (identity == null)
            {
                return new BadRequestResult();
            }

            var jwt = await _jwtFactory.GenerateJwt(identity, credentials.UserName, _jwtFactory, _jwtIssuerOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
            return new OkObjectResult(jwt);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(CredentialsModel credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.UserName) || string.IsNullOrWhiteSpace(credentials.Password))
            {
                return await Task.FromResult<ClaimsIdentity>(null);
            }

            var user = await _userManager.FindByNameAsync(credentials.UserName);
            if (user == null)
            {
                return await Task.FromResult<ClaimsIdentity>(null);
            }

            if (!await _userManager.CheckPasswordAsync(user, credentials.Password))
            {
                return await Task.FromResult<ClaimsIdentity>(null);
            }

            return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(user.UserName, user.Id));
        }
    }
}
