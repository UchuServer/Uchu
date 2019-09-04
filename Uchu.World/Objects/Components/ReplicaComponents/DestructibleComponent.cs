using System;
using System.Linq;
using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    [RequireComponent(typeof(StatsComponent))]
    public class DestructibleComponent : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.Destructible;

        public bool Alive { get; private set; } = true;

        private Core.CdClient.DestructibleComponent _cdClientComponent;

        private Random _random;

        public override void FromLevelObject(LevelObject levelObject)
        {
            _random = new Random();
            
            using (var cdClient = new CdClientContext())
            {
                var entry = GameObject.Lot.GetComponentId(ReplicaComponentsId.Destructible);

                _cdClientComponent = cdClient.DestructibleComponentTable.FirstOrDefault(c => c.Id == entry);

                if (_cdClientComponent == default)
                {
                    Logger.Error($"{GameObject} has a corrupt Destructible Component of id: {entry}");
                }
            }
        }

        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
            // Empty
        }

        public void Smash(GameObject killer, GameObject lootOwner = default, string animation = default)
        {
            Alive = false;
            
            if (Player != null)
            {
                Zone.BroadcastMessage(new DieMessage
                {
                    Associate = Player,
                    DeathType = animation ?? "",
                    Killer = killer,
                    SpawnLoot = false,
                    LootOwner = lootOwner ?? Player
                });

                var coinToDrop = Math.Min((long) Math.Round(Player.Currency * 0.1), 10000);
                Player.Currency -= coinToDrop;
                Player.EntitledCurrency += coinToDrop;
                
                Player.Message(new DropClientLootMessage
                {
                    Associate = Player,
                    Currency = (int) coinToDrop,
                    Owner = Player,
                    Source = Player,
                    SpawnPosition = Player.Transform.Position
                });
                
                return;
            }
            
            using (var cdClient = new CdClientContext())
            {
                var matrices = cdClient.LootMatrixTable.Where(l =>
                    l.LootMatrixIndex == _cdClientComponent.LootMatrixIndex).ToArray();
                
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

                        var drop = GameObject.Instantiate(
                            Zone,
                            item.Itemid.Value,
                            Transform.Position,
                            Transform.Rotation
                        );

                        var finalPosition = new Vector3
                        {
                            X = drop.Transform.Position.X + ((float) _random.NextDouble() % 1f - 0.5f) * 20f,
                            Y = drop.Transform.Position.Y,
                            Z = drop.Transform.Position.X + ((float) _random.NextDouble() % 1f - 0.5f) * 20f
                        };

                        // TODO: Look into weird spawning location
                        
                        Logger.Debug($"Spawning {drop} [{drop.Lot}] at {drop.Transform.Position}");
                        
                        Zone.BroadcastMessage(new DropClientLootMessage
                        {
                            Associate = killer,
                            UsePosition = true,
                            FinalPosition = finalPosition,
                            Currency = default,
                            Lot = drop.Lot,
                            LootObjectId = drop.ObjectId,
                            Owner = (lootOwner ?? killer) as Player,
                            Source = GameObject,
                            SpawnPosition = drop.Transform.Position
                        });
                    }
                }
            }
        }

        public void Resurrect()
        {
            Alive = true;
            
            if (Player != null)
            {
                Zone.BroadcastMessage(new ResurrectMessage
                {
                    Associate = Player
                });
            }
        }
    }
}