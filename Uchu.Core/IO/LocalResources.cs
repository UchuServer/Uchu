using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uchu.Core.IO
{
    public class LocalResources : IFileResources
    {
        private readonly Configuration _config;

        public string RootPath => _config.ResourcesConfiguration.GameResourceFolder;

        public LocalResources(Configuration config)
        {
            _config = config;
        }

        public async Task<string> ReadTextAsync(string path)
        {
            await using var stream = GetStream(path);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }

        public async Task<byte[]> ReadBytesAsync(string path)
        {
            await using var stream = GetStream(path);
            var bytes = new byte[stream.Length];

            await stream.ReadAsync(bytes, 0, (int) stream.Length).ConfigureAwait(false);

            return bytes;
        }

        public byte[] ReadBytes(string path)
        {
            using var stream = GetStream(path);
            var bytes = new byte[stream.Length];

            stream.Read(bytes, 0, (int) stream.Length);

            return bytes;
        }

        public IEnumerable<string> GetAllFilesWithExtension(string extension)
        {
            var files = Directory.GetFiles(
                _config.ResourcesConfiguration.GameResourceFolder,
                $"*.{extension}",
                SearchOption.AllDirectories
            );

            for (var i = 0; i < files.Length; i++)
            {
                var parts = files[i].Split('/').Reverse().ToArray();
                var final = parts.TakeWhile(part => part != "res").Aggregate("",
                    (current, part) => $"{part}/{current}"
                );

                var strBuilder = new StringBuilder(final);
                strBuilder.Length--;

                files[i] = strBuilder.ToString();
            }

            return files;
        }
        
        public IEnumerable<string> GetAllFilesWithExtension(string location, string extension)
        {
            var files = Directory.GetFiles(
                Path.Combine(_config.ResourcesConfiguration.GameResourceFolder, location),
                $"*.{extension}",
                SearchOption.TopDirectoryOnly
            );

            return files;
        }
        
        public Stream GetStream(string path)
        {
            path = path.Replace('\\', '/').ToLower();

            return File.OpenRead(Path.Combine(_config.ResourcesConfiguration.GameResourceFolder, path));
        }
    }
}