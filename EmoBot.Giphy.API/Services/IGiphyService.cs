using System.Threading.Tasks;
using EmoBot.Giphy.API.Models.RequestResults;

namespace EmoBot.Giphy.API.Services
{
    public interface IGiphyService
    {
        Task<UploadRequestResult> UploadFileAsync(byte[] file);
        Task<GetGifRequestResult> GetGifAsync(string gifId);
    }
}
