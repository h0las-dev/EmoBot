using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EmoBot.Client.Services
{
    public interface IUpdateService
    {
        Task Update(Update update);
    }
}
