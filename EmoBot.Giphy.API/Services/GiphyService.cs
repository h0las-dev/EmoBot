using System.Threading.Tasks;
using EmoBot.Giphy.API.Methods;
using EmoBot.Giphy.API.Models;

namespace EmoBot.Giphy.API.Services
{
    public class GiphyService : IGiphyService
    {
        private readonly IGifSearchEndpoint _gifSearchEndpoint;

        public GiphyService(IGifSearchEndpoint gifSearchEndpoint)
        {
            _gifSearchEndpoint = gifSearchEndpoint;
        }

        public async Task<GiphySearchRequestResult> GetGifBasedOnSearchEmoji(string emoji, string token, string endpointUrl)
        {
            return await _gifSearchEndpoint.GetGifBasedOnSearchEmoji(emoji, token, endpointUrl);
        }
    }
}
