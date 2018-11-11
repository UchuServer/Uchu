using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class RequestUseMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x016C;

        public bool IsMultiInteract { get; set; }

        public uint MultiInteractId { get; set; }

        public int MultiInteractType { get; set; }

        public long TargetObjectId { get; set; }

        public bool Secondary { get; set; }

        public override void Deserialize(BitStream stream)
        {
            IsMultiInteract = stream.ReadBit();
            MultiInteractId = stream.ReadUInt();
            MultiInteractType = stream.ReadInt();
            TargetObjectId = stream.ReadLong();
            Secondary = stream.ReadBit();
        }
    }
}