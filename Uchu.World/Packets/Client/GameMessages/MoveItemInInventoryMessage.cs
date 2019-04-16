using System;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class MoveItemInInventoryMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0xE0;

        public long ObjectID;

        public uint Slot;

        public override void Deserialize(BitStream stream)
        {
            stream.ReadBit();
            ObjectID = stream.ReadInt64();
            stream.ReadInt64();
            Slot = stream.ReadUInt32();
        }
    }
}