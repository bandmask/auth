using Microsoft.Extensions.Configuration;

namespace Auth.Settings
{
    public class ConnectionsOptions
    {
        public string MongoDbConnection { get; private set; }

        public ConnectionsOptions(IConfigurationSection section)
        {
            MongoDbConnection = section[nameof(MongoDbConnection)];
        }
    }
}
