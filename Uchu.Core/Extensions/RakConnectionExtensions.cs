using System;
using System.IO;
using RakDotNet;
using RakDotNet.IO;

namespace Uchu.Core
{
    public static class RakConnectionExtensions
    {
        public static void Send(this IRakConnection @this, ISerializable serializable)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this), "Received null connection in send");
            if (serializable == null)
                throw new ArgumentNullException(nameof(serializable), "Received null serializable in send");
            
            Logger.Information($"Sending {serializable}");
            
            using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);
            
            writer.Write(serializable);

            try
            {
                @this.Send(stream.ToArray());
            }
            catch (IOException e)
            {
                Logger.Error(e);
            }
        }

        public static void Send(this IRakConnection @this, MemoryStream stream)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this), "Received null connection in send");
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Received null stream in send");
            @this.Send(stream.ToArray());
        }
    }
}