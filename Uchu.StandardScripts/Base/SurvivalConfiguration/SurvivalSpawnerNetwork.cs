using System.Collections.Generic;

namespace Uchu.StandardScripts.Base.SurvivalConfiguration
{
    public class SurvivalSpawnerNetwork
    {
        /// <summary>
        /// Names of the spawner.
        /// </summary>
        public List<string> SpawnerName { get; set; }
        
        /// <summary>
        /// Number of the spawner.
        /// </summary>
        public string SpawnerNumber { get; set; }
        
        /// <summary>
        /// Whether the spawner is locked.
        /// </summary>
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// Whether the spawner is active.
        /// </summary>
        public bool IsActive { get; set; } = false;
    }
}