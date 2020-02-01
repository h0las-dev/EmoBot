using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using EmoBot.Client.Helpers;
using Microsoft.Extensions.Logging;

namespace EmoBot.Client.Services.Converter
{
    public class TgsConverter : ITgsConverter
    {
        private readonly ILogger _logger;

        public TgsConverter(ILogger<TgsConverter> logger)
        {
            _logger = logger;
        }

        public async Task<FileInfo> ConvertAsync(FileInfo fileInfo)
        {
            var npmProcessInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                RedirectStandardInput = true,
                WorkingDirectory = @"Services/Converter/JsConverter"
            };

            try
            {
                await Task.Run(() =>
                {
                    var npmProcess = Process.Start(npmProcessInfo);

                    if (npmProcess == null)
                    {
                        throw new Exception("Can not start the npm process");
                    }

                    npmProcess.StandardInput.WriteLine($"npm start {fileInfo.Name} & exit");
                    npmProcess.WaitForExit();
                });

                _logger.LogInformation($"File {fileInfo.Name} Converted Successfully");

                return CleanFileName(fileInfo);
            }
            catch (Exception e)
            {
                throw new Exception($"File {fileInfo.Name} Converted Failed: {e.Message}");
            }
        }

        // Нужно убрать лишнее расширение у файла, иначе получится что-то типа img.tgs.gif
        private FileInfo CleanFileName(FileInfo tgsFile)
        {
            // .tgs.gif
            var currentName = tgsFile.Name + Constants.GifExtension;
            // .gif
            var newName =  tgsFile.Name.Replace(Constants.TgsExtension, string.Empty) + Constants.GifExtension;

            File.Move(
                Constants.ConverterPath + currentName, 
                Constants.ConverterPath + newName);

            return new FileInfo(newName);
        }
    }
}
