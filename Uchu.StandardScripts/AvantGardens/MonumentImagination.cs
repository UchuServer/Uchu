using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class MonumentImagination : NativeScript
    {
        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects)
            {
                Mount(gameObject);
            }

            Listen(Zone.OnObject, (obj) =>
            {
                if (!(obj is GameObject gameObject)) return;
                
                Mount(gameObject);
            });

            return Task.CompletedTask;
        }

        private void Mount(GameObject gameObject)
        {
            if (gameObject.Lot != 1656) return;

            var active = true;
            Listen(Zone.OnTick, () =>
            {
                if (!active) return;
                    
                foreach (var player in Zone.Players)
                {
                    if (player?.Transform == default) continue;
                    if (!(Vector3.Distance(player.Transform.Position, gameObject.Transform.Position) < 2)) continue;
                    active = false;
                    
                    player.GetComponent<DestroyableComponent>().Imagination += 1;

                    Task.Run(async () =>
                    {
                        if (!gameObject.TryGetComponent<DestructibleComponent>(out var destructible)) return;
                        await destructible.SmashAsync(player);
                    });
                }
            });
        }
    }
}