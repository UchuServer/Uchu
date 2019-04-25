using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Uchu.Core.IO
{
    public class AssemblyResources : IResources
    {
        private readonly Assembly _assembly;
        private readonly string _namespace;

        public AssemblyResources(string dll)
        {
            _assembly = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), dll));
            _namespace = Path.GetFileNameWithoutExtension(dll);
        }

        public async Task<string> ReadTextAsync(string path)
        {
            using (var stream = GetStream(path))
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<byte[]> ReadBytesAsync(string path)
        {
            using (var stream = GetStream(path))
            {
                var bytes = new byte[stream.Length];

                await stream.ReadAsync(bytes, 0, (int) stream.Length);

                return bytes;
            }
        }

        public Stream GetStream(string path)
        {
            path = path.Replace('\\', '/');

            var str = new StringBuilder();
            var parts = path.Split('/');

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                if (int.TryParse(part[0].ToString(), out _))
                {
                    str.Append('_');
                }

                str.Append(part);

                if (i + 1 != parts.Length)
                {
                    str.Append('.');
                }
            }

            return _assembly.GetManifestResourceStream($"{_namespace}.{str}");
        }
    }
}