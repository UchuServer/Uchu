using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Uchu.Core.IO
{
    public class AssemblyResources
    {
        private readonly Assembly _assembly;
        public readonly string Namespace;

        public AssemblyResources(string dll)
        {
            _assembly = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), dll));
            Namespace = Path.GetFileNameWithoutExtension(dll);
        }

        public async Task<string> ReadTextAsync(string path)
        {
            using (var stream = GetStream(path))
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<byte[]> ReadBytesAsync(string path, bool includeNameSpace = true)
        {
            using (var stream = GetStream(path, includeNameSpace))
            {
                var bytes = new byte[stream.Length];

                await stream.ReadAsync(bytes, 0, (int) stream.Length);

                return bytes;
            }
        }
        
        public byte[] ReadBytes(string path, bool includeNamespace = true)
        {
            using (var stream = GetStream(path, includeNamespace))
            {
                var bytes = new byte[stream.Length];

                stream.Read(bytes, 0, (int) stream.Length);

                return bytes;
            }
        }

        public Stream GetStream(string path, bool includeNamespace = true)
        {
            path = path.Replace('/', '.').Replace('\\', '.');
            
            return _assembly.GetManifestResourceStream(includeNamespace ? $@"{Namespace}.{path}" : path);
        }

        public IEnumerable<string> GetAllPaths()
        {
            return _assembly.GetManifestResourceNames();
        }
    }
}