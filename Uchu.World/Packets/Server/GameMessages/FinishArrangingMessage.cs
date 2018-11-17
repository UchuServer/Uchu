using System.Numerics;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class FinishArrangingMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x0426;

        public long BuildAreaId { get; set; } = -1;

        public int NewSourceBag { get; set; }
        public long NewSourceObjectId { get; set; }
        public int NewSourceLOT { get; set; } = -1;
        public int NewSourceType { get; set; }

        public long NewTargetObjectId { get; set; }
        public int NewTargetLOT { get; set; } = -1;
        public int NewTargetType { get; set; }
        public Vector3 NewTargetPosition { get; set; } = Vector3.Zero;

        public int OldSourceBag { get; set; }
        public long OldItemObjectId { get; set; }
        public int OldItemLOT { get; set; } = -1;
        public int OldItemType { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            var hasId = BuildAreaId != -1;

            stream.WriteBit(hasId);

            if (hasId)
                stream.WriteLong(BuildAreaId);

            stream.WriteInt(NewSourceBag);
            stream.WriteLong(NewSourceObjectId);
            stream.WriteInt(NewSourceLOT);
            stream.WriteInt(NewSourceType);
            stream.WriteLong(NewTargetObjectId);
            stream.WriteInt(NewTargetLOT);
            stream.WriteInt(NewTargetType);
            stream.WriteFloat(NewTargetPosition.X);
            stream.WriteFloat(NewTargetPosition.Y);
            stream.WriteFloat(NewTargetPosition.Z);
            stream.WriteInt(OldSourceBag);
            stream.WriteLong(OldItemObjectId);
            stream.WriteInt(OldItemLOT);
            stream.WriteInt(OldItemType);
        }
    }
}