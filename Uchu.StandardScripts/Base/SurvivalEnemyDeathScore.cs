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
            
            // Set the factions of the enemy.
            // Sentries don't have the correct factions by default for some reason.
            if (!gameObject.TryGetComponent<DestroyableComponent>(out var destroyableComponent)) return;
            destroyableComponent.Factions = new[] {4};
            destroyableComponent.Enemies = new[] {1};
            
            // Listen to the enemy being smashed.
            if (!GameObject.TryGetComponent<DestructibleComponent>(out var destructibleComponent)) return;
            Listen(destructibleComponent.OnSmashed, (o, player) =>
            {
                if (!GameObject.Zone.ZoneControlObject.TryGetComponent<ScriptedActivityComponent>(out var activity)) return;
                activity.SetParameter(player, 0, activity.GetParameter(player, 0) + Points);
            });
        }
    }
}