using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

[ScriptName("fv_race_pillar_d_server.lua")]
public class ForbiddenValleyDPillar : ObjectScript
{
    private GameObject _pillar;
    private GameObject _dragon;
    private bool _triggered;
    public ForbiddenValleyDPillar(GameObject gameObject) : base(gameObject)
    {
        Task.Run(async () =>
        {
            await Task.Delay(7000);
            _pillar = Zone.GameObjects.First(t => t.GetGroups().Contains("pillars") && t.Lot == 11949);
            _dragon = Zone.GameObjects.First(t => t.GetGroups().Contains("dragon"));
            if (!gameObject.TryGetComponent<PhysicsComponent>(out var physicsComponent) || 
                !Zone.ZoneControlObject.TryGetComponent<RacingControlComponent>(out var racingControlComponent)) return;
            Listen(physicsComponent.OnEnter, other =>
            {
                if (_triggered || racingControlComponent.Players.Find(i => i.Player == other.GameObject as Player).Lap != 2) return;
                _triggered = true;
                _pillar.Animate("crumble");
                _dragon.Animate("roar");
                Logger.Log("Pillar going down");
            });
        });
    }
}