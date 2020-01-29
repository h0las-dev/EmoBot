using System.Threading.Tasks;

namespace EmoBot.Client.Services
{
    public interface ITelegramBotService
    {
        Task SetWebhookAsync();
        Task DeleteWebhookAsync();
        Task SendMessage(long chatId, int replyToMessageId, string text);
    }
}
