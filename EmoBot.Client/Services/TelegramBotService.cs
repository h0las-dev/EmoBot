using System;
using System.IO;
using System.Linq;
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

            await _client.SetWebhookAsync(hook);

            _logger.LogInformation(
                LoggingEvents.ConnectToTelegramSuccessful, 
                "Webhook setup was successful via url: {url}", 
                hook);
        }

        public async Task DeleteWebhookAsync()
        {
            await _client.DeleteWebhookAsync();

            _logger.LogInformation(
                LoggingEvents.ConnectToTelegramSuccessful, 
                "Webhook delete was successful");
        }

        public async Task SendMessageAsync(long chatId, int replyToMessageId, string text)
        {
            await _client.SendTextMessageAsync(chatId, text, replyToMessageId: replyToMessageId);
            
            _logger.LogInformation(
                LoggingEvents.MessageSentSuccessfully,
                "Message sent successfully");
        }

        public async Task<FileInfo> DownloadFileAsync(string fileId)
        {
            Telegram.Bot.Types.File file;

            try
            {
                file = await _client.GetFileAsync(fileId);
                _logger.LogInformation($"File received successfully by this id: {fileId}");
            }
            catch (Exception e)
            {
                throw new Exception($"Can not get a file by this id: {fileId}: {e.Message}");
            }

            var fileName = fileId;
            var extension = "." + file.FilePath.Split('.').Last();
            var fileInfo = new FileInfo(fileName + extension);
            var filePath = Constants.ConverterPath + fileInfo.Name;

            try
            {
                await using var saveImageStream = File.Open(filePath, FileMode.Create);
                await _client.DownloadFileAsync(file.FilePath, saveImageStream);
                saveImageStream.Close();
            }
            catch (Exception e)
            {
                throw new Exception($"Can not download file {fileInfo.Name} from telegram: {e.Message}");
            }
            

            return fileInfo;
        }
    }
}
