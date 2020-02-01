using System;
using System.IO;
using System.Threading.Tasks;
using EmoBot.Client.Helpers;
using ImageMagick;
using Microsoft.Extensions.Logging;

namespace EmoBot.Client.Services.Converter
{
    public class WebpConverter : IWebpConverter
    {
        private readonly ILogger _logger;

        public WebpConverter(ILogger<WebpConverter> logger)
        {
            _logger = logger;
        }

        public async Task<FileInfo> ConvertAsync(FileInfo fileInfo)
        {
            var webpFilePath = Constants.ConverterPath + fileInfo.Name;
            var gifFilePath = Constants.ConverterPath + fileInfo.Name.Replace(
                                  Constants.WebpExtension, 
                                  Constants.GifExtension);
            var pngFilePath = Constants.ConverterPath + fileInfo.Name.Replace(
                                  Constants.WebpExtension,
                                  Constants.PngExtension);
            try
            {
                await Task.Run(() =>
                {
                    // Здесь я промежуточно конвертирую в .png, чтобы создать анимированную .gif,
                    // т.к. Giphy не принимает статичные изображения
                    using (var image = new MagickImage(webpFilePath))
                    {
                        image.Write(pngFilePath);
                    }

                    using var collection = new MagickImageCollection
                    {
                        pngFilePath
                    };

                    collection[0].AnimationDelay = 100;
                    collection.Add(pngFilePath);
                    collection[1].AnimationDelay = 100; 
                    collection[1].Flop();
                    collection.Optimize();
                    collection.Write(gifFilePath);
                });

                var fileForReturnInfo = new FileInfo(fileInfo.Name.Replace(Constants.WebpExtension, Constants.GifExtension));

                _logger.LogInformation($"File {fileInfo.Name} Converted Successfully");

                return fileForReturnInfo;
            }
            catch (Exception e)
            {
                throw new Exception($"File {fileInfo.Name} Converted Failed: {e.Message}");
            }
        }
    }
}
