using System.Numerics;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class StartBuildingMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0421;

        public bool FirstTime { get; set; }
        public bool Success { get; set; }
        public int SourceBag { get; set; }
        public long SourceObjectId { get; set; }
        public int SourceLOT { get; set; }
        public int SourceType { get; set; }
        public long TargetObjectId { get; set; }
        public int TargetLOT { get; set; }
        public Vector3 TargetPosition { get; set; }
        public int TargetType { get; set; }

        public override void Deserialize(BitStream stream)
        {
            FirstTime = stream.ReadBit();
            Success = stream.ReadBit();
            SourceBag = stream.ReadInt();
            SourceObjectId = stream.ReadLong();
            SourceLOT = stream.ReadInt();
            SourceType = stream.ReadInt();
            TargetObjectId = stream.ReadLong();
            TargetLOT = stream.ReadInt();
            TargetPosition = new Vector3
            {
                X = stream.ReadFloat(),
                Y = stream.ReadFloat(),
                Z = stream.ReadFloat()
            };
            TargetType = stream.ReadInt();
        }
    }
}