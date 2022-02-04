using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Scripting.Utils;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    public class ScriptedActivityComponent : StructReplicaComponent<ScriptedActivitySerialization>
    {
        private readonly Random _random;
        
        public readonly List<GameObject> Participants = new List<GameObject>();

        public List<float[]> Parameters { get; set; } = new List<float[]>();

        public override ComponentId Id => ComponentId.ScriptedActivityComponent;

        public Activities ActivityInfo { get; protected set; }
        
        public ActivityRewards[] Rewards { get; protected set; }

        /// <summary>
        /// Creates the scripted activity component.
        /// </summary>
        protected ScriptedActivityComponent()
        {
            _random = new Random();
            
            Listen(OnStart, async () =>
            {
                if (!GameObject.Settings.TryGetValue("activityID", out var id))
                {
                    return;
                }

                // Get the activity info.
                var activityId = (int) id;
                ActivityInfo = await ClientCache.FindAsync<Activities>(activityId);
                if (ActivityInfo == default)
                {
                    Logger.Error($"{GameObject} has an invalid activityID: {activityId}");
                    return;
                }

                // Get and sort the activities.
                Rewards = ClientCache.FindAll<ActivityRewards>(activityId).ToSortedList((a, b) =>
                {
                    if (a.ChallengeRating != b.ChallengeRating) return (a.ChallengeRating ?? 1) - (b.ChallengeRating ?? 1);
                    return (a.ActivityRating ?? -1) - (b.ActivityRating ?? -1);
                }).ToArray();
            });
        }

        /// <summary>
        /// Drops loot for the activity.
        /// </summary>
        /// <param name="lootOwner">Owner of the loot.</param>
        /// <param name="autoAddCurrency">Whether to automatically add currency to the player instead of dropping.</param>
        /// <param name="autoAddItems">Whether to automatically add items to the player instead of dropping.</param>
        public async Task DropLootAsync(Player lootOwner, bool autoAddCurrency = false, bool autoAddItems = false)
        {
            // Get the highest activity reward.
            ActivityRewards reward = null;
            var playerScore = this.GetParameter(lootOwner, 1);
            foreach (var activityRewards in this.Rewards)
            {
                if (playerScore < activityRewards.ActivityRating) continue;
                if (reward != default && reward.ActivityRating > activityRewards.ActivityRating) continue;
                reward = activityRewards;
            }
            if (reward == default) return;
            
            // Award the items.
            if (reward.LootMatrixIndex.HasValue)
            {
                var matrices = ClientCache.FindAll<Core.Client.LootMatrix>(reward.LootMatrixIndex);
                foreach (var matrix in matrices)
                {
                    // Determine the count.
                    var count = _random.Next(matrix.MinToDrop ?? 0, matrix.MaxToDrop ?? 0);

                    // Start adding the items.
                    var itemsToAdd = new Dictionary<Lot, uint>();
                    var items = ClientCache.FindAll<LootTable>(matrix.LootTableIndex).ToList();
                    for (var i = 0; i < count; i++)
                    {
                        if (items.Count == default) break;
                    
                        // Return if the chance was not satisfied.
                        var proc = _random.NextDouble();
                        if (!(proc <= matrix.Percent)) continue;

                        // Get the item to add.
                        var item = items[_random.Next(0, items.Count)];
                        if (item.Itemid == null) continue;
                    
                        // Convert the LOT if it is a faction token.
                        Lot lot = item.Itemid ?? 0;
                        var character = lootOwner.GetComponent<CharacterComponent>();
                        if (lot == Lot.FactionTokenProxy)
                        {
                            if (character.IsAssembly) lot = Lot.AssemblyFactionToken;
                            if (character.IsParadox) lot = Lot.ParadoxFactionToken;
                            if (character.IsSentinel) lot = Lot.SentinelFactionToken;
                            if (character.IsVentureLeague) lot = Lot.VentureFactionToken;
                            if (item.Itemid == lot) continue;
                        }
                    
                        // Either drop or prepare to add the item.
                        if (!autoAddItems)
                        {
                            var drop = InstancingUtilities.InstantiateLoot(lot, lootOwner, GameObject, Transform.Position);
                            Start(drop);
                        }
                        else
                        {
                            itemsToAdd[lot] = (itemsToAdd.ContainsKey(lot) ? itemsToAdd[lot] + 1 : 1);
                        }
                    }
                    
                    // Add the items.
                    var inventory = lootOwner.GetComponent<InventoryManagerComponent>();
                    foreach (var (lot, total) in itemsToAdd)
                    {
                        await inventory.AddLotAsync(lot, total);
                    }
                }
            }
            
            // Add the coins.
            if (reward.CurrencyIndex.HasValue)
            {
                var currencies = ClientCache.FindAll<CurrencyTable>(reward.CurrencyIndex);
                foreach (var currency in currencies)
                {
                    if (currency.Npcminlevel > reward.ChallengeRating) continue;
                    var coinToDrop = _random.Next(currency.Minvalue ?? 0, currency.Maxvalue ?? 0);
                    if (autoAddCurrency)
                    {
                        var character = lootOwner.GetComponent<CharacterComponent>();
                        character.Currency += coinToDrop;
                    }
                    else
                    {
                        InstancingUtilities.InstantiateCurrency(coinToDrop, lootOwner, GameObject, Transform.Position);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a player to the activity.
        /// </summary>
        /// <param name="player">Player to add.</param>
        public void AddPlayer(Player player)
        {
            // Add the player.
            if (this.Participants.Contains(player)) return;
            this.Participants.Add(player);
            this.Parameters.Add(new float[10]);

            // Make sure the player will keep receiving updates for this object
            // even when they're far away while playing the footrace.
            player.Perspective.RenderDistanceFilter?.AddBypassFilter(this.GameObject);

            // Serialize the object.
            GameObject.Serialize(this.GameObject);
        }
        
        /// <summary>
        /// Removes a player from the activity.
        /// </summary>
        /// <param name="player">Player to remove.</param>
        public void RemovePlayer(Player player)
        {
            // Remove the player.
            if (!this.Participants.Contains(player)) return;
            var index = this.Participants.IndexOf(player);
            this.Participants.Remove(player);
            this.Parameters.RemoveAt(index);
            
            // Serialize the object.
            GameObject.Serialize(this.GameObject);

            // Remove the override that was added to ensure the player
            // receives updates while playing the footrace.
            player.Perspective.RenderDistanceFilter?.RemoveBypassFilter(this.GameObject);
        }

        /// <summary>
        /// Gets a parameter for a player.
        /// </summary>
        /// <param name="player">Player to get.</param>
        /// <param name="index">Index to get.</param>
        public float GetParameter(Player player, int index)
        {
            if (!this.Participants.Contains(player)) return 0;
            return this.Parameters[this.Participants.IndexOf(player)][index];
        }

        /// <summary>
        /// Sets a parameter for a player.
        /// </summary>
        /// <param name="player">Player to set.</param>
        /// <param name="index">Index to set.</param>
        /// <param name="value">Value to set.</param>
        public void SetParameter(Player player, int index, float value)
        {
            if (!this.Participants.Contains(player)) return;
            this.Parameters[this.Participants.IndexOf(player)][index] = value;
            GameObject.Serialize(this.GameObject);
        }

        /// <summary>
        /// Creates the Construct packet for the replica component.
        /// </summary>
        /// <returns>The Construct packet for the replica component.</returns>
        public override ScriptedActivitySerialization GetConstructPacket()
        {
            var packet = base.GetConstructPacket();
            packet.ActivityUserInfos = new ActivityUserInfo[Participants.Count];
            for (var i = 0; i < this.Participants.Count; i++)
            {
                var participant = this.Participants[i];
                var parameters = Parameters[i];
                var activityUserInfo = new ActivityUserInfo
                {
                    User = participant,
                    ActivityValue0 = parameters[0],
                    ActivityValue1 = parameters[1],
                    ActivityValue2 = parameters[2],
                    ActivityValue3 = parameters[3],
                    ActivityValue4 = parameters[4],
                    ActivityValue5 = parameters[5],
                    ActivityValue6 = parameters[6],
                    ActivityValue7 = parameters[7],
                    ActivityValue8 = parameters[8],
                    ActivityValue9 = parameters[9],
                };
                packet.ActivityUserInfos[i] = activityUserInfo;
            }

            return packet;
        }

        /// <summary>
        /// Creates the Serialize packet for the replica component.
        /// </summary>
        /// <returns>The Serialize packet for the replica component.</returns>
        public override ScriptedActivitySerialization GetSerializePacket()
        {
            var packet = this.GetConstructPacket();
            return packet;
        }
    }
}
