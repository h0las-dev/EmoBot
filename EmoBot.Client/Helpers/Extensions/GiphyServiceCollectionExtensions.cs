using EmoBot.Giphy.API.Methods;
using EmoBot.Giphy.API.Models;
using EmoBot.Giphy.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmoBot.Client.Helpers.Extensions
{
    public static class GiphyServiceCollectionExtensions
    {
        public static IServiceCollection AddGiphy(this IServiceCollection services, IConfiguration configuration)
        {
            var giphyConfig = configuration.GetSection("Giphy");
            services.Configure<GiphyOptions>(giphyConfig);

            services.AddSingleton<IGifGetByIdEndpoint, GifGetByIdEndpoint>();
            services.AddSingleton<IGifUploadEndpoint, GifUploadEndpoint>();
            services.AddSingleton<IGiphyService, GiphyService>();

            return services;
        }
    }
}
