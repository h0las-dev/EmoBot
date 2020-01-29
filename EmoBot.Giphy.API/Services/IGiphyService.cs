using System.Threading.Tasks;
using EmoBot.Giphy.API.Models;

namespace EmoBot.Giphy.API.Services
{
    public interface IGiphyService
    {
        Task<GiphySearchRequestResult> GetGifBasedOnSearchEmoji(string emoji, string token, string endpointUrl);
    }
}
