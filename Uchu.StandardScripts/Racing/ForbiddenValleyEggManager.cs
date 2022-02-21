using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

[ZoneSpecific(1403)]
public class ForbiddenValleyEggManager : NativeScript
{
    public override Task LoadAsync()
    {
        Listen(Zone.OnStart, () =>
        {
            if (!Zone.ZoneControlObject.TryGetComponent<RacingControlComponent>(out var racingControlComponent)) return;
            var eggs = Zone.GameObjects.Where(i => i.Lot == 11406 || i.Lot == 11405).ToList();
            Task.Run(async () =>
            {
                while (racingControlComponent != null)
                {
                    await Task.Delay(1000);
                    if (racingControlComponent.Players.Any(p => p.RespawnIndex is >= 10 and < 14)) continue;
                    foreach (var egg in eggs)
                    {
                        if (!egg.TryGetComponent<DestructibleComponent>(out var destructibleComponent) || destructibleComponent.Alive) return;
                        await destructibleComponent.ResurrectAsync();
                    }
                }
            });
        });
        return Task.CompletedTask;
    }
}