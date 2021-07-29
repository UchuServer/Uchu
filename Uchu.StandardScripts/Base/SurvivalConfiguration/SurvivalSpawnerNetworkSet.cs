using System.Collections.Generic;

namespace Uchu.StandardScripts.Base.SurvivalConfiguration
{
    public class SurvivalSpawnerNetworkSet : List<SurvivalSpawnerNetwork>
    {
        /// <summary>
        /// Name of the set.
        /// </summary>
        public string Set { get; set; }

        /// <summary>
        /// Creates the spawner network set data.
        /// </summary>
        public SurvivalSpawnerNetworkSet()
        {
            
        }
        
        /// <summary>
        /// Creates the spawner network set data.
        /// </summary>
        /// <param name="set">Set name to use.</param>
        public SurvivalSpawnerNetworkSet(string set)
        {
            this.Set = set;
        }
    }
}