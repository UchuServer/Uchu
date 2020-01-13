using System.Linq;
using RakDotNet.IO;
using Uchu.Core.Client;

namespace Uchu.World
{
    public class StatsComponent : ReplicaComponent
    {
        private Stats _stats;
        
        public bool HasStats { get; set; } = true;

        public int[] Factions { get; set; } = new int[0];
        
        public uint DamageAbsorptionPoints { get; set; }
        
        public bool Immune { get; set; }
        
        public bool GameMasterImmune { get; set; }
        
        public bool Shielded { get; set; }

        public override ComponentId Id => ComponentId.Invalid;

        protected StatsComponent()
        {
            Listen(OnStart, () =>
            {
                HasStats = false;
                
                // Stats should be centralized
                if (!GameObject.TryGetComponent(out _stats))
                    _stats = GameObject.AddComponent<Stats>();
                
                using var cdClient = new CdClientContext();

                var destroyable = cdClient.DestructibleComponentTable.FirstOrDefault(
                    c => c.Id == GameObject.Lot.GetComponentId(ComponentId.DestructibleComponent)
                );
                
                if (destroyable == default) return;

                Factions = new[] {destroyable.Faction ?? 1};
            });
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
            writer.Write<float>(_stats.MaxArmor);

            writer.Write(_stats.Imagination);
            writer.Write<float>(_stats.MaxImagination);

            writer.Write(DamageAbsorptionPoints);
            writer.WriteBit(Immune);
            writer.WriteBit(GameMasterImmune);
            writer.WriteBit(Shielded);

            writer.Write<float>(_stats.MaxHealth);
            writer.Write<float>(_stats.MaxArmor);
            writer.Write<float>(_stats.MaxImagination);

            writer.Write((uint) Factions.Length);

            foreach (var faction in Factions) writer.Write(faction);

            writer.WriteBit(_stats.Smashable && !GameObject.TryGetComponent<QuickBuildComponent>(out _));
        }
    }
}