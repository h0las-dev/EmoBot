using System.IO;
using System.Threading.Tasks;
using EmoBot.Client.Helpers;

namespace EmoBot.Client.Services.Converter
{
    public class ConverterService : IConverterService
    {
        private readonly ITgsConverter _tgsToGifConverter;
        private readonly IWebpConverter _webpToGifConverter;

        public ConverterService(ITgsConverter tgsToGifConverter, IWebpConverter webpToGifConverter)
        {
            _tgsToGifConverter = tgsToGifConverter;
            _webpToGifConverter = webpToGifConverter;
        }

        public async Task<FileInfo> ConvertAsync(FileInfo fileInfo)
        {

            var fileForReturnInfo = fileInfo.Extension switch
            {
                Constants.TgsExtension => await _tgsToGifConverter.ConvertAsync(fileInfo),
                Constants.WebpExtension => await _webpToGifConverter.ConvertAsync(fileInfo),
                _ => fileInfo,
            };

            return fileForReturnInfo;
        }
    }
}
