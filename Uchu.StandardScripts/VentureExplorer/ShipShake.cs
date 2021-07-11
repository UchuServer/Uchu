using System;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.VentureExplorer
{
    /// <summary>
    /// Native implementation of scripts/ai/ag/l_ag_ship_shake.lua
    /// </summary>
    [ScriptName("l_ag_ship_shake.lua")]
    public class ShipShake : ObjectScript
    {
        /// <summary>
        /// Base time to repeat the effect.
        /// </summary>
        public const int RepeatTime = 2;

        /// <summary>
        /// Max random time added to play the effect.
        /// </summary>
        public const int RandomTime = 10;

        /// <summary>
        /// Radius of the shake.
        /// </summary>
        public const float ShakeRadius = 500;

        /// <summary>
        /// Name of the shake effect.
        /// </summary>
        public const string FxName = "camshake-bridge";

        /// <summary>
        /// Randomizer for shaking the ship.
        /// </summary>
        private Random _random = new Random();

        /// <summary>
        /// Object for playing the debris.
        /// </summary>
        private GameObject _debrisObject;

        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ShipShake(GameObject gameObject) : base(gameObject)
        {
            // Get the effect objects.
            _debrisObject = this.GetGroup("DebrisFX")[0];

            // Start the initial timer.
            this.AddTimerWithCancel(RepeatTime, "ShakeTimer");
        }

        /// <summary>
        /// Returns the next time to run the shake timer.
        /// </summary>
        /// <returns>The next time to run the shake timer</returns>
        private int NewTime()
        {
            var time = this._random.Next(0, RandomTime + 1);
            if (time < RandomTime / 2)
            {
                time += RandomTime / 2;
            }

            return RandomTime + time;
        }

        /// <summary>
        /// Callback for the timer completing.
        /// </summary>
        /// <param name="timerName">Timer that was completed.</param>
        public override void OnTimerDone(string timerName)
        {
            // Get the ship effect objects.
            var shipFx1 = this.GetGroup("ShipFX")[0];
            var shipFx2 = this.GetGroup("ShipFX2")[0];
            
            if (timerName == "ShakeTimer")
            {
                // Queue the next timer.
                this.AddTimerWithCancel(this.NewTime(), "ShakeTimer");

                // Play the shake effect and falling debris.
                Zone.BroadcastMessage(new PlayEmbeddedEffectOnAllClientsNearObjectMessage
                {
                    Associate = this.GameObject,
                    Radius = ShakeRadius,
                    EffectName = FxName,
                    FromObject = this.GameObject
                });
                this._debrisObject.PlayFX("Debris", "DebrisFall");


                // Run the explosion.
                var randomEffect = this._random.Next(0, 4);
                shipFx1.PlayFX("FX", $"shipboom{randomEffect}", 559);
                shipFx2.Animate("explosion");
                this.AddTimerWithCancel(5, "ExplodeIdle");
            }
            else if (timerName == "ExplodeIdle")
            {
                // Idle the explosion effects.
                shipFx1.Animate("idle");
                shipFx2.Animate("idle");
            }
        }
    }
}