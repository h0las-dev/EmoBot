using System.IO;
using System.Threading.Tasks;

namespace EmoBot.Client.Services.Converter
{
    public interface IConverterService
    {
        Task<FileInfo> ConvertAsync(FileInfo fileInfo);
    }
}
