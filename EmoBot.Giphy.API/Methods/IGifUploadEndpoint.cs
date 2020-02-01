using System.Threading.Tasks;
using EmoBot.Giphy.API.Models.RequestResults;

namespace EmoBot.Giphy.API.Methods
{
    public interface IGifUploadEndpoint
    {
        Task<UploadRequestResult> UploadStickerAsync(byte[] file, string token);
    }
}
