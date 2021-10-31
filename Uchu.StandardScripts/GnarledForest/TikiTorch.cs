using System.Numerics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.GnarledForest
{
    [ScriptName("ScriptComponent_946_script_name__removed")]
    public class TikiTorch : ObjectScript
    {
        private bool IsBurning = true;
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public TikiTorch(GameObject gameObject) : base(gameObject)
        {
            StartFXEffect("tikitorch", "fire", 611);
            // Listen to the tiki torch being interacted with.
            Listen(gameObject.OnInteract, player =>
            {
                // Play the animation.
                this.PlayAnimation("interact");
                this.SetNetworkVar("bIsInUse", true);

                // Drop the imagination.
                for (var i = 0; i < 2; i++)
                {
                    var loot = InstancingUtilities.InstantiateLoot(Lot.Imagination, player, gameObject,
                        gameObject.Transform.Position + Vector3.UnitY);
                    Start(loot);
                }

                // Reset the torch.
                this.AddTimerWithCancel(0.5f, "ResetTikiTorch");
            });
            Listen(Zone.OnPlayerLoad, player => 
            {
                Listen(player.OnSkillEvent, async (target, effectHandler) =>
                {
                    if (effectHandler == "waterspray" && IsBurning && target == gameObject)
                    {
                        IsBurning = false;
                        this.PlayAnimation("water");
                        StopFXEffect("tikitorch");
                        PlayFXEffect("water", "water", 611);
                        PlayFXEffect("steam", "steam", 611);
                        Zone.Schedule(() =>
                        {
                            if (!IsBurning)
                            {
                                IsBurning = true;
                                StopFXEffect("water");
                                StopFXEffect("steam");
                                StartFXEffect("tikitorch", "fire", 611);
                            }
                        }, 7000);
                        if (player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent))
                        {
                            missionInventoryComponent.ScriptAsync(702, gameObject.Lot);
                        }
                    }
                });
            });
        }
        
        /// <summary>
        /// Callback for the timer completing.
        /// </summary>
        /// <param name="timerName">Timer that was completed.</param>
        public override void OnTimerDone(string timerName)
        {
            this.SetNetworkVar("bIsInUse", false);
        }
    }
}
