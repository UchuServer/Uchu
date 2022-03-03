using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

[ScriptName("ScriptComponent_1330_script_name__removed")]
public class ForbiddenValleyColumns : ObjectScript
{
    public ForbiddenValleyColumns(GameObject gameObject) : base(gameObject)
    {
        var movingPlatformComponent = gameObject.GetComponent<MovingPlatformComponent>();
        var lap = gameObject.Lot == 11306 ? 1: 2;
        var activated = false;
        movingPlatformComponent.MoveTo(0);
        Task.Run(async () =>
        {
            await Task.Delay(7000);
            if (!Zone.ZoneControlObject.TryGetComponent<RacingControlComponent>(out var racingControlComponent)) return;
            Listen(racingControlComponent.OnPlayerLap, player =>
            {
                if (activated || player.Lap < lap) return;
                activated = true;
                movingPlatformComponent.MoveTo(1);
                Logger.Log("Platform moving");
            });
        });
    }
}
