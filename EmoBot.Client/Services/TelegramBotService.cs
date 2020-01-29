using System;
using System.Threading.Tasks;
using EmoBot.Client.Helpers;
using EmoBot.Client.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace EmoBot.Client.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ILogger _logger;
        private readonly BotOptions _botOptions;
        private TelegramBotClient _client;

        public TelegramBotService(ILogger<TelegramBotService> logger, IOptions<BotOptions> botOptions)
        {
            _logger = logger;
            _botOptions = botOptions.Value;

            _client = new TelegramBotClient(_botOptions.Token);
        }

        public async Task SetWebhookAsync()
        {
            _client = new TelegramBotClient(_botOptions.Token);
            var hook = string.Format(_botOptions.Url, "api/updates");

            try
            {
                await _client.SetWebhookAsync(hook);
                _logger.LogInformation(
                    LoggingEvents.ConnectToTelegramSuccessful, 
                    "Webhook setup was successful via url: {url}", 
                    hook);
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    LoggingEvents.ConnectToTelegramFailed, e, 
                    "Webhook setup failed via url: {url}", 
                    hook);
            }
        }

        public async Task DeleteWebhookAsync()
        {
            try
            {
                await _client.DeleteWebhookAsync();
                _logger.LogInformation(
                    LoggingEvents.ConnectToTelegramSuccessful, 
                    "Webhook delete was successful");
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    LoggingEvents.ConnectToTelegramFailed, e, 
                    "Webhook delete failed");
            }
        }

        public async Task SendMessage(long chatId, int replyToMessageId, string text)
        {
            try
            {
                await _client.SendTextMessageAsync(chatId, text, replyToMessageId: replyToMessageId);
                _logger.LogInformation(
                    LoggingEvents.MessageSentSuccessfully,
                    "Message sent successfully");
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    LoggingEvents.MessageSentFailed, e,
                    "Message sent failed");
            }
        }
    }
}
