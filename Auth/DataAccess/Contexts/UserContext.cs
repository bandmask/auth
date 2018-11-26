using Auth.Models;
using MongoDB.Driver;

namespace Auth.DataAccess.Contexts
{
    public class UserContext
    {
        private readonly IMongoDatabase _database = null;

        public UserContext()
        {
            var client = new MongoClient("mongodb://mongodb");
            if (client != null)
            {
                _database = client.GetDatabase("users");
            }
        }

        public IMongoCollection<AppUser> Users => _database.GetCollection<AppUser>("Users");
    }
}
