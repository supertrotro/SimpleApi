using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Simple.Api.Repository
{
    public class DataItem
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
