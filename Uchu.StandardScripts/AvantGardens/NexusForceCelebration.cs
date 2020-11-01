using System.Threading.Tasks;
using Uchu.World.Scripting.Native;
using Uchu.World;
using Uchu.Core.Resources;

namespace Uchu.StandardScripts.AvantGardens
{
    public class NexusForceCelebration : NativeScript
    {
        int MissionID = 1851;
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player => {

                if (!player.TryGetComponent(out MissionInventoryComponent Component)) { return; }
                
                Listen(Component.OnCompleteMission, async (mission) =>
                {
                    int missionID = mission.MissionId;
                    if (missionID != MissionID) return;
                    
                    await player.TriggerCelebration((int) Celebrations.JoinNexusForce);
                });
            });

            return Task.CompletedTask;
        }
    }
}