using Uchu.World;
using Uchu.World.Scripting.Native;
using System.Linq;
using System;
using System.Numerics;
using Uchu.Physics;

namespace Uchu.StandardScripts.General
{
    [ScriptName("l_special_firepit.lua")]
    public class Firepit : ObjectScript
    {
        private bool IsBurning = true;
        private int counter = 0;
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public Firepit(GameObject gameObject) : base(gameObject)
        {
            var physics = gameObject.AddComponent<PhysicsComponent>();
            var physicsObject = SphereBody.Create(
                gameObject.Zone.Simulation,
                gameObject.Transform.Position,
                2
            );
            physics.SetPhysics(physicsObject);
            var ai = gameObject.AddComponent<BaseCombatAiComponent>();
            ai.Enabled = false;
            Listen(physics.OnEnter, (collider) =>
            {
                //this seems really easy to break, but it's interpreted directly from the script so uh
                if (IsBurning && collider.GameObject is Player)
                {
                    counter++;
                    if (counter == 1 && GameObject.TryGetComponent<SkillComponent>(out var skillComponent))
                    {
                        skillComponent.CalculateSkillAsync(43);
                        AddTimerWithCancel(2, "TimeBetweenCast");
                    }
                }
            });
            Listen(physics.OnLeave, (collider) =>
            {
                if (IsBurning && collider.GameObject is Player && counter > 0)
                {
                    counter--;
                    if (counter == 0) CancelAllTimers();
                }
            });
            PlayFXEffect("Burn", "running", 295);
            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnSkillEvent, async (target, effectHandler) =>
                {
                    if (effectHandler == "waterspray" && IsBurning)
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
                                PlayFXEffect("Burn", "running", 295);
                            }
                        }, 37000);
                    }
                });
                Listen(player.OnFireServerEvent, (args, message) =>
                {
                    if (args == "physicsReady")
                    {
                        PlayFXEffect("Burn", "running", 295);
                    }
                });
            });
        }
        public override void OnTimerDone(string timerName)
        {
            if (timerName == "TimeBetweenCast" && GameObject.TryGetComponent<SkillComponent>(out var skillComponent))
            {
                AddTimerWithCancel(2, "TimeBetweenCast");
                skillComponent.CalculateSkillAsync(43);
                foreach (var player in Zone.Players)
                {
                    //skill radius
                    if (Vector3.Distance(player.Transform.Position, GameObject.Transform.Position) <= 4 && player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent))
                    {
                        missionInventoryComponent.ScriptAsync(658, GameObject.Lot);
                    }
                }
            }
        }
    }
}