using System.IO;
using System.Threading.Tasks;

namespace EmoBot.Client.Services.Converter
{
    public interface IWebpConverter
    {
        Task<FileInfo> ConvertAsync(FileInfo fileInfo);
    }
}
