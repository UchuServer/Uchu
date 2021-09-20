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
        public override Task LoadAsync()
        {
            List<Player> dead = new List<Player>();
            Dictionary<int, int> heights = new Dictionary<int, int>(){
                {1400, -200},
                {1800, -100},
                //if we decide that -100 is fine for every world, switch this to a
                //list containing just the zoneids, replace the if check below with
                //heights.contains, and replace height in the listen statements
                //with -100
            };
            if (!heights.ContainsKey(Zone.ZoneId))
            {
                return Task.CompletedTask;
            }
            var height = heights[Zone.ZoneId];
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