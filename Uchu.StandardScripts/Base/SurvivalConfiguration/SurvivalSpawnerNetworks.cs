using System.Collections.Generic;

namespace Uchu.StandardScripts.Base.SurvivalConfiguration
{
    public class SurvivalSpawnerNetworks
    {
        /// <summary>
        /// Base networks for the game.
        /// </summary>
        public SurvivalSpawnerNetworkSet BaseNetworks { get; set; }
        
        /// <summary>
        /// Random networks for the game.
        /// </summary>
        public SurvivalSpawnerNetworkSet RandomNetworks { get; set; }
        
        /// <summary>
        /// Reward networks for the game.
        /// </summary>
        public SurvivalSpawnerNetworkSet RewardNetworks { get; set; }
        
        /// <summary>
        /// Smashable networks for the game.
        /// </summary>
        public SurvivalSpawnerNetworkSet SmashNetworks { get; set; }
    }
}