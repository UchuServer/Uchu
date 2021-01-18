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
        protected MissionGiverComponent()
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
        /// Finds all the missions that this giver may offer and stores them
        /// </summary>
        private void CollectMissions()
        {
            var components = ClientCache.GetTable<ComponentsRegistry>().Where(
                c => c.Id == GameObject.Lot && c.Componenttype == (int) ComponentId.MissionNPCComponent
            ).ToArray();

            var missionComponents = components.SelectMany(
                component => ClientCache.GetTable<MissionNPCComponent>().Where(m => m.Id == component.Componentid)
            ).ToArray();

            var missions = new List<(Missions, MissionNPCComponent)>();
            
            foreach (var npcComponent in missionComponents)
            {
                var quest = ClientCache.GetTable<Missions>().FirstOrDefault(m => m.Id == npcComponent.MissionID);

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
        /// Handles the interaction between a mission giver and a player, completing any missions ready to complete or offering
        /// new missions a player may start.
        /// </summary>
        /// <param name="player">The player that interacted with the mission giver</param>
        /// <exception cref="ArgumentOutOfRangeException">If an invalid mission state was provided</exception>
        public void HandleInteraction(Player player)
        {
            var missionInventory = player.GetComponent<MissionInventoryComponent>();

            try
            {
                foreach (var (mission, component) in Missions)
                {
                    // Get the quest id.
                    if (mission.Id == default)
                        continue;
                    
                    var questId = mission.Id.Value;
                    var playerMission = missionInventory.GetMission(questId);
                    
                    // If the player is ready to hand this mission in, allow them to complete the mission
                    if (playerMission != default && (component.AcceptsMission ?? false) && playerMission.State == MissionState.ReadyToComplete)
                    {
                            missionInventory.MessageOfferMission(questId, GameObject);
                            return;
                    }

                    if (!(component.OffersMission ?? false))
                        continue;

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
                            case MissionState.CompletedActive:
                                // If this is an active mission show the offer popup again for information
                                player.GetComponent<MissionInventoryComponent>().MessageOfferMission(
                                    playerMission.MissionId,
                                    GameObject
                                );
                                continue;
                            case MissionState.ReadyToComplete:
                            case MissionState.Unavailable:
                            case MissionState.Completed:
                            case MissionState.CompletedReadyToComplete:
                                // Any other missions are skipped
                                continue;
                            default:
                                throw new ArgumentOutOfRangeException(
                                    nameof(playerMission.State), $"{playerMission.State} is not a valid {nameof(MissionState)}"
                                );
                        }
                    }
                    
                    if (!MissionParser.CheckPrerequiredMissions(
                        mission.PrereqMissionID,
                        missionInventory.CompletedMissions))
                        continue;

                    // If this is a mission the player doesn't have yet or hasn't started yet, offer it
                    missionInventory.MessageOfferMission(questId, GameObject);
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}