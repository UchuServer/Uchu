using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class DropClientLootMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.DropClientLoot;
        
        public bool UsePosition { get; set; }
        
        public Vector3 FinalPosition { get; set; } = Vector3.Zero;
        
        public int Currency { get; set; }
        
        public Lot Lot { get; set; }
        
        public GameObject Loot { get; set; }
        
        public Player Owner { get; set; }
        
        public GameObject Source { get; set; }
        
        public Vector3 SpawnPosition { get; set; } = Vector3.Zero;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(UsePosition);

            var hasFinalPosition = FinalPosition != Vector3.Zero;
            writer.WriteBit(hasFinalPosition);
            if (hasFinalPosition) writer.Write(FinalPosition);

            writer.Write(Currency);
            writer.Write(Lot.Id);

            writer.Write(Loot);
            writer.Write(Owner);
            writer.Write(Source);

            var hasSpawnPosition = SpawnPosition != Vector3.Zero;
            writer.WriteBit(hasSpawnPosition);
            if (hasSpawnPosition) writer.Write(SpawnPosition);
        }
    }
}