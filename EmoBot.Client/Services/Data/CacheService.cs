using System.Collections.Generic;
using EmoBot.Client.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EmoBot.Client.Services.Data
{
    public class CacheService
    {
        private readonly IMongoCollection<Sticker> _stickers;
        private readonly CacheOptions _cacheOptions;

        public CacheService(IOptions<CacheOptions> cacheOptions)
        {
            _cacheOptions = cacheOptions.Value;

            var client = new MongoClient(_cacheOptions.ConnectionString);
            var database = client.GetDatabase(_cacheOptions.DatabaseName);

            _stickers = database.GetCollection<Sticker>(_cacheOptions.CollectionName);
        }

        public List<Sticker> Get() =>
            _stickers.Find(sticker => true).ToList();

        public Sticker Get(string id) =>
            _stickers.Find<Sticker>(sticker => sticker.Id == id).FirstOrDefault();

        public Sticker GetByUniqueFileId(string uniqueId) =>
            _stickers.Find<Sticker>(sticker => sticker.UniqueStickerId == uniqueId).FirstOrDefault();

        public Sticker Create(Sticker sticker)
        {
            _stickers.InsertOne(sticker);
            return sticker;
        }

        public void Update(string id, Sticker stickerIn) =>
            _stickers.ReplaceOne(sticker => sticker.Id == id, stickerIn);

        public void Remove(Sticker stickerIn) =>
            _stickers.DeleteOne(sticker => sticker.Id == stickerIn.Id);

        public void Remove(string id) =>
            _stickers.DeleteOne(sticker => sticker.Id == id);
    }
}
