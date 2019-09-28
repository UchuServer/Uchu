using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class DoneArrangingWithItemMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.DoneArrangingWithItem;

        public int NewSourceBag { get; set; }
        
        public GameObject NewSource { get; set; }
        
        public Lot NewSourceLot { get; set; }
        
        public int NewSourceType { get; set; }
        
        public GameObject NewTarget { get; set; }
        
        public Lot NewTargetLot { get; set; }
        
        public int NewTargetType { get; set; }
        
        public Vector3 NewTargetPosition { get; set; }
        
        public int OldSourceBag { get; set; }
        
        public GameObject OldSource { get; set; }
        
        public Lot OldSourceLot { get; set; }
        
        public int OldSourceType { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            NewSourceBag = reader.Read<int>();
            NewSource = reader.ReadGameObject(Associate.Zone);
            NewSourceLot = reader.Read<Lot>();
            NewSourceType = reader.Read<int>();

            NewTarget = reader.ReadGameObject(Associate.Zone);
            NewTargetLot = reader.Read<Lot>();
            NewTargetType = reader.Read<int>();
            NewTargetPosition = reader.Read<Vector3>();

            OldSourceBag = reader.Read<int>();
            OldSource = reader.ReadGameObject(Associate.Zone);
            OldSourceLot = reader.Read<Lot>();
            OldSourceType = reader.Read<int>();
        }
    }
}