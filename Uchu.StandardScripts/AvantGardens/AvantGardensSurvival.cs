using System.Collections.Generic;
using System.Numerics;
using Uchu.StandardScripts.Base;
using Uchu.StandardScripts.Base.SurvivalConfiguration;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Native implementation of scripts/zone/ag/l_zone_ag_survival.lua
    /// </summary>
    [ScriptName("l_zone_ag_survival.lua")]
    public class AvantGardensSurvival : BaseSurvivalGame
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public AvantGardensSurvival(GameObject gameObject) : base(gameObject)
        {
            var avantGardensSurvivalConfiguration = new SurvivalConfiguration()
            {
                AcceptedDelay = 60,
                StartDelay = 2,
                WaveTime = 7,
                RewardInterval = 5,
                CoolDownTime = 10,
                StartMobSet2 = 5,
                StartMobSet3 = 15,
                UnlockNetwork3 = 10,
                UseMobLots = true,
                LotPhase = 1,
                BaseMobsStartTierAt = new List<int>() { 8, 13, 18, 23, 28, 32, },
                RandomMobsStartTierAt = new List<int>() { 2, 10, 15, 20, 25, 30, },
                ReturnZone = 1100,
                ReturnLocation = new Vector3(125, 376, -175)
            };
            
            // TODO: Create other configurations.
            this.SetGameVariables(avantGardensSurvivalConfiguration, null, null,null);
        }
    }
}