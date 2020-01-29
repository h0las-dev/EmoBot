using EmoBot.Client.Models;
using EmoBot.Client.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmoBot.Client.Helpers.Extensions
{
    public static class TelegramServiceCollectionExtensions
    {
        public static IServiceCollection AddTelegramBot(this IServiceCollection services, IConfiguration configuration)
        {
            var botConfig = configuration.GetSection("Bot");
            services.Configure<BotOptions>(botConfig);

            services.AddSingleton<ITelegramBotService, TelegramBotService>();

            return services;
        }
    }
}
