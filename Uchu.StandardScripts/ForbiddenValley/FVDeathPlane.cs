using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.ForbiddenValley
{
    [ZoneSpecific(1400)]
    public class FVDeathPlane: NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {   
                var destructibleComponent = player.GetComponent<DestructibleComponent>();
                Listen(player.OnPositionUpdate, (position, rotation) =>
                {
                    if (position.Y < -100 && destructibleComponent.Alive)
                        destructibleComponent.SmashAsync(player);
                });
            });
            
            return Task.CompletedTask;
        }
    }
}