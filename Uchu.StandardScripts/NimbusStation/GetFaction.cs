using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.World.Systems.Missions;

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

                    if (rewardItem.Id == -1) return;

                    int[] Missions = new int[3];
                    int CelebrationID = -1;
                    int FactionFlag = 0;

                    if (rewardItem.Id == 6980) // Venture
                    {
                        Missions[0] = 555;
                        Missions[1] = 556;
                        Missions[2] = 778;
                        CelebrationID = 14;
                        FactionFlag = 46;
                    }
                    else if (rewardItem.Id == 6979) // Assembly
                    {
                        Missions[0] = 544;
                        Missions[1] = 545;
                        Missions[2] = 778;
                        CelebrationID = 15;
                        FactionFlag = 47;
                    } 
                    else if (rewardItem.Id == 6981) // Paradox
                    {
                        Missions[0] = 577;
                        Missions[1] = 578;
                        Missions[2] = 778;
                        CelebrationID = 16;
                        FactionFlag = 48;
                    }  
                    else if (rewardItem.Id == 6978) // Sentinel
                    {
                        Missions[0] = 566;
                        Missions[1] = 567;
                        Missions[2] = 778;
                        CelebrationID = 17;
                        FactionFlag = 49;
                    }

                    if (CelebrationID != -1)
                    {
                        // Play effect

                        var Celebration = (await new CdClientContext().CelebrationParametersTable.Where(t => t.Id == CelebrationID).ToArrayAsync())[0];

                        player.Message(new StartCelebrationEffectMessage
                        {
                            Animation = Celebration.Animation,
                            BackgroundObject = new Lot(Celebration.BackgroundObject.Value),
                            CameraPathLOT = new Lot(Celebration.CameraPathLOT.Value),
                            CeleLeadIn = Celebration.CeleLeadIn.Value,
                            CeleLeadOut = Celebration.CeleLeadOut.Value,
                            CelebrationID = Celebration.Id.Value,
                            Duration = Celebration.Duration.Value,
                            IconID = Celebration.IconID.Value,
                            MainText = Celebration.MainText,
                            MixerProgram = Celebration.MixerProgram,
                            MusicCue = Celebration.MusicCue,
                            PathNodeName = Celebration.PathNodeName,
                            SoundGUID = Celebration.SoundGUID,
                            SubText = Celebration.SubText
                        }); // Start effect

                        SetTimer(() =>
                        {
                            player.Message(new CelebrationCompletedMessage
                            {
                                CelebrationToFinishID = CelebrationID,
                                Animation = Celebration.Animation
                            }); // End effect
                        }, 4000);
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