using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Native implementation of scripts/ai/wild/l_wild_ambient_crab.lua
    /// </summary>
    [ScriptName("L_WILD_AMBIENT_CRAB.lua")]
    public class WildAmbientCrab : ObjectScript
    {
        public WildAmbientCrab(GameObject gameObject) : base(gameObject)
        {
            this.SetVar("flipped", true);
            gameObject.Animate("idle");
            Listen(gameObject.OnInteract, player =>
            {
                if (this.GetVar<bool>("flipped"))
                {
                    this.AddTimerWithCancel(0.6f, "Flipping");
                    gameObject.Animate("flip-over");
                    this.SetVar("flipped", false);
                }
                else
                {
                    this.AddTimerWithCancel(0.8f, "Flipback");
                    gameObject.Animate("flip-back");
                    this.SetVar("flipped", true);
                }
                player.Message(new TerminateInteractionMessage
                {
                    Associate = player,
                    Terminator = gameObject,
                    Type = TerminateType.FromInteraction,
                });
            });
        }

        public override void OnTimerDone(string timerName)
        {
            switch (timerName)
            {
                case "Flipping":
                    this.GameObject.Animate("over-idle");
                    break;
                case "Flipback":
                    this.GameObject.Animate("idle");
                    break;
            }
        }
    }
}
