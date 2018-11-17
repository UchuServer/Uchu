using System;
using RakDotNet;
using Uchu.Core;
using Uchu.Core.Collections;

namespace Uchu.World
{
    public class DetailedUserInfoPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;
        public override uint PacketId => 0x04;

        /*public bool Compressed { get; set; } = true;
        public uint UncompressedSize { get; set; }
        public byte[] Data { get; set; }*/

        public LegoDataDictionary LDF { get; set; }

        public override void Serialize(BitStream stream)
        {
            base.Serialize(stream);

            stream.WriteLDFCompressed(LDF);

            /*stream.WriteUInt((uint) (Data.Length + (Compressed ? 9 : 1)));
            stream.WriteByte((byte) (Compressed ? 1 : 0));

            if (Compressed)
            {
                stream.WriteUInt(UncompressedSize);
                stream.WriteUInt((uint) Data.Length);
            }

            stream.Write(Data);*/
        }
    }
}