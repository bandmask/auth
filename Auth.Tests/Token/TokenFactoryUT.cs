using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Auth.Models;
using Auth.Settings;
using Auth.Token;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace Auth.Tests.Token
{
    public class TokenFactoryUT
    {
        private string _token;
        private JwtIssuerOptions _jwtIssuerOptions;

        [SetUp]
        public void Setup()
        {
            _jwtIssuerOptions = GetJwtSignerOptions();
            _token = TokenFactory.CreateToken(_jwtIssuerOptions, GetClient(), GetAppUser());
        }

        [Test]
        public void CreateToken_Should_Return_Token()
        {
            Assert.NotNull(_token);
        }

        [Test]
        public void When_params_valid_Token_should_validate()
        {
            object error = null;
            var tokenValidationParameters = GetValidationParameters(_jwtIssuerOptions);

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            try
            {
                handler.ValidateToken(_token, tokenValidationParameters, out var _);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            Assert.IsNull(error);
        }

        [Test]
        public void When_params_invalid_Token_should_NOT_validate()
        {
            object error = null;
            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            var invalidParameters = GetInvalidParameters(GetValidationParameters(_jwtIssuerOptions));

            try
            {
                handler.ValidateToken(_token, invalidParameters, out var _);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            Assert.IsNotNull(error);
        }

        private string GetClient()
        {
            return "ropr";
        }

        private AppUser GetAppUser()
        {
            var appUser = new AppUser
            {
                Id = "123456",
                UserName = "John Doe"
            };
            return appUser;
        }

        private JwtIssuerOptions GetJwtSignerOptions()
        {
            var signerOptions = new JwtIssuerOptions
            {
                Issuer = "ropr.se",
                Audiences = new List<string> { "ropr", "daysofrum" },
                Authority = "authority-ropr-se",
                SigningKey = "MTE0NEQzMThDOEI4RDVBM0E3OUJBNEIxQTI3QzM="
            };
            return signerOptions;
        }

        private TokenValidationParameters GetValidationParameters(JwtIssuerOptions jwtSignerOptions)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSignerOptions.Issuer,
                ValidAudiences = jwtSignerOptions.Audiences,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSignerOptions.SigningKey)),
                ClockSkew = TimeSpan.Zero
            };

            return tokenValidationParameters;
        }

        private TokenValidationParameters GetInvalidParameters(TokenValidationParameters tokenValidationParameters)
        {
            var copy = tokenValidationParameters.Clone();

            copy.ValidIssuer = "some-invalid-issuer";
            copy.ValidAudience = "some-invalid-audience";
            copy.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SOME_INVALID_KEY"));

            return copy;
        }
    }
}
