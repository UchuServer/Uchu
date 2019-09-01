using System;
using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    [RequireComponent(typeof(StatsComponent))]
    public class DestructibleComponent : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.Destructible;

        public override void FromLevelObject(LevelObject levelObject)
        {
            
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