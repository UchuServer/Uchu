using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class StartBuildingWithItemMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.StartBuildingWithItem;

        public bool FirstTime { get; set; }
        
        public bool Success { get; set; }
        
        public int SourceBag { get; set; }
        
        public GameObject Source { get; set; }
        
        public Lot SourceLot { get; set; }
        
        public int SourceType { get; set; }
        
        public GameObject Target { get; set; }
        
        public Lot TargetLot { get; set; }
        
        public Vector3 TargetPosition { get; set; }
        
        public int TargetType { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            FirstTime = reader.ReadBit();
            Success = reader.ReadBit();

            SourceBag = reader.Read<int>();
            Source = reader.ReadGameObject(Associate.Zone);
            SourceLot = reader.Read<Lot>();
            SourceType = reader.Read<int>();

            Target = reader.ReadGameObject(Associate.Zone);
            TargetLot = reader.Read<Lot>();
            TargetPosition = reader.Read<Vector3>();
            TargetType = reader.Read<int>();
        }
    }
}