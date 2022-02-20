using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Racing;

//reimplemented from DLU
[ScriptName("race_maelstrom_geiser.lua")]
public class MaelstromGeiser : ObjectScript
{
    private bool _firing;
    public MaelstromGeiser(GameObject gameObject) : base(gameObject)
    {
        SetProximityRadius(15, "deathZone");
        if (!gameObject.Settings.TryGetValue("startTime", out var time)) return;
        AddTimerWithCancel((int)time, "downTime");
    }
    public override void OnProximityUpdate(string name, PhysicsCollisionStatus status, Player player)
    {
        if (player == default || name != "deathZone" || status != PhysicsCollisionStatus.Enter || !_firing) return;
        var car = player.GetComponent<CharacterComponent>().VehicleObject;
        if (!Zone.ZoneControlObject.TryGetComponent<RacingControlComponent>(out var racingControlComponent)) return;
        Zone.BroadcastMessage(new DieMessage
        {
            Associate = car,
            Killer = GameObject,
            KillType = KillType.Violent,
            ClientDeath = true,
            SpawnLoot = false,
        });
        racingControlComponent.OnPlayerRequestDie(player);
    }
    public override void OnTimerDone(string timerName)
    {
        switch (timerName)
        {
            case "downTime":
                PlayFXEffect("geiser", "rebuild_medium", 4048);
                AddTimerWithCancel(1, "buildUpTime");
                break;
            case "buildUpTime":
                _firing = true;
                AddTimerWithCancel(1.5f, "killTime");
                break;
            case "killTime":
                StopFXEffect("geiser");
                _firing = false;
                AddTimerWithCancel(3, "downTime");
                break;
        }
    }
}