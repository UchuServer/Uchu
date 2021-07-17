using System.Collections.Generic;
using Uchu.World;

namespace Uchu.StandardScripts.Base.SurvivalConfiguration
{
    public class SurvivalMobSets
    {
        /// <summary>
        /// Mob LOTs used for the game.
        /// </summary>
        public Dictionary<string, List<Lot>> MobLots { get; set; }

        /// <summary>
        /// Base mobs spawned for a given tier.
        /// </summary>
        public Dictionary<string, List<List<int>>> BaseMobSet { get; set; }

        /// <summary>
        /// Random mobs spawned for a given tier.
        /// </summary>
        public Dictionary<string, List<List<int>>> RandomMobSet { get; set; }
        
        /// <summary>
        /// Fetches a set by name.
        /// </summary>
        /// <param name="index">Index to search.</param>
        public Dictionary<string, List<List<int>>> this[string index]
            => (Dictionary<string, List<List<int>>>) typeof(SurvivalMobSets).GetProperty(index)?.GetValue(this);
    }
}