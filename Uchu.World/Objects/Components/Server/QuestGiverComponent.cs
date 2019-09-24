using System;
using System.Linq;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [ServerComponent(Id = ReplicaComponentsId.QuestGiver)]
    public class QuestGiverComponent : Component
    {
        public QuestGiverComponent()
        {
            OnStart += CollectMissions;
        }

        public MissionNPCComponent[] MissionComponents { get; private set; }

        public Missions[] Quests { get; set; }

        private void CollectMissions()
        {
            using (var ctx = new CdClientContext())
            {
                var components = ctx.ComponentsRegistryTable.Where(
                    c => c.Id == GameObject.Lot && c.Componenttype == (int) ReplicaComponentsId.QuestGiver
                ).ToArray();

                MissionComponents = components.SelectMany(
                    component => ctx.MissionNPCComponentTable.Where(m => m.Id == component.Componentid)
                ).ToArray();

                Quests = MissionComponents.SelectMany(
                    component => ctx.MissionsTable.Where(m => m.Id == component.MissionID)
                ).ToArray();
            }

            Logger.Information($"{GameObject} is a quest give with {MissionComponents.Length} missions");
        }

        public void OfferMission(Player player)
        {
            var questInventory = player.GetComponent<QuestInventory>();

            foreach (var component in MissionComponents)
            {
                //
                // Get all of the missions the player has active, completed, or otherwise interacted with.
                // I.e Missions not started will not be included.
                //

                var playerMissions = questInventory.GetMissions();

                foreach (var quest in Quests.Where(q => q.Id == component.MissionID))
                {
                    if (!(quest.IsMission ?? false)) continue; // Is achievement

                    // Get the quest id.
                    if (quest.Id == default) continue;
                    var questId = quest.Id.Value;

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
                            //
                            // Offer mission hand in to the player.
                            //

                            questInventory.MessageOfferMission(questId, GameObject);

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
                            case MissionState.ReadyToComplete:
                            case MissionState.Unavailable:
                            case MissionState.Completed:
                            case MissionState.CompletedActive:
                            case MissionState.CompletedReadyToComplete:
                                return;
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
                        quest.PrereqMissionID,
                        questInventory.GetCompletedMissions()
                    );

                    if (!hasPrerequisite) continue;

                    //
                    // Offer new mission to the player.
                    //

                    questInventory.MessageOfferMission(questId, GameObject);

                    return;
                }
            }
        }
    }
}