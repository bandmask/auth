using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Auth.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthController : Controller
    {
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
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, credentials.Password, true, false);
                if (result.Succeeded)
                {
                    return Ok($"Bearer { TokenFactory.CreateToken(_jwtIssuerOptions, user.UserName, user.Id) }");
                }
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CredentialsModel credentials)
        {
            var user = new AppUser { UserName = credentials.UserName, Email = credentials.Email };
            var result = await _userManager.CreateAsync(user, credentials.Password);
            if (result.Succeeded)
            {
                // Not a good solution - prefer confirmation mail or the sorts - just for debugging purposes
                var signedInUser = await _userManager.FindByEmailAsync(credentials.Email);
                if (signedInUser != null)
                {
                    return Ok($"Bearer { TokenFactory.CreateToken(_jwtIssuerOptions, signedInUser.UserName, signedInUser.Id) }");
                }
            }

            return BadRequest(new { StatusCode = 500 });
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

        private static class TokenFactory
        {
            public static string CreateToken(JwtIssuerOptions jwtIssuerOptions, string userName, string userId)
            {
                var claims = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "Custom");

                var symetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtIssuerOptions.SigningKey));
                var signingCredentials = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);

                var expires = DateTime.Now.AddDays(30);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = jwtIssuerOptions.Issuer,
                    Audience = jwtIssuerOptions.Audience,
                    Subject = claims,
                    NotBefore = DateTime.Now,
                    Expires = expires,
                    IssuedAt = DateTime.Now,
                    SigningCredentials = signingCredentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
        }
    }
}
