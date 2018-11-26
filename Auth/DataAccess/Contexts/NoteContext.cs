using Auth.Models.Notes;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Auth.DataAccess.Contexts
{
    public class NoteContext
    {
        private readonly IMongoDatabase _database = null;

        public NoteContext()
        {
            var client = new MongoClient("mongodb://mongodb");
            if (client != null)
                _database = client.GetDatabase("notes");
        }

        public IMongoCollection<Note> Notes
        {
            get
            {
                return _database.GetCollection<Note>("Note");
            }
        }
    }
}
