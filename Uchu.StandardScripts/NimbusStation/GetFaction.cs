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
                Listen(player.OnRespondToMission, async (missionID, playerObject, rewardItem)  => {
                    if (missionID != MissionID) return;

                    if (rewardItem.Id == -1) return; // If no reward chosen 

                    int[] Missions = new int[3];
                    int CelebrationID = -1;
                    int FactionFlag = 0;

                    if (rewardItem.Id == 6980) // Venture
                    {
                        Missions[0] = 555; //
                        Missions[1] = (int) Core.Resources.Missions.JoinVentureLeague; // Venture Missions
                        Missions[2] = (int) Core.Resources.Missions.JoinaFaction; //
                        CelebrationID = (int) Celebrations.JoinVenture; // Venture Celebration;
                        FactionFlag = 46; // Venture Faction Flag
                    }
                    else if (rewardItem.Id == 6979) // Assembly
                    {
                        Missions[0] = 544; // 
                        Missions[1] = (int) Core.Resources.Missions.JoinAssembly; // Assembly Missions
                        Missions[2] = (int) Core.Resources.Missions.JoinaFaction; //
                        CelebrationID = (int) Celebrations.JoinAssembly; // Assembly Celebration;
                        FactionFlag = 47; // Assembly Faction Flag
                    } 
                    else if (rewardItem.Id == 6981) // Paradox
                    {
                        Missions[0] = 577; // 
                        Missions[1] = (int) Core.Resources.Missions.JoinTheSentinels; // Paradox Missions
                        Missions[2] = (int) Core.Resources.Missions.JoinaFaction; // 
                        CelebrationID = (int) Celebrations.JoinParadox; // Paradox Celebration
                        FactionFlag = 48; // Paradox Faction Flag
                    }  
                    else if (rewardItem.Id == 6978) // Sentinel
                    {
                        Missions[0] = 566; // Sentinel Missions
                        Missions[1] = (int) Core.Resources.Missions.JoinTheSentinels;
                        Missions[2] = (int) Core.Resources.Missions.JoinaFaction;
                        CelebrationID = (int) Celebrations.JoinSentinels; // Sentinel Celebration
                        FactionFlag = 49; // Sentinel Faction Flag
                    }

                    if (CelebrationID != -1)
                    {
                        // Play effect
                        await player.TriggerCelebration(CelebrationID);
                    }

                    MissionInventoryComponent MissionInventory = player.GetComponent<MissionInventoryComponent>(); 

                    foreach (int item in Missions)
                    {
                        await MissionInventory.CompleteMissionAsync(item);
                    }

                    await player.SetFlagAsync(FactionFlag, true);
                });
            });

            return Task.CompletedTask;
        }
    }
}