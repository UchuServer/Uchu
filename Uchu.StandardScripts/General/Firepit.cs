using Uchu.World;
using Uchu.World.Scripting.Native;
using System.Linq;
using System;
using System.Numerics;
using Uchu.Physics;
using System.Threading.Tasks;

namespace Uchu.StandardScripts.General
{
    [ScriptName("l_special_firepit.lua")]
    public class Firepit : ObjectScript
    {
        private bool IsBurning = true;
        private Quaternion lockRotation { get; set; }
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public Firepit(GameObject gameObject) : base(gameObject)
        {
            SetProximityRadius(2, "Firepit");
            StartFXEffect("Burn", "running", 295);
            
            //fix aoe skill not running
            var ai = gameObject.AddComponent<BaseCombatAiComponent>();
            ai.Enabled = false;
            ai.StartCombatAI();

            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnSkillEvent, async (target, effectHandler) =>
                {
                    if (effectHandler == "waterspray" && IsBurning && target == gameObject)
                    {
                        IsBurning = false;
                        StopFXEffect("Burn");
                        PlayFXEffect("Off", "idle", 295);
                        Zone.Schedule(() =>
                        {
                            if (!IsBurning)
                            {
                                IsBurning = true;
                                StopFXEffect("Off");
                                StartFXEffect("Burn", "running", 295);
                            }
                        }, 37000);
                    }
                });
            });
        }
        public override void OnTimerDone(string timerName)
        {
            if (timerName == "TimeBetweenCast" && GameObject.TryGetComponent<SkillComponent>(out var skillComponent))
            {
                AddTimerWithCancel(2, "TimeBetweenCast");
                foreach (var player in Zone.Players)
                {
                    //skill radius
                    if (Vector3.Distance(player.Transform.Position, GameObject.Transform.Position) <= 5 && player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent))
                    {
                        Task.Run(async () => 
                        {
                            await missionInventoryComponent.ScriptAsync(1103, 8239);
                        });
                    }
                }
                skillComponent.CalculateSkillAsync(43, GameObject);
            }
        }
        public override void OnProximityUpdate(string name, PhysicsCollisionStatus status, Player player)
        {
            if (name == "Firepit")
            {
                if (status == PhysicsCollisionStatus.Enter)
                {
                    //this seems really easy to break, but it's interpreted directly from the script so uh
                    if (IsBurning && GameObject.TryGetComponent<SkillComponent>(out var skillComponent))
                    {
                        skillComponent.CalculateSkillAsync(43, GameObject);
                        AddTimerWithCancel(2, "TimeBetweenCast");
                    }
                }
                else if (status == PhysicsCollisionStatus.Leave)
                {
                    CancelAllTimers();
                }
            }
        }
    }
}
