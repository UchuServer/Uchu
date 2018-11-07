using System.Threading.Tasks;

namespace Uchu.Core
{
    public interface IResources
    {
        Task<string> ReadTextAsync(string path);
        Task<byte[]> ReadBytesAsync(string path);
    }
}