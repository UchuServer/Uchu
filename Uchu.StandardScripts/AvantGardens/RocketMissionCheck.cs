using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class RocketMissionCheck : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, async player =>
            {
                if (player.TryGetComponent<CharacterComponent>(out var character))
                {
                    if (character.LaunchedRocketFrom != 1000)
                        return;

                    if (player.TryGetComponent<MissionInventoryComponent>(out var missions))
                        await missions.ScriptAsync(5652);
                }
            });
            
            return Task.CompletedTask;
        }
    }
}