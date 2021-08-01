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
            var mobSets = new SurvivalMobSets()
            {
                MobLots = new Dictionary<string, List<Lot>>()
                {
                    {"MobA", new List<Lot>() { 6351, 8088, 8089 }},
                    {"MobB", new List<Lot>() { 6668, 8090, 8091 }},
                    {"MobC", new List<Lot>() { 6454, 8096, 8097 }},
                },
                BaseMobSet = new Dictionary<string, List<List<int>>>()
                {
                    {"tier1", new List<List<int>>() { new List<int>() { 3, 0, 0 } } },
                    {"tier2", new List<List<int>>() { new List<int>() { 2, 1, 0 } } },
                    {"tier3", new List<List<int>>() { new List<int>() { 4, 1, 0 } } },
                    {"tier4", new List<List<int>>() { new List<int>() { 1, 2, 0 } } },
                    {"tier5", new List<List<int>>() { new List<int>() { 0, 1, 1 } } },
                    {"tier6", new List<List<int>>() { new List<int>() { 0, 2, 2 } } },
                },
                RandomMobSet = new Dictionary<string, List<List<int>>>()
                {
                    {"tier1", new List<List<int>>()
                        {
                            new List<int>() { 4, 0, 0 },
                            new List<int>() { 4, 0, 0 },
                            new List<int>() { 4, 0, 0 },
                            new List<int>() { 4, 0, 0 },
                            new List<int>() { 3, 1, 0 },
                        }
                    },
                    {"tier2", new List<List<int>>()
                        {
                            new List<int>() { 4, 1, 0 },
                            new List<int>() { 4, 1, 0 },
                            new List<int>() { 4, 1, 0 },
                            new List<int>() { 4, 1, 0 },
                            new List<int>() { 2, 1, 1 },
                        }
                    },
                    {"tier3", new List<List<int>>()
                        {
                            new List<int>() { 1, 2, 0 },
                            new List<int>() { 1, 2, 0 },
                            new List<int>() { 1, 2, 0 },
                            new List<int>() { 1, 2, 0 },
                            new List<int>() { 0, 1, 1 },
                        }
                    },
                    {"tier4", new List<List<int>>()
                        {
                            new List<int>() { 1, 2, 1 },
                            new List<int>() { 1, 2, 1 },
                            new List<int>() { 1, 2, 1 },
                            new List<int>() { 0, 2, 1 },
                            new List<int>() { 0, 2, 2 },
                        }
                    },
                    {"tier5", new List<List<int>>()
                        {
                            new List<int>() { 0, 1, 2 },
                            new List<int>() { 0, 1, 2 },
                            new List<int>() { 0, 1, 2 },
                            new List<int>() { 0, 1, 3 },
                            new List<int>() { 0, 1, 3 },
                        }
                    },
                    {"tier6", new List<List<int>>()
                        {
                            new List<int>() { 0, 2, 3 },
                            new List<int>() { 0, 2, 3 },
                            new List<int>() { 0, 2, 3 },
                            new List<int>() { 0, 2, 3 },
                            new List<int>() { 0, 2, 3 },
                        }
                    },
                },
            };
            var spawnerNetworks = new SurvivalSpawnerNetworks()
            {
                BaseNetworks = new SurvivalSpawnerNetworkSet("BaseMobSet")
                {
                    new SurvivalSpawnerNetwork()
                    {
                        SpawnerName = new List<string>() { "Base_MobA", "Base_MobB", "Base_MobC" },
                        SpawnerNumber = "",
                    },
                },
                RandomNetworks = new SurvivalSpawnerNetworkSet("RandomMobSet")
                {
                    new SurvivalSpawnerNetwork()
                    {
                        SpawnerName = new List<string>() { "MobA_", "MobB_", "MobC_" },
                        SpawnerNumber = "01",
                    },
                    new SurvivalSpawnerNetwork()
                    {
                        SpawnerName = new List<string>() { "MobA_", "MobB_", "MobC_" },
                        SpawnerNumber = "02",
                    },
                    new SurvivalSpawnerNetwork()
                    {
                        SpawnerName = new List<string>() { "MobA_", "MobB_", "MobC_" },
                        SpawnerNumber = "03",
                        IsLocked = true,
                    },
                },
                RewardNetworks = new SurvivalSpawnerNetworkSet()
                {
                    new SurvivalSpawnerNetwork()
                    {
                        SpawnerName = new List<string>() { "Rewards_" },
                        SpawnerNumber = "01",
                    },
                },
                SmashNetworks = new SurvivalSpawnerNetworkSet()
                {
                    new SurvivalSpawnerNetwork()
                    {
                        SpawnerName = new List<string>() { "Smash_" },
                        SpawnerNumber = "01",
                    },
                },
            };
            var missionsToUpdate = new Dictionary<int, int>()
            {
                { 479, 60 },
                { 1153, 180 },
                { 1618, 420 },
                { 1648, 420 },
                { 1628, 420 },
                { 1638, 420 },
                { 1412, 120 },
                { 1510, 120 },
                { 1547, 120 },
                { 1584, 120 },
                { 1426, 300 },
                { 1524, 300 },
                { 1561, 300 },
                { 1598, 300 },
                { 1865, 180 },
            };
            
            this.SetGameVariables(avantGardensSurvivalConfiguration, mobSets, spawnerNetworks,missionsToUpdate);
        }
    }
}