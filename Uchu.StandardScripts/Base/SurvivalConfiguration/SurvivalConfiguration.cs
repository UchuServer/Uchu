using System.Collections.Generic;
using System.Numerics;

namespace Uchu.StandardScripts.Base.SurvivalConfiguration
{
    public class SurvivalConfiguration
    {
        /// <summary>
        /// Delay after 1 person accepts the round.
        /// </summary>
        public int AcceptedDelay { get; set; }
        
        /// <summary>
        /// Delay after all players have accepted.
        /// </summary>
        public int StartDelay { get; set; }
        
        /// <summary>
        /// Time between spawning waves.
        /// </summary>
        public int WaveTime { get; set; }
        
        /// <summary>
        /// How many waves to wait to drop a reward and give the player a CoolDownTime.
        /// </summary>
        public int RewardInterval { get; set; }
        
        /// <summary>
        /// How long to wait between waves of RewardInterval.
        /// </summary>
        public int CoolDownTime { get; set; }
        
        /// <summary>
        /// Wave number to start spawning set 2.
        /// </summary>
        public int StartMobSet2 { get; set; }
        
        /// <summary>
        /// Wave number to start spawning set 3.
        /// </summary>
        public int StartMobSet3 { get; set; }
        
        /// <summary>
        /// Wave number to start spawning network 3.
        /// </summary>
        public int UnlockNetwork3 { get; set; }
        
        /// <summary>
        /// Whether to use mob LOTs.
        /// </summary>
        public bool UseMobLots { get; set; }
        
        /// <summary>
        /// Starting LOT phase of the game.
        /// </summary>
        public int LotPhase { get; set; }

        /// <summary>
        /// Wave number to start normal spawning tier mobs.
        /// </summary>
        public List<int> BaseMobsStartTierAt { get; set; }
        
        /// <summary>
        /// Wave number to start random spawning tier mobs.
        /// </summary>
        public List<int> RandomMobsStartTierAt { get; set; }

        /// <summary>
        /// Zone to return to.
        /// </summary>
        public int ReturnZone { get; set; }
        
        /// <summary>
        /// Position to return to in the zone.
        /// </summary>
        public Vector3 ReturnLocation { get; set; }
    }
}