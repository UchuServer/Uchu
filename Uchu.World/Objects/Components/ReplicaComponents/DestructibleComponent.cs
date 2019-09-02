using System;
using System.Linq;
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

        public Core.CdClient.DestructibleComponent CdClientComponent;

        public Random _random;

        public override void FromLevelObject(LevelObject levelObject)
        {
            _random = new Random();
            
            using (var cdClient = new CdClientContext())
            {
                var entry = GameObject.Lot.GetComponentId(ReplicaComponentsId.Destructible);

                CdClientComponent = cdClient.DestructibleComponentTable.FirstOrDefault(c => c.Id == entry);

                if (CdClientComponent == default)
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
            }

            using (var cdClient = new CdClientContext())
            {
                var matrices = cdClient.LootMatrixTable.Where(l =>
                    l.LootMatrixIndex == CdClientComponent.LootMatrixIndex).ToArray();
                
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

                        var drop = GameObject.Instantiate(Zone, item.Itemid.Value);

                        var finalPosition = Transform.Position;
                        finalPosition.X += ((float) _random.NextDouble() % 1f - 0.5f) * 20f;
                        finalPosition.Z += ((float) _random.NextDouble() % 1f - 0.5f) * 20f;
                        
                        Zone.BroadcastMessage(new DropClientLootMessage
                        {
                            Associate = killer,
                            UsePosition = true,
                            FinalPosition = finalPosition,
                            Currency = default,
                            Lot = drop.Lot,
                            LootObjectId = drop.ObjectId,
                            Owner = killer as Player,
                            Source = GameObject,
                            SpawnPosition = Transform.Position
                        });
                    }
                }
            }
        }

        public void Resurrect()
        {
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