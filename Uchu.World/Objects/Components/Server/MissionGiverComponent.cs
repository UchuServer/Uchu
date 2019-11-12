using System;
using System.Collections.Generic;
using System.Linq;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    [ServerComponent(Id = ComponentId.MissionNPCComponent)]
    public class MissionGiverComponent : Component
    {
        public readonly AsyncEvent<(int missionId, bool isComplete, MissionState state, GameObject responder)>
            OnMissionOk = new AsyncEvent<(int missionId, bool isComplete, MissionState state, GameObject responder)>();

        protected MissionGiverComponent()
        {
            OnStart.AddListener(() =>
            {
                CollectMissions();

                GameObject.OnInteract.AddListener(OfferMission);
            });
        }

        public (Missions, MissionNPCComponent)[] Missions { get; set; }

        private void CollectMissions()
        {
            using (var ctx = new CdClientContext())
            {
                var components = ctx.ComponentsRegistryTable.Where(
                    c => c.Id == GameObject.Lot && c.Componenttype == (int) ComponentId.MissionNPCComponent
                ).ToArray();

                var missionComponents = components.SelectMany(
                    component => ctx.MissionNPCComponentTable.Where(m => m.Id == component.Componentid)
                ).ToArray();

                var missions = new List<(Missions, MissionNPCComponent)>();
                
                foreach (var npcComponent in missionComponents)
                {
                    var quest = ctx.MissionsTable.FirstOrDefault(m => m.Id == npcComponent.MissionID);

                    if (quest == default)
                    {
                        Logger.Warning($"{GameObject} has a Mission NPC Component with no corresponding quest: [{npcComponent.Id}] {npcComponent.MissionID}");
                        continue;
                    }
                    
                    missions.Add((quest, npcComponent));
                }

                Missions = missions.ToArray();
            }

            Logger.Information(
                $"{GameObject} is a quest give with: {string.Join(" ", Missions.Select(s => s.Item1.Id))}"
            );
        }

        public void OfferMission(Player player)
        {
            var missionInventory = player.GetComponent<MissionInventoryComponent>();

            player.SendChatMessage($"\n\n\nInteracting with {GameObject.ClientName} [{Missions.Length}]\n");

            try
            {
                foreach (var (mission, component) in Missions)
                {
                    //
                    // Get all of the missions the player has active, completed, or otherwise interacted with.
                    // I.e Missions not started will not be included.
                    //

                    var playerMissions = missionInventory.GetMissions();

                    player.SendChatMessage($"Checking: {mission.Id}");

                    // Get the quest id.
                    if (mission.Id == default) continue;
                    var questId = mission.Id.Value;

                    //
                    // See if the player has interacted with this mission and could passably hand it in.
                    //

                    var playerMission = playerMissions.FirstOrDefault(p => p.MissionId == questId);

                    MissionState missionState;

                    if (playerMission != default && (component.AcceptsMission ?? false))
                    {
                        missionState = (MissionState) playerMission.State;

                        //
                        // Check if the player can hand in any missions.
                        //

                        if (missionState == MissionState.ReadyToComplete)
                        {
                            player.SendChatMessage($"Can complete: {mission.Id}");

                            //
                            // Offer mission hand in to the player.
                            //

                            missionInventory.MessageOfferMission(questId, GameObject);

                            //
                            // Can only hand in one mission at a time.
                            //

                            return;
                        }
                    }

                    if (!(component.OffersMission ?? false)) continue;

                    if (playerMission != default)
                    {
                        missionState = (MissionState) playerMission.State;

                        switch (missionState)
                        {
                            //
                            // If the mission is available but not started for some reason the mission is ready to be pickup up.
                            //

                            case MissionState.Available:
                            case MissionState.CompletedAvailable:
                                break;

                            //
                            // If the mission is active in some way or unavailable the player cannot take on this mission.
                            //

                            case MissionState.Active:
                            case MissionState.CompletedActive:
                                player.GetComponent<MissionInventoryComponent>().MessageOfferMission(
                                    playerMission.MissionId,
                                    GameObject
                                );
                                
                                continue;
                            case MissionState.ReadyToComplete:
                            case MissionState.Unavailable:
                            case MissionState.Completed:
                            case MissionState.CompletedReadyToComplete:
                                player.SendChatMessage($"Unavailable mission: {mission.Id} [{missionState}]");
                                continue;
                            default:
                                throw new ArgumentOutOfRangeException(
                                    nameof(missionState), $"{missionState} is not a valid {nameof(MissionState)}"
                                );
                        }
                    }

                    //
                    // Check if player has completed the required missions to take on this new mission.
                    //

                    var hasPrerequisite = MissionParser.CheckPrerequiredMissions(
                        mission.PrereqMissionID,
                        missionInventory.GetCompletedMissions()
                    );

                    player.SendChatMessage($"Prerequisite for: {mission.Id} [{hasPrerequisite}]");

                    if (!hasPrerequisite) continue;

                    //
                    // Offer new mission to the player.
                    //

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