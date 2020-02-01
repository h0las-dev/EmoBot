using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmoBot.Client.Models
{
    public class Sticker
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UniqueStickerId")]
        public string UniqueStickerId { get; set; }

        [BsonElement("GiphyUrl")]
        public string GiphyUrl { get; set; }
    }
}
