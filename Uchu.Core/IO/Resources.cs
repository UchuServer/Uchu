using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uchu.Core.IO
{
    public static class Resources
    {
        public static async Task<string> ReadTextAsync(string path)
        {
            using (var stream = GetStream(path))
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static async Task<byte[]> ReadBytesAsync(string path)
        {
            using (var stream = GetStream(path))
            {
                var bytes = new byte[stream.Length];

                await stream.ReadAsync(bytes, 0, (int) stream.Length);

                return bytes;
            }
        }
        
        public static byte[] ReadBytes(string path)
        {
            using (var stream = GetStream(path))
            {
                var bytes = new byte[stream.Length];

                stream.Read(bytes, 0, (int) stream.Length);

                return bytes;
            }
        }

        public static string[] GetAllFilesWithExtenstion(string extenstion)
        {
            var files = Directory.GetFiles(
                Configuration.Singleton.ResourcesConfiguration.GameResourceFolder,
                $"*.{extenstion}",
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
        
        private static Stream GetStream(string path)
        {
            path = path.Replace('\\', '/').ToLower();

            return File.OpenRead(Path.Combine(Configuration.Singleton.ResourcesConfiguration.GameResourceFolder, path));
        }
    }
}