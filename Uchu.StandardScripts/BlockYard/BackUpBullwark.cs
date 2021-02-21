using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.BlockYard
{
    [ZoneSpecific(1150)]
    public class BackUpBullwark : NativeScript
    {
        public override Task LoadAsync()
        {
            // Progresses the Back-up Bullwark mission
            Listen(Zone.OnPlayerLoad, async player =>
            {
                if (player.TryGetComponent<MissionInventoryComponent>(out var missions))
                        await missions.ScriptAsync(5652);
            });
        
            return Task.CompletedTask;
        }
    }
}
