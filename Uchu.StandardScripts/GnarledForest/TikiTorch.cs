using System.Numerics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.GnarledForest
{
    [ScriptName("ScriptComponent_946_script_name__removed")]
    public class TikiTorch : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public TikiTorch(GameObject gameObject) : base(gameObject)
        {
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