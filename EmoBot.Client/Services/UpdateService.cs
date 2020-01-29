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

            if (!UserInputIsEmoji(message))
            {
                await _bot.SendMessage(chatId, replyTo, 
                    "Прости, но я понимаю только стандартные emoji ;c");

                return;
            }

            var emoji = message.Text;

            await SendResponseToUser(chatId, replyTo, emoji);
        }

        private async Task SendResponseToUser(long chatId, int replyTo, string userEmojiInput)
        {
            var emojiFromCache = _cacheService.GetByValue(userEmojiInput);

            if (emojiFromCache != null)
            {
                await _bot.SendMessage(chatId, replyTo, emojiFromCache.GiphyUrl);
            }
            else
            {
                var rabbitMessage = new EmojiRabbitDto
                {
                    EmojiValue = userEmojiInput,
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

        private bool UserInputIsEmoji(Message message)
        {
            return message.Type == MessageType.Text && message.Text.ContainsEmoji();
        }
    }
}
