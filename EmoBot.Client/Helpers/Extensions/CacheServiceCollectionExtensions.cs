using EmoBot.Client.Models;
using EmoBot.Client.Services.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmoBot.Client.Helpers.Extensions
{
    public static class CacheServiceCollectionExtensions
    {
        public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
        {
            var cacheConfig = configuration.GetSection("MongoDbOptions");
            services.Configure<CacheOptions>(cacheConfig);

            services.AddSingleton<CacheService>();

            return services;
        }
    }
}
