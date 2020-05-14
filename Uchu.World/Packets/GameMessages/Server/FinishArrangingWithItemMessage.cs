using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class FinishArrangingWithItemMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.FinishArrangingWithItem;
        
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
        
        public override void SerializeMessage(BitWriter writer)
        {
            if (writer.Flag(BuildArea != default))
            {
                writer.Write(BuildArea);
            }

            writer.Write(NewSourceBag);
            writer.Write(NewSource);
            writer.Write(NewSourceLot);
            
            writer.Write(NewTarget);
            writer.Write(NewTargetLot);
            writer.Write(NewTargetType);
            writer.Write(NewTargetPosition);

            writer.Write(OldItemBag);
            writer.Write(OldItem);
            writer.Write(OldItemLot);
            writer.Write(OldItemType);
        }
    }
}