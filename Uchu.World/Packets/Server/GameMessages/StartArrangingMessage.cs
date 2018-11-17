using System.Numerics;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class StartArrangingMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x0425;

        public bool FirstTime { get; set; } = true;
        public long BuildAreaId { get; set; } = -1;
        public Vector3 StartPosition { get; set; }
        public int SourceBag { get; set; }
        public long SourceObjectId { get; set; }
        public int SourceLOT { get; set; }
        public int SourceType { get; set; }
        public long TargetObjectId { get; set; }
        public int TargetLOT { get; set; }
        public Vector3 TargetPosition { get; set; }
        public int TargetType { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(FirstTime);

            var hasId = BuildAreaId != -1;

            stream.WriteBit(hasId);

            if (hasId)
                stream.WriteLong(BuildAreaId);

            stream.WriteFloat(StartPosition.X);
            stream.WriteFloat(StartPosition.Y);
            stream.WriteFloat(StartPosition.Z);
            stream.WriteInt(SourceBag);
            stream.WriteLong(SourceObjectId);
            stream.WriteInt(SourceLOT);
            stream.WriteInt(SourceType);
            stream.WriteLong(TargetObjectId);
            stream.WriteInt(TargetLOT);
            stream.WriteFloat(TargetPosition.X);
            stream.WriteFloat(TargetPosition.Y);
            stream.WriteFloat(TargetPosition.Z);
            stream.WriteInt(TargetType);
        }
    }
}