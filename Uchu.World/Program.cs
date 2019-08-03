using System.IO;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var stream = new MemoryStream();

            using (var writer = new BitWriter(stream))
            {
                writer.WriteBit(true);
                writer.WriteBit(true);
                writer.WriteBit(false);
                writer.WriteBit(true);
                writer.WriteBit(false);
                writer.Write(42.42412f);
                writer.WriteBit(false);
                writer.Write(420);
            }

            using (var reader = new BitReader(stream))
            {
                var bit = reader.ReadBit();
                var bit1 = reader.ReadBit();
                var bit2 = reader.ReadBit();
                var bit3 = reader.ReadBit();
                var bit4 = reader.ReadBit();
                var mol = reader.Read<float>();
                var bit5 = reader.ReadBit();
                var f = reader.Read<int>();
                
                Logger.Information($"{bit} | {bit1} | {bit2} | {bit3} | {bit4} | {mol} | {bit5} | {f}");
            }
            
            var server = new WorldServer(2003);

            server.Start();
        }
    }
}