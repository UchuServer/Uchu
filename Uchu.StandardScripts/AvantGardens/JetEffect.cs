using System;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Native implementation of scripts/ai/ag/l_ag_jet_effect_server.lua
    /// </summary>
    [ScriptName("l_ag_jet_effect_server.lua")]
    public class JetEffect : ObjectScript
    {
        /// <summary>
        /// Randomizer for the mortars.
        /// </summary>
        private Random _random = new Random();

        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public JetEffect(GameObject gameObject) : base(gameObject)
        {
            if (!gameObject.TryGetComponent<QuickBuildComponent>(out var quickBuildComponent)) return;
            this.SetVar("isInUse", false);
            
            // Listen to the dish being interacted with.
            Listen(this.GameObject.OnInteract, (player) =>
            {
                if (quickBuildComponent.State != RebuildState.Completed) return;
                if (this.GetVar<bool>("isInUse")) return;

                // Set the jet as in use.
                this.Zone.BroadcastMessage(new NotifyClientObjectMessage()
                {
                    Associate = this.GameObject,
                    Name = "toggleInUse",
                    Param1 = 1,
                });
                this.SetVar("isInUse", true);

                // Play the effect.
                var jetEffect = this.GetGroup("Jet_FX")[0];
                jetEffect.Animate("jetFX");
                this.PlayFXEffect("radarDish", "create", 641);

                // Enable the timers.
                this.AddTimerWithCancel(2, "radarDish");
                this.AddTimerWithCancel(2.5f, "PlayEffect");
                this.AddTimerWithCancel(10, "CineDone");
            });

            // Listen to the quickbuild state completing.
            Listen(quickBuildComponent.OnStateChange, (state) =>
            {
                if (state != RebuildState.Completed) return;

                // Play the effect.
                var jetEffect = this.GetGroup("Jet_FX")[0];
                jetEffect.Animate("jetFX");

                // Enable the timers.
                this.AddTimerWithCancel(2.5f, "PlayEffect");
                this.AddTimerWithCancel(10, "CineDone");
            });
        }

        /// <summary>
        /// Callback for the timer completing.
        /// </summary>
        /// <param name="timerName">Timer that was completed.</param>
        public override void OnTimerDone(string timerName)
        {
            if (timerName == "radarDish")
            {
                // Stop the effect.
                this.StopFXEffect("radarDish");
            }
            else if (timerName == "PlayEffect")
            {
                // Run the air strike.
                var mortarObjects = this.GetGroup("mortarMain");
                var randomMortar = this._random.Next(1, mortarObjects.Length);
                mortarObjects[randomMortar].AddComponent<SkillComponent>().CalculateSkillAsync(318, this.GameObject)
                    .Wait();
            }
            else if (timerName == "CineDone")
            {
                // Set the jet as not in use.
                this.Zone.BroadcastMessage(new NotifyClientObjectMessage()
                {
                    Associate = this.GameObject,
                    Name = "toggleInUse",
                    Param1 = -1,
                });
                this.SetVar("isInUse", false);
            }
        }
    }
}