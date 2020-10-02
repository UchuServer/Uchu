using System.Threading.Tasks;
using Uchu.World.Scripting.Native;
using Uchu.World;

namespace Uchu.StandardScripts.AvantGardens
{
    public class NexusForceCelebration : NativeScript
    {
        int MissionID = 1851;
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player => {

                player.TryGetComponent<MissionInventoryComponent>(out MissionInventoryComponent Component);
                
                Listen(Component.OnCompleteMission, async (mission) =>
                {
                    int missionID = mission.MissionId;
                    if (missionID != MissionID) return;

                    await player.TriggerCelebration(22);
                });
            });

            return Task.CompletedTask;
        }
    }
}