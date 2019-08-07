using System.Linq;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class QuestGiverComponent : ReplicaComponent
    {
        public MissionNPCComponent[] MissionComponents { get; private set; }

        public Missions[] Quests { get; set; }

        public override ReplicaComponentsId Id => ReplicaComponentsId.QuestGiver;

        public override void FromLevelObject(LevelObject levelObject)
        {
            Logger.Information($"{levelObject.LOT} is Quest Giver");
            using (var ctx = new CdClientContext())
            {
                var components = ctx.ComponentsRegistryTable.Where(
                    c => c.Id == GameObject.Lot && c.Componenttype == (int) Id
                ).ToArray();

                MissionComponents = components.SelectMany(
                    component => ctx.MissionNPCComponentTable.Where(m => m.Id == component.Componentid)
                ).ToArray();

                Quests = MissionComponents.SelectMany(
                    component => ctx.MissionsTable.Where(m => m.Id == component.MissionID)
                ).ToArray();
            }
        }

        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
        }

        public async Task OfferMissionAsync(Player player)
        {
            var questInventory = player.GetComponent<QuestInventory>();

            foreach (var component in MissionComponents)
            {
                foreach (var quest in Quests.Where(q => q.Id == component.MissionID))
                {
                    if (quest.IsMission ?? false) continue; // Is achievement

                    var playerMissions = questInventory.GetMissions();

                    if (!(component.AcceptsMission ?? false))
                    {
                        if (playerMissions.Where(mission => mission.MissionId == quest.Id)
                            .Any(mission => mission.State == (int) MissionState.ReadyToComplete))
                        {
                            questInventory.MessageOfferMission(quest.Id ?? 0, GameObject);

                            return;
                        }
                    }

                    if (component.OffersMission ?? false) continue;
                    if (playerMissions.Any(m => m.MissionId == quest.Id) &&
                        playerMissions.First(m => m.MissionId == quest.Id).State != (int) MissionState.Active &&
                        playerMissions.First(m => m.MissionId == quest.Id).State != (int) MissionState.ReadyToComplete)
                        continue;

                    if (!await MissionParser.CheckPrerequiredMissionsAsync(quest.PrereqMissionID,
                        questInventory.GetCompletedMissions()))
                    {
                        continue;
                    }

                    questInventory.MessageOfferMission(quest.Id ?? 0, GameObject);

                    return;
                }
            }
        }
    }
}