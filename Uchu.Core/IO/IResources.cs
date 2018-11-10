using System.Threading.Tasks;

namespace Uchu.Core.IO
{
    public interface IResources
    {
        Task<string> ReadTextAsync(string path);
        Task<byte[]> ReadBytesAsync(string path);
    }
}