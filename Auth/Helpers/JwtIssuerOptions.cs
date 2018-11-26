using System;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Helpers
{
    public class JwtIssuerOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public DateTime IssuedAt { get; set; }
        public TimeSpan ValidFor { get; set; }
        public DateTime? NotBefore { get; set; }
        public DateTime? Expiration { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
    }
}
