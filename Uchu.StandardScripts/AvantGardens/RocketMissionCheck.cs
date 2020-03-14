using System.Threading.Tasks;
using Uchu.Core;
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
                var character = await player.GetCharacterAsync();
                
                if (character.LaunchedRocketFrom != 1000) return;

                var questInventory = player.GetComponent<MissionInventoryComponent>();

                await questInventory.ScriptAsync(5652);
            });
            
            return Task.CompletedTask;
        }
    }
}