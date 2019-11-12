using System;
using System.Linq;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World
{
    [RequireComponent(typeof(StatsComponent))]
    public class DestructibleComponent : ReplicaComponent
    {
        private readonly Random _random;
        
        private Core.Client.DestructibleComponent _cdClientComponent;
        private Stats _stats;

        public override ComponentId Id => ComponentId.DestructibleComponent;

        /// <summary>
        ///     Warning: Should not be used as a definitive client state. Could be unreliable.
        /// </summary>
        public bool Alive { get; private set; } = true;

        private float _resurrectTime;

        /// <summary>
        ///     Killer, Loot Owner
        /// </summary>
        public readonly Event<GameObject, Player> OnSmashed = new Event<GameObject, Player>();

        public readonly Event OnResurrect = new Event();

        protected DestructibleComponent()
        {
            _random = new Random();
            
            OnStart.AddListener(() =>
            {
                if (GameObject.Settings.TryGetValue("respawn", out var resTimer))
                {
                    _resurrectTime = resTimer switch
                    {
                        uint timer => timer,
                        float timer => timer,
                        int timer => timer,
                        _ => _resurrectTime
                    };
                }

                GameObject.Layer = Layer.Smashable;

                using (var cdClient = new CdClientContext())
                {
                    var entry = GameObject.Lot.GetComponentId(ComponentId.DestructibleComponent);

                    _cdClientComponent = cdClient.DestructibleComponentTable.FirstOrDefault(c => c.Id == entry);

                    if (_cdClientComponent == default)
                        Logger.Error($"{GameObject} has a corrupt Destructible Component of id: {entry}");
                }

                foreach (var stats in GameObject.GetComponents<StatsComponent>()) stats.HasStats = false;
                
                if (GameObject.TryGetComponent(out _stats))
                {
                    _stats.OnDeath.AddListener(() =>
                    {
                        Logger.Debug($"LATEST: {_stats.LatestDamageSource}");
                        
                        Smash(
                            _stats.LatestDamageSource,
                            _stats.LatestDamageSource is Player player ? player : default
                        );
                    });
                    
                    return;
                }
                
                Logger.Error($"{GameObject} has a {nameof(DestructibleComponent)} with a {nameof(Stats)} component.");
            });
        }

        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
        }

        public void Smash(GameObject killer, Player lootOwner = default, string animation = default)
        {
            if (!Alive) return;

            lootOwner ??= killer as Player;

            OnSmashed.Invoke(killer, lootOwner);

            Alive = false;
            
            Zone.BroadcastMessage(new DieMessage
            {
                Associate = GameObject,
                DeathType = animation ?? "",
                Killer = killer,
                SpawnLoot = false,
                LootOwner = lootOwner ?? GameObject
            });
            
            if (As<Player>() != null)
            {
                //
                // Player
                //
                
                var coinToDrop = Math.Min((long) Math.Round(As<Player>().Currency * 0.1), 10000);
                As<Player>().Currency -= coinToDrop;

                InstancingUtil.Currency((int) coinToDrop, lootOwner, lootOwner, Transform.Position);

                return;
            }

            //
            // Normal Smashable
            //
            
            if (lootOwner == default) return;

            GameObject.Layer -= Layer.Smashable;
            GameObject.Layer += Layer.Hidden;

            Task.Run(async () =>
            {
                await Task.Delay((int) (_resurrectTime * 1000));
                
                //
                // Re-Spawn Smashable 
                //

                Alive = true;
                
                GameObject.Layer += Layer.Smashable;
                GameObject.Layer -= Layer.Hidden;

                _stats.Health = _stats.MaxHealth;
            });

            using var cdClient = new CdClientContext();
            var matrices = cdClient.LootMatrixTable.Where(l =>
                l.LootMatrixIndex == _cdClientComponent.LootMatrixIndex
            ).ToArray();

            foreach (var matrix in matrices)
            {
                var count = _random.Next(matrix.MinToDrop ?? 0, matrix.MaxToDrop ?? 0);

                var items = cdClient.LootTableTable.Where(t => t.LootTableIndex == matrix.LootTableIndex).ToList();
                
                for (var i = 0; i < count; i++)
                {
                    if (items.Count == default) break;
                    
                    var proc = _random.NextDouble();

                    if (!(proc <= matrix.Percent)) continue;

                    var item = items[_random.Next(0, items.Count)];
                    items.Remove(item);

                    if (item.Itemid == null) continue;

                    var drop = InstancingUtil.Loot(item.Itemid ?? 0, lootOwner, GameObject, Transform.Position);
                    
                    Start(drop);
                }
            }

            var currencies = cdClient.CurrencyTableTable.Where(
                c => c.CurrencyIndex == _cdClientComponent.CurrencyIndex
            );

            foreach (var currency in currencies)
            {
                if (currency.Npcminlevel > _cdClientComponent.Level) continue;

                var coinToDrop = _random.Next(currency.Minvalue ?? 0, currency.Maxvalue ?? 0);

                InstancingUtil.Currency(coinToDrop, lootOwner, GameObject, Transform.Position);
            }
        }

        public void Resurrect()
        {
            if (As<Player>() != null)
            {
                Alive = true;
                
                Zone.BroadcastMessage(new ResurrectMessage
                {
                    Associate = As<Player>()
                });
            }

            OnResurrect.Invoke();
            
            _stats.Health = Math.Min(_stats.MaxHealth, 4);
        }
    }
}