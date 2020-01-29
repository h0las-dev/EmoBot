using System.Collections.Generic;
using EmoBot.Client.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EmoBot.Client.Services.Data
{
    public class CacheService
    {
        private readonly IMongoCollection<Emoji> _emoji;
        private readonly CacheOptions _cacheOptions;

        public CacheService(IOptions<CacheOptions> cacheOptions)
        {
            _cacheOptions = cacheOptions.Value;

            var client = new MongoClient(_cacheOptions.ConnectionString);
            var database = client.GetDatabase(_cacheOptions.DatabaseName);

            _emoji = database.GetCollection<Emoji>(_cacheOptions.CollectionName);
        }

        public List<Emoji> Get() =>
            _emoji.Find(emoji => true).ToList();

        public Emoji Get(string id) =>
            _emoji.Find<Emoji>(emoji => emoji.Id == id).FirstOrDefault();

        public Emoji GetByValue(string emojiValue) =>
            _emoji.Find<Emoji>(emoji => emoji.EmojiValue == emojiValue).FirstOrDefault();

        public Emoji Create(Emoji emoji)
        {
            _emoji.InsertOne(emoji);
            return emoji;
        }

        public void Update(string id, Emoji emojiIn) =>
            _emoji.ReplaceOne(emoji => emoji.Id == id, emojiIn);

        public void Remove(Emoji emojiIn) =>
            _emoji.DeleteOne(emoji => emoji.Id == emojiIn.Id);

        public void Remove(string id) =>
            _emoji.DeleteOne(emoji => emoji.Id == id);
    }
}
