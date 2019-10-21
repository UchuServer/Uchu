using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.CdClient;

namespace Uchu.World
{
    public class Stats : Component
    {
        private uint _health;

        private uint _maxHealth;

        private uint _armor;

        private uint _maxArmor;

        private uint _imagination;

        private uint _maxImagination;

        public GameObject LatestDamageSource;

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
                OnMaxHealthChanged.Invoke(value, (int) ((int) value - _maxHealth));

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
                OnMaxArmorChanged.Invoke(value, (int) ((int) value - _maxArmor));

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
                OnMaxImaginationChanged.Invoke(value, (int) ((int) value - _maxImagination));

                _maxImagination = value;

                GameObject.Serialize(GameObject);
            }
        }

        public bool Smashable { get; set; }

        /// <summary>
        /// New Health, Delta
        /// </summary>
        public readonly AsyncEvent<uint, int> OnHealthChanged = new AsyncEvent<uint, int>();

        /// <summary>
        /// New Armor, Delta
        /// </summary>
        public readonly AsyncEvent<uint, int> OnArmorChanged = new AsyncEvent<uint, int>();

        /// <summary>
        /// New Imagination, Delta
        /// </summary>
        public readonly AsyncEvent<uint, int> OnImaginationChanged = new AsyncEvent<uint, int>();

        /// <summary>
        /// New MaxHealth, Delta
        /// </summary>
        public readonly AsyncEvent<uint, int> OnMaxHealthChanged = new AsyncEvent<uint, int>();

        /// <summary>
        /// New MaxArmor, Delta
        /// </summary>
        public readonly AsyncEvent<uint, int> OnMaxArmorChanged = new AsyncEvent<uint, int>();

        /// <summary>
        /// New MaxImagination, Delta
        /// </summary>
        public readonly AsyncEvent<uint, int> OnMaxImaginationChanged = new AsyncEvent<uint, int>();

        public readonly Event OnDeath = new Event();

        public Stats()
        {
            OnStart.AddListener(() =>
            {
                if (GameObject is Player) CollectPlayerStats();
                else CollectObjectStats();
            });

            OnDestroyed.AddListener(() =>
            {
                OnHealthChanged.Clear();
                OnArmorChanged.Clear();
                OnImaginationChanged.Clear();
                OnHealthChanged.Clear();
                OnMaxArmorChanged.Clear();
                OnMaxImaginationChanged.Clear();

                OnDeath.Clear();
            });

            if (As<Player>() == default) return;

            OnHealthChanged.AddListener(async (total, delta) =>
            {
                await using var ctx = new UchuContext();

                var character = await ctx.Characters.FirstAsync(c => c.CharacterId == As<Player>().ObjectId);

                character.CurrentHealth = (int) total;

                await ctx.SaveChangesAsync();
            });

            OnArmorChanged.AddListener(async (total, delta) =>
            {
                await using var ctx = new UchuContext();

                var character = await ctx.Characters.FirstAsync(c => c.CharacterId == As<Player>().ObjectId);

                character.CurrentArmor = (int) total;

                await ctx.SaveChangesAsync();
            });

            OnImaginationChanged.AddListener(async (total, delta) =>
            {
                await using var ctx = new UchuContext();

                var character = await ctx.Characters.FirstAsync(c => c.CharacterId == As<Player>().ObjectId);

                character.CurrentImagination = (int) total;

                await ctx.SaveChangesAsync();
            });

            OnMaxHealthChanged.AddListener(async (total, delta) =>
            {
                await using var ctx = new UchuContext();

                var character = await ctx.Characters.FirstAsync(c => c.CharacterId == As<Player>().ObjectId);

                character.MaximumHealth = (int) total;

                await ctx.SaveChangesAsync();
            });

            OnMaxArmorChanged.AddListener(async (total, delta) =>
            {
                await using var ctx = new UchuContext();

                var character = await ctx.Characters.FirstAsync(c => c.CharacterId == As<Player>().ObjectId);

                character.MaximumArmor = (int) total;

                await ctx.SaveChangesAsync();
            });

            OnMaxImaginationChanged.AddListener(async (total, delta) =>
            {
                await using var ctx = new UchuContext();

                var character = await ctx.Characters.FirstAsync(c => c.CharacterId == As<Player>().ObjectId);

                character.MaximumImagination = (int) total;

                await ctx.SaveChangesAsync();
            });
        }

        public void Damage(uint value, GameObject source)
        {
            var armorDamage = Math.Min(value, Armor);

            value -= armorDamage;
            Armor -= armorDamage;

            Health -= Math.Min(value, Health);

            LatestDamageSource = source;
        }

        public void Heal(uint value)
        {
            var armorHeal = Math.Min(value, MaxArmor - Armor);

            value -= armorHeal;
            Armor += armorHeal;

            Health += Math.Min(value, MaxHealth - Health);
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
            using var ctx = new UchuContext();

            var character = ctx.Characters.First(c => c.CharacterId == As<Player>().ObjectId);

            _health = (uint) character.CurrentHealth;
            _maxHealth = (uint) character.MaximumHealth;

            _armor = (uint) character.CurrentArmor;
            _maxArmor = (uint) character.MaximumArmor;

            _imagination = (uint) character.CurrentImagination;
            _maxImagination = (uint) character.MaximumImagination;
        }
    }
}