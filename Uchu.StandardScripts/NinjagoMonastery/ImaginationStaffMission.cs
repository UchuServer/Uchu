using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ZoneSpecific(2000)]
    public class ImaginationStaffMission : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnSkillEvent, async target =>
                {
                    if (target.Lot != 13789) return; // Sensei Wu
                    if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory)) return;

                    await missionInventory.ScriptAsync(2543, 13789);
                });
            });
            return Task.CompletedTask;
        }
    }
}