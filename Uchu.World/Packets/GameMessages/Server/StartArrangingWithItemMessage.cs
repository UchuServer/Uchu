using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class StartArrangingWithItemMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.StartArrangingWithItem;

        public bool FirstTime { get; set; } = true;
        
        public GameObject BuildArea { get; set; }
        
        public Vector3 StartPosition { get; set; }
        
        public int SourceBag { get; set; }
        
        public GameObject Source { get; set; }
        
        public Lot SourceLot { get; set; }
        
        public int SourceType { get; set; }
        
        public GameObject Target { get; set; }
        
        public Lot TargetLot { get; set; }
        
        public Vector3 TargetPosition { get; set; }
        
        public int TargetType { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(FirstTime);

            if (writer.Flag(BuildArea != default))
                writer.Write(BuildArea);

            writer.Write(StartPosition);

            writer.Write(SourceBag);
            writer.Write(Source);
            writer.Write(SourceLot);
            writer.Write(SourceType);

            writer.Write(Target);
            writer.Write(TargetLot);
            writer.Write(TargetPosition);
            writer.Write(TargetType);
        }
    }
}