using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EmoBot.Giphy.API.Methods;
using EmoBot.Giphy.API.Models;
using EmoBot.Giphy.API.Models.RequestResults;

namespace EmoBot.Giphy.API.Services
{
    public class GiphyService : IGiphyService
    {
        private readonly IGifUploadEndpoint _gifUploadEndpoint;
        private readonly IGifGetByIdEndpoint _gifGetEndpoint;
        private readonly GiphyOptions _giphyOptions;
        public GiphyService(IGifUploadEndpoint gifUploadEndpoint, IGifGetByIdEndpoint gifGetEndpoint, 
            IOptions<GiphyOptions> giphyOptions)
        {
            _giphyOptions = giphyOptions.Value;

            _gifUploadEndpoint = gifUploadEndpoint;
            _gifGetEndpoint = gifGetEndpoint;
        }

        public async Task<UploadRequestResult> UploadFileAsync(byte[] file)
        {
            return await _gifUploadEndpoint.UploadStickerAsync(file, _giphyOptions.Token);
        }

        public async Task<GetGifRequestResult> GetGifAsync(string gifId)
        {
            return await _gifGetEndpoint.GetGifAsync(gifId, _giphyOptions.Token);
        }
    }
}
