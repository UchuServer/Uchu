using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Uchu.Core;
using Uchu.Core.Scriptable;

namespace Uchu.World.Scriptable
{
    /// <inheritdoc />
    /// <summary>
    /// Script for every smashable object.
    /// </summary>
    [AutoAssign(typeof(DestructibleComponent))]
    public class SmashableScript : GameScript
    {
        private bool _canDrop = true;
        
        public SmashableScript(Core.World world, ReplicaPacket replicaPacket) : base(world, replicaPacket)
        {
        }

        /// <summary>
        /// Called when this object is being hit.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public override async Task OnSmashAsync(Player player)
        {
            /*
             * Code moved from GameMessage Handlers.
             */
            
            // Check if this object can drop items at this moment in time. This is to prevent duplicate drops due to
            // the current behavior system.
            if (!_canDrop) return;
            
            var session = Server.SessionCache.GetSession(player.EndPoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var obj = world.GetObject(ObjectID);
            var physics = (SimplePhysicsComponent) obj.Components.FirstOrDefault(c => c is SimplePhysicsComponent);

            if (physics == null)
                return;

            var rand = new Random();

            var spawnPosition = physics.Position;

            spawnPosition.Y++;

            var drops = await Server.CDClient.GetDropsForObjectAsync(obj.LOT);
            var itemMatrix = new Dictionary<LootMatrixRow, LootTableRow[]>();

            foreach (var drop in drops)
            {
                itemMatrix.Add(drop, await Server.CDClient.GetItemDropsAsync(drop.LootTableIndex));
            }
            
            foreach (var drop in drops)
            {
                var count = rand.Next(drop.MinDrops, drop.MaxDrops);
                /*var items = (await Server.CDClient.GetItemDropsAsync(drop.LootTableIndex)).Where(i => !i.IsMissionDrop)
                    .ToArray();*/

                var items = itemMatrix[drop];
                
                if (items.Length == 0)
                    return;

                for (var i = 0; i < count; i++)
                {
                    if (!(rand.NextDouble() <= drop.Percent)) continue;
                    var item = items[rand.Next(0, items.Length)];
                    var lootId = Utils.GenerateObjectId();
                    var finalPosition = physics.Position;

                    finalPosition.X += ((float) rand.NextDouble() % 1f - 0.5f) * 20f;
                    finalPosition.Z += ((float) rand.NextDouble() % 1f - 0.5f) * 20f;

                    world.RegisterLoot(lootId, item.ItemId);

                    Server.Send(new DropClientLootMessage
                    {
                        ObjectId = session.CharacterId,
                        UsePosition = true,
                        FinalPosition = finalPosition,
                        Currency = 0,
                        ItemLOT = item.ItemId,
                        LootObjectId = lootId,
                        OwnerObjectId = session.CharacterId,
                        SourceObjectId = ObjectID,
                        SpawnPosition = spawnPosition
                    }, player.EndPoint);
                }
            }

            /*
             * Drop Coins and Imagination.
             * TODO: Find out how to make these numbers correct.
             */
            
            for (var i = 0; i < rand.Next(0, 10); i++)
            {
                var coinLootId = Utils.GenerateObjectId();
                var coinFinalPosition = physics.Position;
            
                Server.Send(new DropClientLootMessage
                {
                    ObjectId = session.CharacterId,
                    UsePosition = true,
                    FinalPosition = coinFinalPosition,
                    Currency = 1,
                    ItemLOT = 0,
                    LootObjectId = coinLootId,
                    OwnerObjectId = session.CharacterId,
                    SourceObjectId = ObjectID,
                    SpawnPosition = spawnPosition
                }, player.EndPoint);
            }

            for (var i = 0; i < rand.Next(0, 4); i++)
            {
                var imgLootId = Utils.GenerateObjectId();
                var imgFinalPosition = physics.Position;
                
                Server.Send(new DropClientLootMessage
                {
                    ObjectId = session.CharacterId,
                    UsePosition = true,
                    FinalPosition = imgFinalPosition,
                    Currency = 0,
                    ItemLOT = (int) PickupLOT.Imagination,
                    LootObjectId = imgLootId,
                    OwnerObjectId = session.CharacterId,
                    SourceObjectId = ObjectID,
                    SpawnPosition = spawnPosition
                }, player.EndPoint);
                
                world.RegisterLoot(imgLootId, (int) PickupLOT.Imagination);
            }

            /*
             * Make this object able to drop loot again.
             */
            
            _canDrop = false;
            
            var timer = new Timer
            {
                AutoReset = false,
                Interval = 1000
            };

            timer.Elapsed += (sender, args) => { _canDrop = true; };
            
            timer.Start();
        }
    }
}