using System;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.VentureExplorer
{
    /// <summary>
    /// Native implementation of scripts/ai/ag/l_ag_space_stuff.lua
    /// </summary>
    [ScriptName("l_ag_space_stuff.lua")]
    public class SpaceStuff : ObjectScript
    {
        /// <summary>
        /// Randomizer for the effects.
        /// </summary>
        private Random _random = new Random();
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public SpaceStuff(GameObject gameObject) : base(gameObject)
        {
            // Start the initial timer.
            this.AddTimerWithCancel(5, "FloaterScale");
        }
        
        /// <summary>
        /// Callback for the timer completing.
        /// </summary>
        /// <param name="timerName">Timer that was completed.</param>
        public override void OnTimerDone(string timerName)
        {
            if (timerName == "FloaterScale")
            {
                // Play the animation.
                var scaleType = _random.Next(1, 6);
                this.PlayAnimation($"scale_0{scaleType}");
                
                // Start the next timer.
                this.AddTimerWithCancel(0.4f, "FloaterScale");
            } else if (timerName == "FloaterPath")
            {
                // Play the animation.
                var scaleType = _random.Next(1, 5);
                this.PlayAnimation($"scale_0{scaleType}");
                
                // Start the next timer.
                this.AddTimerWithCancel(_random.Next(20, 26), "FloaterScale");
            }
        }
    }
}