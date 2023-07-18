using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General.DeathPlane
{
    public class WorldDeathPlane: NativeScript
    {
        private static readonly Dictionary<int, int> Heights = new(){
            {1300, 80},
            {1400, -200},
            {1800, -100},
        };
        public override Task LoadAsync()
        {
            List<Player> dead = new List<Player>();
            if (!Heights.ContainsKey(Zone.ZoneId))
            {
                return Task.CompletedTask;
            }
            var height = Heights[Zone.ZoneId];
            Listen(Zone.OnPlayerLoad, player =>
            {   
                var destructibleComponent = player.GetComponent<DestructibleComponent>();
                Listen(player.OnPositionUpdate, (position, rotation) =>
                {
                    if (position.Y < height && destructibleComponent.Alive && !dead.Contains(player))
                    {
                        dead.Add(player);
                        destructibleComponent.SmashAsync(player);
                    }
                });
                Listen(destructibleComponent.OnResurrect, async () => 
                {
                    await Task.Delay(1000);
                    dead.Remove(player);
                });
            });
            
            return Task.CompletedTask;
        }
    }
}