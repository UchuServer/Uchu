using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class BusDoor : NativeScript
    {
        public override Task LoadAsync()
        {
            var gameObjects = HasLuaScript("l_ag_bus_door.lua");

            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.TryGetComponent<MovingPlatformComponent>(out var movingPlatformComponent)) continue;
                movingPlatformComponent.Stop();
                
                bool doorOpen = default;
                Listen(Zone.OnTick, () =>
                {
                    if (movingPlatformComponent.State != PlatformState.Idle) return;
                    
                    var playerInRadius = false;
                    foreach (var player in Zone.Players)
                    {
                        if (player?.Transform == default) continue;

                        var radius = Vector3.Distance(player.Transform.Position, gameObject.Transform.Position);
                        if (radius > 85) continue;
                        
                        playerInRadius = true;
                        break;
                    }

                    if (doorOpen != playerInRadius)
                    {
                        doorOpen = playerInRadius;
                        if (doorOpen)
                        {
                            movingPlatformComponent.MoveTo(1);
                        }
                        else
                        {
                            movingPlatformComponent.MoveTo(0, () =>
                            {
                                gameObject.PlayFX("busDust", "create", 642);
                            });
                        }
                    }
                });
            }

            return Task.CompletedTask;
        }
    }
}