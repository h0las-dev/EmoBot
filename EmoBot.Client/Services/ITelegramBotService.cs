using System.IO;
using System.Threading.Tasks;

namespace EmoBot.Client.Services
{
    public interface ITelegramBotService
    {
        Task SetWebhookAsync();
        Task DeleteWebhookAsync();
        Task SendMessageAsync(long chatId, int replyToMessageId, string text);
        Task<FileInfo> DownloadFileAsync(string fileId);
    }
}
