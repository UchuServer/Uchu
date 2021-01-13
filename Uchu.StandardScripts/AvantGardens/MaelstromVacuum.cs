using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class MaelstromVacuum : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnObject, obj =>
            {
                if (!(obj is AuthoredGameObject gameObject)) return;
                
                if (gameObject.Lot != 14596) return;
                
                if (!(gameObject.Author is Player player)) return;

                Delegate onFireServerEventDelegate = null;
                onFireServerEventDelegate = Listen(player.OnFireServerEvent, (name, message) =>
                {
                    if (name != "attemptCollection") return;
                    
                    if (message.Associate != gameObject) return;
                    
                    ReleaseListener(onFireServerEventDelegate);
                    
                    var samples = Zone.GameObjects.Where(g => g.Lot == 14718).ToList();

                    var position = gameObject.Transform.Position;

                    samples.Sort((a, b) => (int) (
                        Vector3.Distance(position, a.Transform.Position) -
                        Vector3.Distance(position, b.Transform.Position)
                    ));

                    var selected = samples.FirstOrDefault();

                    if (selected == default) return;

                    if (Vector3.Distance(position, selected.Transform.Position) > 15) return;

                    var smashable = selected.GetComponent<DestructibleComponent>();
                    
                    Task.Run(async () =>
                    {
                        gameObject.Animate("collect_maelstrom");
                        
                        await Task.Delay(4000);

                        await smashable.SmashAsync(gameObject, gameObject.Author as Player);
                    });
                });

                Delegate onReadyForUpdatesEventDelegate = null;
                onReadyForUpdatesEventDelegate = Listen(player.OnReadyForUpdatesEvent, (message) =>
                {
                    if (message.GameObject.Lot != 14596) return;
                    
                    if (message.GameObject != gameObject) return;
                    
                    ReleaseListener(onReadyForUpdatesEventDelegate);
                    
                    message.GameObject.Animate("idle_maelstrom");
                });
            });
            
            return Task.CompletedTask;
        }
    }
}