using Uchu.World;
using Uchu.World.Scripting.Native;
using System.Linq;
using System.Threading.Tasks;

namespace Uchu.StandardScripts.General
{
    [ScriptName("l_ag_turret.lua")]
    [ScriptName("turret.lua")]
    public class Turret : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public Turret(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnStart, () =>
            {
                //engie turret + rebuild mechs have a smash time of 10 so they'll self destruct before the timer happens, ?????????
                float killTime = gameObject.TryGetComponent<LuaScriptComponent>(out var luaScriptComponent) && luaScriptComponent.ScriptName.ToLower().EndsWith("l_ag_turret.lua") ? 20 : 30;
                AddTimerWithCancel(30, "TickTime");
                var quickBuild = gameObject.GetComponent<QuickBuildComponent>();
                Listen(quickBuild.OnStateChange, state =>
                {
                    if (state == RebuildState.Completed)
                    {
                        CancelAllTimers();
                        AddTimerWithCancel(30, "TickTime");
                    }
                    if (state == RebuildState.Building)
                    {
                        Zone.BroadcastMessage(new LockNodeRotationMessage
                        {
                            Associate = gameObject,
                            NodeName = "base",
                        });
                    }
                });
            });
        }
        public override void OnTimerDone(string timerName)
        {
            if (timerName == "TickTime")
            {
                GameObject.GetComponent<DestructibleComponent>().SmashAsync(GameObject);
            }
        }
    }
}