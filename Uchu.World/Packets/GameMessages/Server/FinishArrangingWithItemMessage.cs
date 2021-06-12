using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct FinishArrangingWithItemMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.FinishArrangingWithItem;
        [Default]
        public GameObject BuildArea { get; set; }
        public int NewSourceBag { get; set; }
        public GameObject NewSource { get; set; }
        public Lot NewSourceLot { get; set; }
        public GameObject NewTarget { get; set; }
        public Lot NewTargetLot { get; set; }
        public int NewTargetType { get; set; }
        public Vector3 NewTargetPosition { get; set; }
        public int OldItemBag { get; set; }
        public GameObject OldItem { get; set; }
        public Lot OldItemLot { get; set; }
        public int OldItemType { get; set; }
    }
}