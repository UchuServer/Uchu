using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Base
{
    /// <summary>
    /// Native implementation of scripts/zone/enemy/waves/l_waves_enemy_death_score.lua
    /// </summary>
    [ScriptName("l_waves_enemy_death_score.lua")]
    public class SurvivalEnemyDeathScore : ObjectScript
    {
        /// <summary>
        /// Points to award on the death of the enemy.
        /// </summary>
        public const int Points = 100;
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public SurvivalEnemyDeathScore(GameObject gameObject) : base(gameObject)
        {
            // Set the points of the enemy.
            this.SetNetworkVar("points", Points);
            
            // Listen to the enemy being smashed.
            if (!GameObject.TryGetComponent<DestructibleComponent>(out var destructibleComponent));
            Listen(destructibleComponent.OnSmashed, (o, player) =>
            {
                if (!GameObject.Zone.ZoneControlObject.TryGetComponent<ScriptedActivityComponent>(out var activity)) return;
                activity.SetParameter(player, 0, activity.GetParameter(player, 0) + Points);
            });
        }
    }
}