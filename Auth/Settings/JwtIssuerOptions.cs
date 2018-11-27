using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Auth.Settings
{
    public class JwtIssuerOptions
    {
        public List<string> Audiences { get; private set; }
        public string Issuer { get; private set; }
        public string Authority { get; private set; }
        public string SigningKey { get; private set; }
        public string SigningDecryption { get; private set; }

        public JwtIssuerOptions(IConfigurationSection section)
        {
            Audiences = section.GetSection(nameof(Audiences))
                .GetChildren()
                .Select(audience => audience.Value)
                .ToList();

            Issuer = section[nameof(Issuer)];
            Authority = section[nameof(Authority)];
            SigningKey = section[nameof(SigningKey)];
            SigningDecryption = section[nameof(SigningDecryption)];
        }
    }
}
