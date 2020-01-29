using System.Threading.Tasks;
using EmoBot.Giphy.API.Models;

namespace EmoBot.Giphy.API.Methods
{
    public interface IGifSearchEndpoint
    {
        Task<GiphySearchRequestResult> GetGifBasedOnSearchEmoji(string emoji, string token, string endpointUrl);
    }
}
