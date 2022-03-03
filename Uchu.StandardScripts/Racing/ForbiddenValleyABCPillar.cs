using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

[ScriptName("fv_race_pillar_abc_server.lua")]
public class ForbiddenValleyABCPillar : ObjectScript
{
    private List<GameObject> _pillars;
    private GameObject _dragon;
    private bool _triggered;
    public ForbiddenValleyABCPillar(GameObject gameObject) : base(gameObject)
    {
        Task.Run(async () =>
        {
            await Task.Delay(7000);
            _pillars = Zone.GameObjects.Where(t => t.GetGroups().Contains("pillars")).ToList();
            _dragon = Zone.GameObjects.First(t => t.GetGroups().Contains("dragon"));
            if (!gameObject.TryGetComponent<PhysicsComponent>(out var physicsComponent) || 
                !Zone.ZoneControlObject.TryGetComponent<RacingControlComponent>(out var racingControlComponent)) return;
            Listen(physicsComponent.OnEnter, other =>
            {
                if (_triggered || racingControlComponent.Players.Find(i => i.Player == other.GameObject as Player).Lap != 1) return;
                _triggered = true;
                _pillars.First(i => i.Lot == 11946).Animate("crumble");
                _dragon.Animate("roar");
                AddTimerWithCancel(2.5f, "PillarBFall");
                AddTimerWithCancel(3.7f, "PillarCFall");
            });
        });
    }
    public override void OnTimerDone(string timerName)
    {
        if (!timerName.StartsWith("Pillar")) return;
        var lot = timerName[6] == 'B' ? 11947 : 11948;
        _pillars.First(i => i.Lot == lot).Animate("crumble");
    }
}