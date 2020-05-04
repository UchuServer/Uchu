using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World
{
    public class ScriptedActivityComponent : ReplicaComponent
    {
        private readonly Random _random;
        
        public readonly List<GameObject> Participants = new List<GameObject>();

        public float[] Parameters { get; set; } = new float[10];

        public override ComponentId Id => ComponentId.ScriptedActivityComponent;

        public Activities ActivityInfo { get; private set; }
        
        public ActivityRewards[] Rewards { get; private set; }

        protected ScriptedActivityComponent()
        {
            _random = new Random();
            
            Listen(OnStart, async () =>
            {
                if (!GameObject.Settings.TryGetValue("activityID", out var id))
                {
                    return;
                }

                var activityId = (int) id;
                await using var cdClient = new CdClientContext();

                ActivityInfo = await cdClient.ActivitiesTable.FirstOrDefaultAsync(
                    a => a.ActivityID == activityId
                );

                if (ActivityInfo == default) return;
                
                ActivityInfo = await cdClient.ActivitiesTable.FirstOrDefaultAsync(
                    a => a.ActivityID == activityId
                );

                if (ActivityInfo == default)
                {
                    Logger.Error($"{GameObject} has an invalid activityID: {activityId}");
                    return;
                }

                Rewards = cdClient.ActivityRewardsTable.Where(
                    a => a.ObjectTemplate == activityId
                ).ToArray();
            });
        }

        public async Task DropLootAsync(Player lootOwner)
        {
            await using var cdClient = new CdClientContext();
            
            var matrices = cdClient.LootMatrixTable.Where(l =>
                Rewards.Any(r => r.LootMatrixIndex == l.LootMatrixIndex)
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

                    lootOwner.SendChatMessage("Dropping activity item!!!");
                    
                    var drop = InstancingUtilities.InstantiateLoot(item.Itemid ?? 0, lootOwner, GameObject, Transform.Position);
                    
                    Start(drop);
                }
            }

            foreach (var reward in Rewards)
            {
                var currencies = cdClient.CurrencyTableTable.Where(c => 
                    c.CurrencyIndex == reward.CurrencyIndex
                );

                foreach (var currency in currencies)
                {
                    if (currency.Npcminlevel > reward.ChallengeRating) continue;

                    var coinToDrop = _random.Next(currency.Minvalue ?? 0, currency.Maxvalue ?? 0);
                    
                    lootOwner.SendChatMessage("Dropping activity coin!!!");
                    
                    InstancingUtilities.InstantiateCurrency(coinToDrop, lootOwner, GameObject, Transform.Position);
                }
            }
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.Write((uint) Participants.Count);

            foreach (var contributor in Participants)
            {
                writer.Write(contributor);

                foreach (var parameter in Parameters) writer.Write(parameter);
            }
        }
    }
}