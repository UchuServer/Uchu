using System;
using RakDotNet;

namespace Uchu.Core
{
    public class StatsComponent : ReplicaComponent
    {
        public bool HasStats { get; set; } = true;

        public uint CurrentHealth { get; set; } = 4;
        public uint CurrentArmor { get; set; } = 0;
        public uint CurrentImagination { get; set; } = 0;

        public float MaxHealth { get; set; } = 4;
        public float MaxArmor { get; set; } = 0;
        public float MaxImagination { get; set; } = 0;

        public int[] Factions = {1};

        public bool Smashable { get; set; } = false;

        private void _write(BitStream stream)
        {
            stream.WriteBit(HasStats);

            if (!HasStats) return;
            stream.WriteUInt(CurrentHealth);
            stream.WriteFloat(MaxHealth);
            stream.WriteUInt(CurrentArmor);
            stream.WriteFloat(MaxArmor);
            stream.WriteUInt(CurrentImagination);
            stream.WriteFloat(MaxImagination);
            stream.WriteUInt(0);
            stream.WriteBit(true);
            stream.WriteBit(false);
            stream.WriteBit(false);
            stream.WriteFloat(MaxHealth);
            stream.WriteFloat(MaxArmor);
            stream.WriteFloat(MaxImagination);
            stream.WriteUInt((uint) Factions.Length);

            foreach (var faction in Factions) stream.WriteInt(faction);

            stream.WriteBit(Smashable);
        }

        public override void Serialize(BitStream stream)
        {
            _write(stream);

            stream.WriteBit(true);
            stream.WriteBit(false);
        }

        public override void Construct(BitStream stream)
        {
            stream.WriteBit(true);

            for (var i = 0; i < 9; i++)
            {
                stream.WriteUInt(0);
            }

            _write(stream);

            if (HasStats)
            {
                stream.WriteBit(false);
                stream.WriteBit(false);

                if (Smashable)
                {
                    stream.WriteBit(false);
                    stream.WriteBit(false);
                }
            }

            stream.WriteBit(true);
            stream.WriteBit(false);
        }
    }
}