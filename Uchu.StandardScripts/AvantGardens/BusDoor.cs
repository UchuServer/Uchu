using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Physics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class BusDoor : NativeScript
    {
        private List<Player> _playersInRadius = new List<Player>();
        private bool _doorOpen = false;
        
        public override Task LoadAsync()
        {
            var gameObjects = HasLuaScript("l_ag_bus_door.lua");

            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.TryGetComponent<MovingPlatformComponent>(out var movingPlatformComponent)) continue;
                movingPlatformComponent.Stop();
                
                var physics = gameObject.AddComponent<PhysicsComponent>();

                var physicsObject = CylinderBody.Create(
                    gameObject.Zone.Simulation,
                    gameObject.Transform.Position,
                    gameObject.Transform.Rotation,
                    new Vector2(85, 190));
                
                physics.SetPhysics(physicsObject);

                // Set up players entering and leaving.
                Listen(physics.OnEnter, (other) =>
                {
                    if (!(other.GameObject is Player player)) return;
                    if (_playersInRadius.Contains(player)) return;
                    _playersInRadius.Add(player);
                    UpdateDoor(gameObject);
                });
                
                Listen(physics.OnLeave, (other) =>
                {
                    if (!(other.GameObject is Player player)) return;
                    if (!_playersInRadius.Contains(player)) return;
                    _playersInRadius.Remove(player);
                    UpdateDoor(gameObject);
                });

                // Set up player disconnecting so that players disconnecting near the door don't keep it open.
                Listen(Zone.OnPlayerLoad, (player) =>
                {
                    Listen(player.OnDestroyed, () =>
                    {
                        if (!_playersInRadius.Contains(player)) return;
                        _playersInRadius.Remove(player);
                        UpdateDoor(gameObject);
                    });
                });
            }

            return Task.CompletedTask;
        }

        private void UpdateDoor(GameObject door)
        {
            // Return if the door is moving.
            // If the intended open state changed, it will be updated when the move ends.
            if (!door.TryGetComponent<MovingPlatformComponent>(out var movingPlatformComponent)) return;
            if (movingPlatformComponent.State != PlatformState.Idle) return;
            
            // Return if the door is already open to reduce updates.
            var doorShouldBeOpen = _playersInRadius.Count > 0;
            if (doorShouldBeOpen == _doorOpen) return;
            _doorOpen = doorShouldBeOpen;
            
            // Open or close the door, then update again in case the intended state changed mid-movement.
            if (_doorOpen)
            {
                movingPlatformComponent.MoveTo(1, () =>
                {
                    UpdateDoor(door);
                });
            }
            else
            {
                movingPlatformComponent.MoveTo(0, () =>
                {
                    door.PlayFX("busDust", "create", 642);
                    UpdateDoor(door);
                });
            }
        }
    }
}