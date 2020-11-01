using RakDotNet.IO;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using InfectedRose.Core;

namespace Uchu.World.Utilities
{
    public static class SavePacket
    {
        public static void Write(BitReader reader, string FileName)
        {
            FileStream Bin = File.OpenWrite(FileName);

            Bin.Write(reader.ReadBuffer((uint)reader.BaseStream.Length));
        }

        public static void Write(BitWriter writer, string FileName)
        {
            FileStream Bin = File.OpenWrite(FileName);

            BitReader reader = new BitReader(writer.BaseStream);

            Bin.Write(reader.ReadBuffer((uint)reader.BaseStream.Length));
        }

        public static void WritePacket(Uchu.Core.Packet packet, string FileName)
        {
            using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);

            packet.Serialize(writer);

            FileStream Bin = File.OpenWrite(FileName);

            BitReader reader = new BitReader(writer.BaseStream);

            Bin.Write(reader.ReadBuffer((uint)reader.BaseStream.Length));
        }
    }
}
