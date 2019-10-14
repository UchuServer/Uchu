using System.IO;
using RakDotNet;
using RakDotNet.IO;

namespace Uchu.Core
{
    public static class RakConnectionExtensions
    {
        public static void Send(this IRakConnection @this, ISerializable serializable)
        {
            Logger.Information($"Sending {serializable}");
            
            using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);
            
            writer.Write(serializable);

            @this.Send(stream.ToArray());
        }

        public static void Send(this IRakConnection @this, MemoryStream stream)
        {
            @this.Send(stream.ToArray());
        }
    }
}