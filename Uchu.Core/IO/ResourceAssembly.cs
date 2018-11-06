using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Uchu.Core
{
    public class ResourceAssembly
    {
        private readonly Assembly _assembly;
        private readonly string _namespace;

        public ResourceAssembly(string dll)
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

        public Stream GetStream(string path)
        {
            path = path.Replace('/', '.').Replace('\\', '.').Replace('-', '_');

            return _assembly.GetManifestResourceStream($@"{_namespace}.{path}");
        }
    }
}