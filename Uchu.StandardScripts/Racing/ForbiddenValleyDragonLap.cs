using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

public class ForbiddenValleyDragonLap : ObjectScript
{
    protected uint _lap;
    private GameObject _dragon;
    protected bool lap3Hack;
    private bool activated = false;
    public ForbiddenValleyDragonLap(GameObject gameObject) : base(gameObject)
    {
        Task.Run(async () =>
        {
            //????????
            await Task.Delay(7000);
            _dragon = Zone.GameObjects.First(t => t.GetGroups().Contains("dragon"));
            if (lap3Hack) return;
            Listen(gameObject.GetComponent<PhysicsComponent>().OnEnter, other =>
            {
                if (other.GameObject is not Player player || 
                    !Zone.ZoneControlObject.TryGetComponent<RacingControlComponent>(out var racingControlComponent)) return;
                var lap = racingControlComponent.Players.First(i => i.Player == player).Lap;
                if (lap != _lap - 1 || activated) return;
                activated = true;
                _dragon.Animate($"lap_0{_lap}");
            });
        });
    }
}

[ScriptName("fv_race_dragon_lap1_server.lua")]
public class ForbiddenValleyDragonLap1 : ForbiddenValleyDragonLap
{
    public ForbiddenValleyDragonLap1(GameObject gameObject) : base(gameObject)
    {
        _lap = 1;
    }
}

[ScriptName("fv_race_dragon_lap2_server.lua")]
public class ForbiddenValleyDragonLap2 : ForbiddenValleyDragonLap
{
    public ForbiddenValleyDragonLap2(GameObject gameObject) : base(gameObject)
    {
        _lap = 2;
    }
}

//bugfix: lap3 is accidentally set to be a client script, this should load it anyway
[LotSpecific(9613)]
//[ScriptName("fv_race_dragon_lap3_server.lua")]
public class ForbiddenValleyDragonLap3 : ForbiddenValleyDragonLap
{
    public ForbiddenValleyDragonLap3(GameObject gameObject) : base(gameObject)
    {
        _lap = 3;
        //Id specific?
        lap3Hack = gameObject.Id != 2483696;
    }
}