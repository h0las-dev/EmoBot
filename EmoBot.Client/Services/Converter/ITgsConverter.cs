using System.IO;
using System.Threading.Tasks;

namespace EmoBot.Client.Services.Converter
{
    public interface ITgsConverter
    {
        Task<FileInfo> ConvertAsync(FileInfo fileInfo);
    }
}
