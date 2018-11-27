using Microsoft.IdentityModel.Tokens;

namespace Auth.Models
{
    public class JwtIssuerOptions
    {
        public string Issuer { get; set; }
        public string Authority { get; set; }
        public string Audience { get; set; }
        public string SigningKey { get; set; }
        public string SigningDecryption { get; set; }
    }
}
