using System.Numerics;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class DoneArrangingMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0427;

        public int NewSourceBag { get; set; }
        public long NewSourceObjectId { get; set; }
        public int NewSourceLOT { get; set; }
        public int NewSourceType { get; set; }

        public long NewTargetObjectId { get; set; }
        public int NewTargetLOT { get; set; }
        public int NewTargetType { get; set; }
        public Vector3 NewTargetPosition { get; set; }

        public int OldSourceBag { get; set; }
        public long OldItemObjectId { get; set; }
        public int OldItemLOT { get; set; }
        public int OldItemType { get; set; }

        public override void Deserialize(BitStream stream)
        {
            NewSourceBag = stream.ReadInt();
            NewSourceObjectId = stream.ReadLong();
            NewSourceLOT = stream.ReadInt();
            NewSourceType = stream.ReadInt();

            NewTargetObjectId = stream.ReadLong();
            NewTargetLOT = stream.ReadInt();
            NewTargetType = stream.ReadInt();
            NewTargetPosition = new Vector3
            {
                X = stream.ReadFloat(),
                Y = stream.ReadFloat(),
                Z = stream.ReadFloat()
            };

            OldSourceBag = stream.ReadInt();
            OldItemObjectId = stream.ReadLong();
            OldItemLOT = stream.ReadInt();
            OldItemType = stream.ReadInt();
        }
    }
}