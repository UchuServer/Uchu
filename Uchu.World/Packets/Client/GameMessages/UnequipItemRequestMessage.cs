using System;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class UnequipItemRequestMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x00E9;

        public bool EvenIfDead { get; set; }
        public bool IgnoreCooldown { get; set; }
        public bool OutSuccess { get; set; }
        public long ItemObjectId { get; set; }
        public long ReplacementItemObjectId { get; set; } = -1;

        public override void Deserialize(BitStream stream)
        {
            EvenIfDead = stream.ReadBit();
            IgnoreCooldown = stream.ReadBit();
            OutSuccess = stream.ReadBit();
            ItemObjectId = stream.ReadLong();

            if (stream.ReadBit())
                ReplacementItemObjectId = stream.ReadLong();
        }
    }
}