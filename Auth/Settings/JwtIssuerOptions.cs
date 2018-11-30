using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Auth.Settings
{
    public class JwtIssuerOptions
    {
        public List<string> Audiences { get; set; }
        public string Issuer { get; set; }
        public string Authority { get; set; }
        public string SigningKey { get; set; }

        public JwtIssuerOptions() { }

        public JwtIssuerOptions(IConfigurationSection section)
        {
            Audiences = section.GetSection(nameof(Audiences))
                .GetChildren()
                .Select(audience => audience.Value)
                .ToList();

            Issuer = section[nameof(Issuer)];
            Authority = section[nameof(Authority)];
            SigningKey = section[nameof(SigningKey)];
        }
    }
}
