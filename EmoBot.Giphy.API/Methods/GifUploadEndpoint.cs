using System;
using System.Net.Http;
using System.Threading.Tasks;
using EmoBot.Giphy.API.Models.RequestResults;
using Newtonsoft.Json;

namespace EmoBot.Giphy.API.Methods
{
    public class GifUploadEndpoint : IGifUploadEndpoint
    {
        private readonly string _baseUrl = @"http://upload.giphy.com/v1/gifs?api_key={0}";

        public async Task<UploadRequestResult> UploadStickerAsync(byte[] file, string token)
        {
            var url = string.Format(_baseUrl, token);

            var uploadContent = new ByteArrayContent(file);
            var tagContent = new StringContent("animated");

            using var client = new HttpClient();

            var formData = new MultipartFormDataContent
            {
                {uploadContent, "file", "file"}, 
                {tagContent, "tags"}
            };

            var response = await client.PostAsync(url, formData);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Upload file to Giphy failed");
            }

            var responseData = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<UploadRequestResult>(responseData);
        }
    }
}
