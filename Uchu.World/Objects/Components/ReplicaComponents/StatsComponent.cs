using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class StatsComponent : ReplicaComponent
    {
        private Stats _stats;
        
        public bool HasStats { get; set; } = true;

        public int[] Factions { get; set; } = {1};

        public override ReplicaComponentsId Id => ReplicaComponentsId.Invalid;

        public StatsComponent()
        {
            OnStart += () =>
            {
                // Stats should be centralized
                if (!GameObject.TryGetComponent(out _stats))
                    _stats = GameObject.AddComponent<Stats>();
            };
        }
        
        public override void FromLevelObject(LevelObject levelObject)
        {
            HasStats = false;
        }

        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(true);

            for (var i = 0; i < 9; i++) writer.Write<uint>(0);

            WriteStats(writer);

            if (HasStats)
            {
                writer.WriteBit(false);
                writer.WriteBit(false);

                if (_stats.Smashable)
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

            writer.Write(_stats.Health);
            writer.Write<float>(_stats.MaxHealth);

            writer.Write(_stats.Armor);
            writer.Write<float>(_stats.Armor);

            writer.Write(_stats.Imagination);
            writer.Write<float>(_stats.Imagination);

            writer.Write<uint>(0);
            writer.WriteBit(true);
            writer.WriteBit(false);
            writer.WriteBit(false);

            writer.Write<float>(_stats.Health);
            writer.Write<float>(_stats.Armor);
            writer.Write<float>(_stats.Imagination);

            writer.Write((uint) Factions.Length);

            foreach (var faction in Factions) writer.Write(faction);

            writer.WriteBit(_stats.Smashable);
        }
    }
}