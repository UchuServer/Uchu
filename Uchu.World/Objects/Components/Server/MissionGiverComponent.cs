using System;
using System.Collections.Generic;
using System.Linq;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    /// <summary>
    /// Component responsible for giving missions to the player
    /// </summary>
    [ServerComponent(Id = ComponentId.MissionNPCComponent)]
    public class MissionGiverComponent : Component
    {
        public MissionGiverComponent()
        {
            OnMissionOk = new Event<(int, bool, MissionState, GameObject)>();
            
            Listen(OnStart, () =>
            {
                CollectMissions();
                Listen(GameObject.OnInteract, HandleInteraction);
            });
        }

        /// <summary>
        /// Event that's called when a player accepts a mission
        /// </summary>
        public Event<(int missionId, bool isComplete, MissionState state, GameObject responder)> OnMissionOk { get; }
        
        /// <summary>
        /// All missions this giver can offer
        /// </summary>
        public (Missions, MissionNPCComponent)[] Missions { get; set; }

        /// <summary>
        /// Randomizer for selecting random messages.
        /// </summary>
        private Random _random = new Random();

        /// <summary>
        /// Finds all the missions that this giver may offer and stores them
        /// </summary>
        private void CollectMissions()
        {
            var components = ClientCache.FindAll<ComponentsRegistry>(GameObject.Lot).Where(
                c => c.Componenttype == (int) ComponentId.MissionNPCComponent
            ).ToArray();

            var missionComponents = components.SelectMany(
                component => ClientCache.FindAll<MissionNPCComponent>(component.Componentid)
            ).ToArray();

            var missions = new List<(Missions, MissionNPCComponent)>();
            
            foreach (var npcComponent in missionComponents)
            {
                var quest = ClientCache.Find<Missions>(npcComponent.MissionID);

                if (quest == default)
                {
                    Logger.Warning($"{GameObject} has a Mission NPC Component with no corresponding quest: \"[{GameObject.Lot}] {GameObject.Name}\" [{npcComponent.Id}] {npcComponent.MissionID}");
                    continue;
                }
                
                missions.Add((quest, npcComponent));
            }

            Missions = missions.ToArray();

            Logger.Information(
                $"{GameObject} is a quest give with: {string.Join(" ", Missions.Select(s => s.Item1.Id))}"
            );
        }

        /// <summary>
        /// Gets the id of a mission to offer.
        /// </summary>
        /// <param name="missionInventory">Mission inventory to give a mission for.</param>
        /// <returns>The mission id to offer.</returns>
        private Missions GetMissionToOffer(MissionInventoryComponent missionInventory)
        {
            Missions questToOffer = null;
            foreach (var (mission, component) in Missions)
            {
                // Get the quest id.
                if (mission.Id == default)
                    continue;
                
                // If the player is ready to hand this mission in, allow them to complete the mission.
                var questId = mission.Id.Value;
                var playerMission = missionInventory.GetMission(questId);
                if (playerMission != default && (component.AcceptsMission ?? false) && (playerMission.State == MissionState.ReadyToComplete || playerMission.State == MissionState.CompletedReadyToComplete))
                {
                    return mission;
                }

                // Ignore the mission if the component can't offer a mission.
                if (!(component.OffersMission ?? false))
                    continue;

                // Change or return a mission id depending on the state of the mission.
                if (playerMission != default)
                {
                    switch (playerMission.State)
                    {
                        case MissionState.Available:
                        case MissionState.CompletedAvailable:
                            // If this is a mission a player hasn't started yet, but somehow has in their inventory
                            // Allow them to start it
                            break;
                        case MissionState.Active:
                            // Display the in-progress mission instead of possibly continuing and offering a new mission.
                            return mission;
                        case MissionState.CompletedActive:
                            // If this is an active mission show the offer popup again for information.
                            return mission;
                        case MissionState.ReadyToComplete:
                        case MissionState.CompletedReadyToComplete:
                        case MissionState.Unavailable:
                        case MissionState.Completed:
                            // Allow the mission if it is repeatable and the cooldown period has passed.
                            if (!playerMission.CanRepeat) continue;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(playerMission.State), $@"{playerMission.State} is not a valid {nameof(MissionState)}"
                            );
                    }
                }
                
                // Ignore the mission if the prerequisites aren't met.
                if (!MissionParser.CheckPrerequiredMissions(mission.PrereqMissionID, missionInventory.AllMissions))
                    continue;

                // Set the mission as the mission to offer.
                // The mission is not offered directly in cases where an Available mission comes up before a ReadyToComplete mission.
                if (questToOffer != default) continue;
                questToOffer = mission;
            }
            
            // Return the final quest to offer.
            // This is reached if there are no completed missions to complete.
            return questToOffer;
        }
        
        /// <summary>
        /// Gets the id of a mission to offer.
        /// </summary>
        /// <param name="missionInventory">Mission inventory to give a mission for.</param>
        /// <returns>The mission id to offer.</returns>
        public int GetIdMissionToOffer(MissionInventoryComponent missionInventory)
        {
            // Get the mission.
            var mission = this.GetMissionToOffer(missionInventory);
            if (mission == null) return default;
            
            // Return a random mission.
            if ((mission.IsRandom ?? false) && !string.IsNullOrEmpty(mission.RandomPool))
            {
                var randomMissions = mission.RandomPool.Split(",");
                if (int.TryParse(randomMissions[this._random.Next(randomMissions.Length)], out var randomMissionId))
                {
                    return randomMissionId;
                }
            }
            
            // Return the id.
            return mission.Id ?? default;
        }

        /// <summary>
        /// Handles the interaction between a mission giver and a player, completing any missions ready to complete or offering
        /// new missions a player may start.
        /// </summary>
        /// <param name="player">The player that interacted with the mission giver</param>
        /// <exception cref="ArgumentOutOfRangeException">If an invalid mission state was provided</exception>
        public void HandleInteraction(Player player)
        {
            try
            {
                // Get the mission to offer and offer it if one exists.
                var missionInventory = player.GetComponent<MissionInventoryComponent>();
                var questIdToOffer = this.GetIdMissionToOffer(missionInventory);
                if (questIdToOffer == default) return;
                missionInventory.MessageOfferMission(questIdToOffer, GameObject);
            }
            catch (Exception e)
            {
                // Log the error (most likely network related).
                Logger.Error(e);
            }
        }
    }
}