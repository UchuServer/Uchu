using System.Linq;
using System.Numerics;
using Uchu.Core.Client;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("ScriptComponent_1483_script_name__removed")]
    public class PowerPanel : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public PowerPanel(GameObject gameObject) : base(gameObject)
        {
            // Find animation duration from cdclient.
            var cdClientContext = new CdClientContext();
            var animation = cdClientContext.AnimationsTable.FirstOrDefault(e => e.Animationtype == "nexus-powerpanel");
            if (animation?.Animationlength == null) return;

            Listen(gameObject.OnInteract, player =>
            {
                // If the player has this mission active, they should fix the panel. Don't display explosion/sparks animation.
                if (player.GetComponent<MissionInventoryComponent>().HasActive(MissionId.PowerintheTower))
                {
                    // Show animation of player fixing the panel.
                    player.Animate("nexus-powerpanel");

                    // When the animation is done, update the effects and set the flag to progress the mission.
                    Zone.Schedule(() =>
                    {
                        // Set flag so sparks will be hidden in the future.
                        player.GetComponent<CharacterComponent>()
                            .SetFlagAsync((int) gameObject.Settings["flag"], true);

                        // Stop sparks effect.
                        player.Message(new NotifyClientObjectMessage
                        {
                            Associate = gameObject,
                            Name = "SparkStop",
                        });

                        // Show rebuild celebrate animation.
                        player.Animate("rebuild-celebrate", true);
                    }, 1000 * (float) animation.Animationlength);
                }
                else
                {
                    // Orient player to object.
                    player.Message(new OrientToObjectMessage
                    {
                        Associate = player,
                        ObjId = gameObject,
                    });

                    // Apply the knockback effect, in the direction away from the object.
                    player.Message(new KnockbackMessage
                    {
                        Associate = player,
                        Caster = gameObject,
                        Originator = gameObject,
                        KnockbackTime = 200,
                        Vector = Vector3.Transform(new Vector3(15, 5, 0), gameObject.Transform.Rotation),
                    });
                    player.Animate("knockback-recovery", true, 2f);

                    // Start and set a timer to stop the paradox_panel_burst effect (explosion/sparks).
                    this.PlayFXEffect("paradox_panel_burst", "create", 6432);
                    this.AddTimerWithCancel(1, "FXTime");

                    // Wait 2 seconds before making the interaction available again.
                    Zone.Schedule(() =>
                    {
                        // Terminate the interaction.
                        player.Message(new TerminateInteractionMessage
                        {
                            Associate = player,
                            Terminator = gameObject,
                            Type = TerminateType.FromInteraction,
                        });
                    }, 2000);
                }
            });
        }

        /// <summary>
        /// Callback for the timer completing.
        /// </summary>
        /// <param name="timerName">Timer that was completed.</param>
        public override void OnTimerDone(string timerName)
        {
            this.StopFXEffect("paradox_panel_burst");
        }
    }
}
