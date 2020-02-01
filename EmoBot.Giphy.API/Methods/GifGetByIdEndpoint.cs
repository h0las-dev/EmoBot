using System.Net.Http;
using System.Threading.Tasks;
using EmoBot.Giphy.API.Models.RequestResults;
using Newtonsoft.Json;

namespace EmoBot.Giphy.API.Methods
{
    public class GifGetByIdEndpoint : IGifGetByIdEndpoint
    {
        private readonly string _baseUrl = @"http://api.giphy.com/v1/gifs/{0}?api_key={1}";

        public async Task<GetGifRequestResult> GetGifAsync(string gifId, string token)
        {
            var url = string.Format(_baseUrl, gifId, token);

            using var client = new HttpClient();

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Can not get a gif with id: {gifId}");
            }

            var responseData = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<GetGifRequestResult>(responseData);
        }
    }
}
