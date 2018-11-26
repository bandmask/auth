using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.Models;
using Newtonsoft.Json;

namespace Auth.Helpers
{
    public interface IJwtFactory
    {
        ClaimsIdentity GenerateClaimsIdentity(string userName, string userId);
        Task<string> GenerateJwt(ClaimsIdentity identity, string userName, IJwtFactory jwtFactory, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings);
    }

    public class JwtFactory : IJwtFactory
    {
        public ClaimsIdentity GenerateClaimsIdentity(string userName, string userId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> GenerateJwt(ClaimsIdentity identity, string userName, IJwtFactory jwtFactory, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings)
        {
            var response = new
            {
                id = identity.Claims.Single(c => c.Type == "id").Value,
                auth_token = await GenerateEncodedToken(userName, identity, jwtOptions),
                expires_in = (int) jwtOptions.ValidFor.TotalSeconds
            };

            return JsonConvert.SerializeObject(response, serializerSettings);
        }

        private async Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity, JwtIssuerOptions jwtOptions)
        {
            var claims = new []
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await Task.FromResult(Guid.NewGuid().ToString())),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst("rol"),
                identity.FindFirst("id")
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                notBefore: jwtOptions.NotBefore,
                expires: jwtOptions.Expiration,
                signingCredentials: jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        private static long ToUnixEpochDate(DateTime date) => (long) Math.Round((date.ToUniversalTime() -
                new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
            .TotalSeconds);
    }
}
