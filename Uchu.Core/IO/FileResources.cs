using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Uchu.Core.IO
{
    public class FileResources : IResources
    {
        public static readonly string AssemblyDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

        private readonly string _dir;

        public FileResources(string dir = null)
        {
            _dir = dir ?? AssemblyDirectory;
        }

        public async Task<string> ReadTextAsync(string path)
        {
            using (var stream = File.OpenRead(Path.Combine(_dir, path)))
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<byte[]> ReadBytesAsync(string path)
        {
            using (var stream = File.OpenRead(Path.Combine(_dir, path)))
            {
                var data = new byte[stream.Length];

                await stream.ReadAsync(data, 0, (int) stream.Length);

                return data;
            }
        }
    }
}