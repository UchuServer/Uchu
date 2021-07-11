using System.Numerics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.VentureExplorer
{
    /// <summary>
    /// Native implementation of scripts/ai/ag/l_ag_ship_player_shock_server.lua
    /// </summary>
    [ScriptName("l_ag_ship_player_shock_server.lua")]
    public class BrokenConsole : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public BrokenConsole(GameObject gameObject) : base(gameObject)
        {
            // Connect the player interacting.
            Listen(gameObject.OnInteract, player =>
            {
                // Terminate the interaction.
                player.Message(new ServerTerminateInteractionMessage
                {
                    Associate = player,
                    Terminator = gameObject,
                    Type = TerminateType.FromInteraction
                });

                // Perform the knockback effect.
                player.Animate("knockback-recovery", true);
                player.Message(new KnockbackMessage
                {
                    Associate = player,
                    Caster = gameObject,
                    Originator = gameObject,
                    Vector = new Vector3(-20, 10, -20),
                });
                
                // Start and stop the console sparks.
                this.PlayFXEffect("console_sparks", "create", 1430);
                this.AddTimerWithCancel(2, "FXTime");
            });
        }
        
        /// <summary>
        /// Callback for the timer completing.
        /// </summary>
        /// <param name="timerName">Timer that was completed.</param>
        public override void OnTimerDone(string timerName)
        {
            this.StopFXEffect("console_sparks");
        }
    }
}