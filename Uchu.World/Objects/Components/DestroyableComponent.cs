using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World
{
    public class DestroyableComponent : Component
    {
        private uint _health;

        private uint _maxHealth;

        private uint _armor;

        private uint _maxArmor;

        private uint _imagination;

        private uint _maxImagination;
        
        public int[] Factions { get; set; } = new int[0];
        
        public int[] Enemies { get; set; } = new int[0];
        
        public uint DamageAbsorptionPoints { get; set; }
        
        public bool Immune { get; set; }
        
        public bool GameMasterImmune { get; set; }
        
        public bool Shielded { get; set; }

        public GameObject LatestDamageSource { get; private set; }
        
        public string LatestEffect { get; private set; }

        public uint Health
        {
            get => _health;
            set
            {
                value = Math.Min(value, MaxHealth);

                if (value == _health) return;

                OnHealthChanged.Invoke(value, (int) ((int) value - _health));

                _health = value;

                GameObject.Serialize(GameObject);

                if (_health == 0) OnDeath.Invoke();
            }
        }

        public uint MaxHealth
        {
            get => _maxHealth;
            set
            {
                var delta = (int) ((int) value - _maxHealth);

                if (delta < 0 && _health > value)
                {
                    OnHealthChanged.Invoke(value, (int) ((int) value - _health));

                    _health = value;
                }
                
                OnMaxHealthChanged.Invoke(value, delta);

                _maxHealth = value;

                GameObject.Serialize(GameObject);
            }
        }

        public uint Armor
        {
            get => _armor;
            set
            {
                value = Math.Min(value, MaxArmor);

                if (value == _armor) return;

                OnArmorChanged.Invoke(value, (int) ((int) value - _armor));

                _armor = value;

                GameObject.Serialize(GameObject);
            }
        }

        public uint MaxArmor
        {
            get => _maxArmor;
            set
            {
                var delta = (int) ((int) value - _maxArmor);

                if (delta < 0 && _armor > value)
                {
                    OnArmorChanged.Invoke(value, (int) ((int) value - _armor));

                    _armor = value;
                }
                
                OnMaxArmorChanged.Invoke(value, delta);

                _maxArmor = value;

                GameObject.Serialize(GameObject);
            }
        }

        public uint Imagination
        {
            get => _imagination;
            set
            {
                value = Math.Min(value, MaxImagination);

                if (value == _imagination) return;

                OnImaginationChanged.Invoke(value, (int) ((int) value - _imagination));

                _imagination = value;

                GameObject.Serialize(GameObject);
            }
        }

        public uint MaxImagination
        {
            get => _maxImagination;
            set
            {
                var delta = (int) ((int) value - _maxImagination);

                if (delta < 0 && _imagination > value)
                {
                    OnImaginationChanged.Invoke(value, (int) ((int) value - _imagination));

                    _imagination = value;
                }
                
                OnMaxImaginationChanged.Invoke(value, delta);

                _maxImagination = value;

                GameObject.Serialize(GameObject);
            }
        }

        public bool Smashable { get; set; }
        
        public bool HasStats { get; set; }

        /// <summary>
        /// New Health, Delta
        /// </summary>
        public Event<uint, int> OnHealthChanged { get; }

        /// <summary>
        /// New Armor, Delta
        /// </summary>
        public Event<uint, int> OnArmorChanged { get; }

        /// <summary>
        /// New Imagination, Delta
        /// </summary>
        public Event<uint, int> OnImaginationChanged { get; }

        /// <summary>
        /// New MaxHealth, Delta
        /// </summary>
        public Event<uint, int> OnMaxHealthChanged { get; }

        /// <summary>
        /// New MaxArmor, Delta
        /// </summary>
        public Event<uint, int> OnMaxArmorChanged { get; }

        /// <summary>
        /// New MaxImagination, Delta
        /// </summary>
        public Event<uint, int> OnMaxImaginationChanged { get; }

        public Event OnDeath { get; }

        protected DestroyableComponent()
        {
            OnHealthChanged = new Event<uint, int>();
            
            OnArmorChanged = new Event<uint, int>();
            
            OnImaginationChanged = new Event<uint, int>();
            
            OnMaxHealthChanged = new Event<uint, int>();
            
            OnMaxArmorChanged = new Event<uint, int>();
            
            OnMaxImaginationChanged = new Event<uint, int>();
            
            OnDeath = new Event();
            
            Listen(OnStart, async () =>
            {
                if (GameObject is Player) CollectPlayerStats();
                else CollectObjectStats();
                
                await using var cdClient = new CdClientContext();

                var destroyable = cdClient.DestructibleComponentTable.FirstOrDefault(
                    c => c.Id == GameObject.Lot.GetComponentId(ComponentId.DestructibleComponent)
                );
                
                if (destroyable == default) return;

                Factions = new[] {destroyable.Faction ?? 1};

                Smashable = destroyable.IsSmashable ?? false;

                var faction = await cdClient.FactionsTable.FirstOrDefaultAsync(
                    f => f.Faction == Factions[0]
                );
                
                if (faction?.EnemyList == default) return;
                
                if (string.IsNullOrWhiteSpace(faction.EnemyList)) return;

                Enemies = faction.EnemyList
                    .Replace(" ", "")
                    .Split(',')
                    .Select(int.Parse)
                    .ToArray();
            });

            Listen(OnDestroyed, () =>
            {
                OnHealthChanged.Clear();
                OnArmorChanged.Clear();
                OnImaginationChanged.Clear();
                OnHealthChanged.Clear();
                OnMaxArmorChanged.Clear();
                OnMaxImaginationChanged.Clear();

                OnDeath.Clear();
            });

            if (!(GameObject is Player)) return;

            Listen(OnHealthChanged, async (total, delta) =>
            {
                await using var ctx = new UchuContext();

                var character = await ctx.Characters.FirstAsync(c => c.Id == GameObject.Id);

                character.CurrentHealth = (int) total;

                await ctx.SaveChangesAsync();
            });

            Listen(OnArmorChanged, async (total, delta) =>
            {
                await using var ctx = new UchuContext();

                var character = await ctx.Characters.FirstAsync(c => c.Id == GameObject.Id);

                character.CurrentArmor = (int) total;

                await ctx.SaveChangesAsync();
            });

            Listen(OnImaginationChanged, async (total, delta) =>
            {
                await using var ctx = new UchuContext();

                var character = await ctx.Characters.FirstAsync(c => c.Id == GameObject.Id);

                character.CurrentImagination = (int) total;

                await ctx.SaveChangesAsync();
            });

            Listen(OnMaxHealthChanged, async (total, delta) =>
            {
                await using var ctx = new UchuContext();

                var character = await ctx.Characters.FirstAsync(c => c.Id == GameObject.Id);

                character.MaximumHealth = (int) total;

                await ctx.SaveChangesAsync();
            });

            Listen(OnMaxArmorChanged, async (total, delta) =>
            {
                await using var ctx = new UchuContext();

                var character = await ctx.Characters.FirstAsync(c => c.Id == GameObject.Id);

                character.MaximumArmor = (int) total;

                await ctx.SaveChangesAsync();
            });

            Listen(OnMaxImaginationChanged, async (total, delta) =>
            {
                await using var ctx = new UchuContext();

                var character = await ctx.Characters.FirstAsync(c => c.Id == GameObject.Id);

                character.MaximumImagination = (int) total;

                await ctx.SaveChangesAsync();
            });
        }

        public void Damage(uint value, GameObject source, string effectHandler = "")
        {
            LatestDamageSource = source;
            LatestEffect = effectHandler;
            
            var armorDamage = Math.Min(value, Armor);

            value -= armorDamage;
            Armor -= armorDamage;

            Health -= Math.Min(value, Health);
        }

        public void Heal(uint value)
        {
            var armorHeal = Math.Min(value, MaxArmor - Armor);

            value -= armorHeal;
            Armor += armorHeal;

            Health += Math.Min(value, MaxHealth - Health);
        }

        public async Task BoostBaseHealth(uint delta)
        {
            if (!(GameObject is Player)) return;

            await using var ctx = new UchuContext();

            var character = ctx.Characters.First(c => c.Id == GameObject.Id);

            character.BaseHealth += (int) delta;

            MaxHealth += delta;

            Health += delta;

            await ctx.SaveChangesAsync();
        }

        public async Task BoostBaseImagination(uint delta)
        {
            if (!(GameObject is Player)) return;

            await using var ctx = new UchuContext();

            var character = ctx.Characters.First(c => c.Id == GameObject.Id);

            character.BaseImagination += (int) delta;

            MaxImagination += delta;

            Imagination += delta;

            await ctx.SaveChangesAsync();
        }

        private void CollectObjectStats()
        {
            using var cdClient = new CdClientContext();

            var stats = cdClient.DestructibleComponentTable.FirstOrDefault(
                o => o.Id == GameObject.Lot.GetComponentId(ComponentId.DestructibleComponent)
            );

            if (stats == default) return;

            var rawHealth = stats.Life ?? 0;
            var rawArmor = (int) (stats.Armor ?? 0);
            var rawImagination = stats.Imagination ?? 0;

            _health = (uint) (rawHealth != -1 ? rawHealth : 0);
            _armor = (uint) (rawArmor != -1 ? rawArmor : 0);
            _imagination = (uint) (rawImagination != -1 ? rawImagination : 0);

            _maxHealth = Health;
            _maxArmor = Armor;
            _maxImagination = Imagination;
        }

        private void CollectPlayerStats()
        {
            if (!(GameObject is Player)) return;
            
            using var ctx = new UchuContext();

            var character = ctx.Characters.First(c => c.Id == GameObject.Id);

            /*
             * Any additional stats gets added on by skills.
             */
            
            _health = (uint) character.CurrentHealth;
            _maxHealth = (uint) character.BaseHealth;

            _armor = (uint) character.CurrentArmor;
            _maxArmor = default;

            _imagination = (uint) character.CurrentImagination;
            _maxImagination = (uint) character.BaseImagination;
        }
        
        public void Construct(BitWriter writer)
        {
            writer.WriteBit(true);

            for (var i = 0; i < 9; i++) writer.Write<uint>(0);

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

        public void Serialize(BitWriter writer)
        {
            WriteStats(writer);

            writer.WriteBit(true);
            writer.WriteBit(false);
        }

        private void WriteStats(BitWriter writer)
        {
            if (!writer.Flag(HasStats)) return;
            
            writer.Write(Health);
            writer.Write<float>(MaxHealth);

            writer.Write(Armor);
            writer.Write<float>(MaxArmor);

            writer.Write(Imagination);
            writer.Write<float>(MaxImagination);

            writer.Write(DamageAbsorptionPoints);
            writer.WriteBit(Immune);
            writer.WriteBit(GameMasterImmune);
            writer.WriteBit(Shielded);

            writer.Write<float>(MaxHealth);
            writer.Write<float>(MaxArmor);
            writer.Write<float>(MaxImagination);

            writer.Write((uint) Factions.Length);

            foreach (var faction in Factions) writer.Write(faction);

            writer.WriteBit(Smashable);
        }
    }
}