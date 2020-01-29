using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmoBot.Client.Models
{
    public class Emoji
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("EmojiValue")]
        public string EmojiValue { get; set; }

        [BsonElement("GiphyUrl")]
        public string GiphyUrl { get; set; }
    }
}
