using System.Threading.Tasks;
using EmoBot.Client.Services.Data;
using EmoBot.Client.Helpers.Extensions;
using EmoBot.Client.Models.RabbitMQ;
using EmoBot.Client.Services.RabbitMQ;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EmoBot.Client.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly ITelegramBotService _bot;
        private readonly CacheService _cacheService;
        private readonly IProduceRabbitMqService _rabbitSendingService;

        public UpdateService(ITelegramBotService bot, CacheService cacheService, 
            IProduceRabbitMqService rabbitSendingService)
        {
            _bot = bot;
            _cacheService = cacheService;
            _rabbitSendingService = rabbitSendingService;
        }

        public async Task Update(Update update)
        {
            var message = update.Message;
            var chatId = message.Chat.Id;
            var replyTo = message.MessageId;

            if (!UserInputIsSticker(message))
            {
                await _bot.SendMessageAsync(chatId, replyTo, "Кажется, это не стикер ;c");

                return;
            }

            await SendResponseToUser(chatId, replyTo, message.Sticker.FileId, message.Sticker.FileUniqueId);
        }

        private async Task SendResponseToUser(long chatId, int replyTo, string fileId, string uniqueFileId)
        {
            var stickerFromCache = _cacheService.GetByUniqueFileId(uniqueFileId);

            if (stickerFromCache != null)
            {
                await _bot.SendMessageAsync(chatId, replyTo, stickerFromCache.GiphyUrl);
            }
            else
            {
                var rabbitMessage = new StickerRabbitDto
                {
                    StickerId = fileId,
                    StickerUniqueId = uniqueFileId,
                    GiphyUrl = "",
                    TelegramChatId = chatId,
                    TelegramMessageId = replyTo
                };

                _rabbitSendingService.Send(
                    rabbitMessage,
                    "telegram.bot",
                    "direct",
                    "converter.endpoint");
            }
        }

        private bool UserInputIsSticker(Message message)
        {
            return message.Type == MessageType.Sticker;
        }
    }
}
