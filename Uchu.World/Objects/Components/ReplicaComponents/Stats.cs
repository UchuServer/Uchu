using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class Stats : ReplicaComponent
    {
        public bool HasStats { get; set; } = true;

        public uint CurrentHealth { get; set; } = 4;
        
        public uint CurrentArmor { get; set; }
        
        public uint CurrentImagination { get; set; }

        public uint MaxHealth { get; set; } = 4;
        
        public uint MaxArmor { get; set; }
        
        public uint MaxImagination { get; set; }
        
        public int[] Factions { get; set; }

        public bool Smashable { get; set; } = false;
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Invalid;
        
        public override void Construct(BitWriter writer)
        {
            writer.Write(true);

            writer.Write(new byte[9 * 4], 9 * 4 * 8);

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
            writer.Write(HasStats);
            
            if (!HasStats) return;

            writer.Write(CurrentHealth);
            writer.Write<float>(CurrentHealth);

            writer.Write(CurrentArmor);
            writer.Write<float>(CurrentArmor);

            writer.Write(CurrentImagination);
            writer.Write<float>(CurrentImagination);

            writer.Write<uint>(0);
            writer.Write(true);
            writer.Write(false);
            writer.Write(false);

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