using System.Threading.Tasks;
using EmoBot.Giphy.API.Models.RequestResults;

namespace EmoBot.Giphy.API.Methods
{
    public interface IGifGetByIdEndpoint
    {
        Task<GetGifRequestResult> GetGifAsync(string gifId, string token);
    }
}
