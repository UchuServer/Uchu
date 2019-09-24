using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Uchu.Core.IO
{
    public interface IFileResources
    {
        Task<string> ReadTextAsync(string path);

        Task<byte[]> ReadBytesAsync(string path);

        byte[] ReadBytes(string path);

        IEnumerable<string> GetAllFilesWithExtension(string extension);

        Stream GetStream(string path);
    }
}