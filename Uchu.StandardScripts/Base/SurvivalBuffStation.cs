using System.Numerics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Base
{
    [ScriptName("ScriptComponent_867_script_name__removed")]
    public class SurvivalBuffStation : ObjectScript
    {
        /// <summary>
        /// Time between dropping life collectibles.
        /// </summary>
        public const int DropLifeTime = 3;
        
        /// <summary>
        /// Time between dropping imagination collectibles.
        /// </summary>
        public const int DropImaginationTime = 4;
        
        /// <summary>
        /// Time between dropping armor collectibles.
        /// </summary>
        public const int DropArmorTime = 6;

        /// <summary>
        /// Time until the station is automatically smashed.
        /// </summary>
        public const int SmashTime = 25;

        /// <summary>
        /// Player that built the station.
        /// </summary>
        private Player _player;
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public SurvivalBuffStation(GameObject gameObject) : base(gameObject)
        {
            // Listen to the station being built.
            var quickBuildComponent = gameObject.AddComponent<QuickBuildComponent>();
            quickBuildComponent.ResetTime = SmashTime;
            Listen(quickBuildComponent.OnStateChange, (state) =>
            {
                if (state != RebuildState.Completed) return;
                this._player = (Player) quickBuildComponent.Participants[0];
                this.AddTimerWithCancel(DropLifeTime, "DropLife");
                this.AddTimerWithCancel(DropImaginationTime, "DropImagination");
                this.AddTimerWithCancel(DropArmorTime, "DropArmor");
            });
        }
        
        /// <summary>
        /// Callback for the timer completing.
        /// </summary>
        /// <param name="timerName">Timer that was completed.</param>
        public override void OnTimerDone(string timerName)
        {
            // Return if the quick build isn't complete.
            var quickBuildComponent = this.GameObject.AddComponent<QuickBuildComponent>();
            if (quickBuildComponent.State != RebuildState.Completed) return;
            
            // Drop the item.
            if (timerName == "DropLife")
            {
                // Drop a life and prepare the next timer.
                var loot = InstancingUtilities.InstantiateLoot(Lot.Health, this._player, this.GameObject, this.GameObject.Transform.Position + Vector3.UnitY * 3);
                Start(loot);
                this.AddTimerWithCancel(DropLifeTime, "DropLife");
            }
            else if (timerName == "DropImagination")
            {
                // Drop an imagination and prepare the next timer.
                var loot = InstancingUtilities.InstantiateLoot(Lot.Imagination, this._player, this.GameObject, this.GameObject.Transform.Position + Vector3.UnitY * 3);
                Start(loot);
                this.AddTimerWithCancel(DropImaginationTime, "DropImagination");
            }
            else if (timerName == "DropArmor")
            {
                // Drop an armor and prepare the next timer.
                var loot = InstancingUtilities.InstantiateLoot(Lot.Armor, this._player, this.GameObject, this.GameObject.Transform.Position + Vector3.UnitY * 3);
                Start(loot);
                this.AddTimerWithCancel(DropArmorTime, "DropArmor");
            }
        }
    }
}