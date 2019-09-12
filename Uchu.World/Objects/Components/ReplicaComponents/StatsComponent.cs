using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class StatsComponent : ReplicaComponent
    {
        public bool HasStats { get; set; } = true;

        public uint CurrentHealth { get; set; } = 4;
        
        public uint CurrentArmor { get; set; }
        
        public uint CurrentImagination { get; set; }

        public uint MaxHealth { get; set; } = 4;
        
        public uint MaxArmor { get; set; }
        
        public uint MaxImagination { get; set; }

        public int[] Factions { get; set; } = {1};

        public bool Smashable { get; set; } = false;
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Invalid;

        public override void FromLevelObject(LevelObject levelObject)
        {
            HasStats = false;
        }

        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(true);

            for (var i = 0; i < 9; i++)
            {
                writer.Write<uint>(0);
            }

            WriteStats(writer);

            if (HasStats)
            {
                writer.WriteBit(false);
                writer.WriteBit(false);

                if (Smashable)
                {
                    writer.WriteBit(false);
                    writer.WriteBit(false);
                }
            }

            writer.WriteBit(true);
            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
            WriteStats(writer);

            writer.WriteBit(true);
            writer.WriteBit(false);
        }

        private void WriteStats(BitWriter writer)
        {
            writer.WriteBit(HasStats);
            
            if (!HasStats) return;

            writer.Write(CurrentHealth);
            writer.Write<float>(CurrentHealth);

            writer.Write(CurrentArmor);
            writer.Write<float>(CurrentArmor);

            writer.Write(CurrentImagination);
            writer.Write<float>(CurrentImagination);

            writer.Write<uint>(0);
            writer.WriteBit(true);
            writer.WriteBit(false);
            writer.WriteBit(false);

            writer.Write<float>(MaxHealth);
            writer.Write<float>(MaxArmor);
            writer.Write<float>(MaxImagination);

            writer.Write((uint) Factions.Length);

            foreach (var faction in Factions)
            {
                writer.Write(faction);
            }

            writer.WriteBit(Smashable);
        }
    }
}