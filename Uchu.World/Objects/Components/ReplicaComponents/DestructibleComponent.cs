using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [RequireComponent(typeof(StatsComponent))]
    public class DestructibleComponent : ReplicaComponent
    {
        private Core.CdClient.DestructibleComponent _cdClientComponent;
        private Random _random;

        public override ReplicaComponentsId Id => ReplicaComponentsId.Destructible;

        public bool CanDrop { get; private set; } = true;

        public float ResurrectTime;

        /// <summary>
        /// Killer, Loot Owner
        /// </summary>
        public event Action<GameObject, Player> OnSmashed;

        public event Action OnResurrect;

        public override void FromLevelObject(LevelObject levelObject)
        {
            if (levelObject.Settings.TryGetValue("respawn", out var resTimer))
            {
                ResurrectTime = (float) resTimer;
            }

            _random = new Random();

            GameObject.Layer = Layer.Smashable;

            using (var cdClient = new CdClientContext())
            {
                var entry = GameObject.Lot.GetComponentId(ReplicaComponentsId.Destructible);

                _cdClientComponent = cdClient.DestructibleComponentTable.FirstOrDefault(c => c.Id == entry);

                if (_cdClientComponent == default)
                    Logger.Error($"{GameObject} has a corrupt Destructible Component of id: {entry}");
            }

            foreach (var stats in GameObject.GetComponents<StatsComponent>()) stats.HasStats = false;
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
            if (!CanDrop) return;

            lootOwner ??= killer as Player;

            OnSmashed?.Invoke(killer, lootOwner);

            CanDrop = false;

            if (As<Player>() != null)
            {
                Zone.BroadcastMessage(new DieMessage
                {
                    Associate = As<Player>(),
                    DeathType = animation ?? "",
                    Killer = killer,
                    SpawnLoot = false,
                    LootOwner = lootOwner ?? As<Player>()
                });

                var coinToDrop = Math.Min((long) Math.Round(As<Player>().Currency * 0.1), 10000);
                As<Player>().Currency -= coinToDrop;

                InstancingUtil.Currency((int) coinToDrop, lootOwner, lootOwner, Transform.Position);

                return;
            }

            if (lootOwner == default) return;

            GameObject.Layer -= Layer.Smashable;
            GameObject.Layer += Layer.Hidden;

            Task.Run(async () =>
            {
                await Task.Delay((int) (ResurrectTime * 1000));

                CanDrop = true;

                GameObject.Layer += Layer.Smashable;
                GameObject.Layer -= Layer.Hidden;
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

                InstancingUtil.Currency(coinToDrop, lootOwner, lootOwner, Transform.Position);
            }
        }

        public void Resurrect()
        {
            if (As<Player>() != null)
            {
                Zone.BroadcastMessage(new ResurrectMessage
                {
                    Associate = As<Player>()
                });
            }

            OnResurrect?.Invoke();
        }
    }
}