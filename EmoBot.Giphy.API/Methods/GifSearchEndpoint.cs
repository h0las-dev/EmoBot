using System;
using System.Net.Http;
using System.Threading.Tasks;
using EmoBot.Giphy.API.Models;
using Newtonsoft.Json;

namespace EmoBot.Giphy.API.Methods
{
    public class GifSearchEndpoint : IGifSearchEndpoint
    {
        public async Task<GiphySearchRequestResult> GetGifBasedOnSearchEmoji(string emoji, string token, string endpointUrl)
        {
            using var client = new HttpClient();
            var jsonResponse = string.Empty;

            try
            {
                var url = string.Format(endpointUrl, emoji, token);

                var response = await client.GetAsync(url);

                using var content = response.Content;

                jsonResponse = await content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Can not get a gif from Giphy: " + e.Message);
            }


            return JsonConvert.DeserializeObject<GiphySearchRequestResult>(jsonResponse);
        }
    }
}
