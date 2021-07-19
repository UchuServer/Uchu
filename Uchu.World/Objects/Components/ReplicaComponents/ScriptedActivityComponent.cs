using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Core.Resources;
using Uchu.World.Client;

namespace Uchu.World
{
    public class ScriptedActivityComponent : ReplicaComponent
    {
        private readonly Random _random;
        
        public readonly List<GameObject> Participants = new List<GameObject>();

        public List<float[]> Parameters { get; set; } = new List<float[]>();

        public override ComponentId Id => ComponentId.ScriptedActivityComponent;

        public Activities ActivityInfo { get; private set; }
        
        public ActivityRewards[] Rewards { get; private set; }

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

                var activityId = (int) id;
                ActivityInfo = await ClientCache.FindAsync<Activities>(activityId);

                if (ActivityInfo == default)
                {
                    Logger.Error($"{GameObject} has an invalid activityID: {activityId}");
                    return;
                }

                Rewards = ClientCache.FindAll<ActivityRewards>(activityId);
            });
        }

        /// <summary>
        /// Drops loot for the activity.
        /// </summary>
        /// <param name="lootOwner">Owner of the loot.</param>
        public async Task DropLootAsync(Player lootOwner)
        {
            var matrices = Rewards.SelectMany(r =>
                ClientCache.FindAll<Core.Client.LootMatrix>(r.LootMatrixIndex)).ToArray();

            foreach (var matrix in matrices)
            {
                var count = _random.Next(matrix.MinToDrop ?? 0, matrix.MaxToDrop ?? 0);

                var items = ClientCache.FindAll<LootTable>(matrix.LootTableIndex).ToList();
                for (var i = 0; i < count; i++)
                {
                    if (items.Count == default) break;
                    
                    var proc = _random.NextDouble();

                    if (!(proc <= matrix.Percent)) continue;

                    var item = items[_random.Next(0, items.Count)];
                    items.Remove(item);

                    if (item.Itemid == null) continue;
                    
                    Lot lot = item.Itemid ?? 0;
                    var character = lootOwner.GetComponent<CharacterComponent>();
                    
                    if (lot == Lot.FactionTokenProxy)
                    {
                        if (character.IsAssembly) lot = Lot.AssemblyFactionToken;
                        if (character.IsParadox) lot = Lot.ParadoxFactionToken;
                        if (character.IsSentinel) lot = Lot.SentinelFactionToken;
                        if (character.IsVentureLeague) lot = Lot.VentureFactionToken;
                        if (item.Itemid == lot) return;
                    }
                    
                    var drop = InstancingUtilities.InstantiateLoot(lot, lootOwner, GameObject, Transform.Position);
                    
                    Start(drop);
                }
            }

            foreach (var reward in Rewards)
            {
                var currencies = ClientCache.FindAll<CurrencyTable>(reward.CurrencyIndex);
                foreach (var currency in currencies)
                {
                    if (currency.Npcminlevel > reward.ChallengeRating) continue;

                    var coinToDrop = _random.Next(currency.Minvalue ?? 0, currency.Maxvalue ?? 0);
                    
                    lootOwner.SendChatMessage("Dropping activity coin!!!");
                    
                    InstancingUtilities.InstantiateCurrency(coinToDrop, lootOwner, GameObject, Transform.Position);
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
        }

        /// <summary>
        /// Writes the Construct information.
        /// </summary>
        /// <param name="writer">Writer to write to.</param>
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        /// <summary>
        /// Writes the Serialize information.
        /// </summary>
        /// <param name="writer">Writer to write to.</param>
        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.Write((uint) Participants.Count);

            foreach (var contributor in Participants)
            {
                writer.Write(contributor);

                foreach (var parameter in Parameters[this.Participants.IndexOf(contributor)])
                    writer.Write(parameter);
            }
        }
    }
}