using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Uchu.Core.Config;
using Uchu.Core.Resources;

namespace Uchu.Core.IO
{
    public class LocalResources : IFileResources
    {
        private readonly UchuConfiguration _config;

        public string RootPath => _config.ResourcesConfiguration.GameResourceFolder;

        public LocalResources(UchuConfiguration config)
        {
            _config = config;
        }

        public async Task<string> ReadTextAsync(string path)
        {
            await using var stream = GetStream(path);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }

        [SuppressMessage("ReSharper", "CA2000")]
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
            
            var folder = new Uri(RootPath);

            for (var i = 0; i < files.Length; i++)
            {
                var file = new Uri(files[i]);

                var final = Uri.UnescapeDataString(
                    folder.MakeRelativeUri(file)
                        .ToString()
                        .Replace('\\', '/')
                );

                files[i] = $"../{final}";
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
        
        [SuppressMessage("ReSharper", "CA1304")]
        public Stream GetStream(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), 
                    ResourceStrings.LocalResources_GetStream_PathNullException);
            
            path = path.Replace('\\', '/').ToLower();

            return File.OpenRead(Path.Combine(RootPath, path));
        }
    }
}