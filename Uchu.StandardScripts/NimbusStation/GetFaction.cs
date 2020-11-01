using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.Core.Resources;
using Uchu.Core;

namespace Uchu.StandardScripts.NimbusStation
{
    [ZoneSpecific(1200)]
    public class GetFaction : NativeScript
    {
        int MissionID { get; set; } = 474;

        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player => {
                Listen(player.OnRespondToMission, async (missionId, playerObject, rewardItem)  => {
                    if (missionId != MissionID) return;

                    if (rewardItem.Id == -1) return; // If no reward chosen 

                    var missions = new int[3];
                    var celebrationId = -1;
                    var factionFlag = 0;

                    switch (rewardItem.Id)
                    {
                        // Venture
                        case 6980:
                            missions[0] = 555;
                            missions[1] = (int) Core.Resources.Mission.JoinVentureLeague;
                            missions[2] = (int) Core.Resources.Mission.JoinaFaction;
                            celebrationId = (int) Celebration.JoinVenture;
                            factionFlag = 46; // Venture Faction Flag
                            break;
                        // Assembly
                        case 6979:
                            missions[0] = 544; 
                            missions[1] = (int) Core.Resources.Mission.JoinAssembly;
                            missions[2] = (int) Core.Resources.Mission.JoinaFaction;
                            celebrationId = (int) Celebration.JoinAssembly;
                            factionFlag = 47; // Assembly Faction Flag
                            break;
                        // Paradox
                        case 6981:
                            missions[0] = 577; 
                            missions[1] = (int) Core.Resources.Mission.JoinTheSentinels;
                            missions[2] = (int) Core.Resources.Mission.JoinaFaction; 
                            celebrationId = (int) Celebration.JoinParadox;
                            factionFlag = 48; // Paradox Faction Flag
                            break;
                        // Sentinel
                        case 6978:
                            missions[0] = 566; // Sentinel Missions
                            missions[1] = (int) Core.Resources.Mission.JoinTheSentinels;
                            missions[2] = (int) Core.Resources.Mission.JoinaFaction;
                            celebrationId = (int) Celebration.JoinSentinels;
                            factionFlag = 49; // Sentinel Faction Flag
                            break;
                    }

                    var celebration = (Celebration) celebrationId;
                    if (celebration != Celebration.Invalid)
                    {
                        await player.TriggerCelebration(celebration);
                    }

                    var missionInventory = player.GetComponent<MissionInventoryComponent>();
                    foreach (var item in missions)
                    {
                        await missionInventory.CompleteMissionAsync(item);
                    }

                    await player.SetFlagAsync(factionFlag, true);
                });
            });

            return Task.CompletedTask;
        }
    }
}