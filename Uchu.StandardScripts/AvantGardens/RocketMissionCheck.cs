using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(ZoneId.AvantGardens)]
    public class RocketMissionCheck : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, async player =>
            {
                var character = await player.GetCharacterAsync();
                
                if (character.LaunchedRocketFrom != ZoneId.VentureExplorer) return;

                var questInventory = player.GetComponent<MissionInventoryComponent>();

                await questInventory.ScriptAsync(5652);
            });
            
            return Task.CompletedTask;
        }
    }
}