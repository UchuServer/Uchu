using System.Threading.Tasks;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    public class NexusForceCelebration : NativeScript
    {
        int MissionID = 1851;
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player => {
                Listen(player.OnRespondToMission, async (missionID, playerObject, rewardItem) =>
                {
                    if (missionID != MissionID) return;

                    await player.TriggerCelebration(22);
                });
            });

            return Task.CompletedTask;
        }
    }
}