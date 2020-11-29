using System.Threading.Tasks;
using Uchu.World.Scripting.Native;
using Uchu.World;
using Uchu.Core.Resources;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class NexusForceCelebration : NativeScript
    {
        int MissionID = 1851;
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player => {

                if (!player.TryGetComponent(out MissionInventoryComponent component)) { return; }
                
                Listen(component.OnCompleteMission, async mission =>
                {
                    var missionId = mission.MissionId;
                    if (missionId != MissionID)
                        return;
                    
                    await player.TriggerCelebration(CelebrationId.JoinNexusForce);
                });
            });

            return Task.CompletedTask;
        }
    }
}