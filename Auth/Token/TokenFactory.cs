using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Models;
using Auth.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Token
{
    public interface ITokenFactory
    {
        string CreateToken(JwtIssuerOptions jwtIssuerOptions, string client, AppUser user);
    }

    public class TokenFactory : ITokenFactory
    {
        public string CreateToken(JwtIssuerOptions jwtIssuerOptions, string client, AppUser user)
        {
            var claims = new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }, "Custom");

            var symetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtIssuerOptions.SigningKey));
            var signingCredentials = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(30);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtIssuerOptions.Issuer,
                Audience = client,
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
