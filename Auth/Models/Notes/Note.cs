using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.Models.Notes
{
    public class Note
    {
        [BsonId]
        // standard BSonId generated by MongoDb
        public ObjectId InternalId { get; set; }

        // external Id, easier to reference: 1,2,3 or A, B, C etc.
        public string Id { get; set; }

        public string Body { get; set; } = string.Empty;

        [BsonDateTimeOptions]
        // attribute to gain control on datetime serialization
        public DateTime UpdatedOn { get; set; } = DateTime.Now;

        public int UserId { get; set; } = 0;
    }
}
