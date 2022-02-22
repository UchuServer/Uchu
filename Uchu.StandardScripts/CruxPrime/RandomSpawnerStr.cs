using System.Collections.Generic;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.CruxPrime
{
    /// <summary>
    /// Native implementation of scripts/02_server/map/am/l_random_spawner_str.lua
    /// </summary>
    [ScriptName("ScriptComponent_1420_script_name__removed")]
    public class RandomSpawnerStr : RandomSpawnerBase
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public RandomSpawnerStr(GameObject gameObject) : base(gameObject)
        {
            this.Zones = new List<RandomSpawnerZone>()
            {
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 4, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 3, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 3, "type3"),
                    },
                    Chance = 45,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 3, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 3, "type3"),
                    },
                    Chance = 20,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 4, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 2, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 1, "type3"),
                    },
                    Chance = 10,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 1, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 4, "type3"),
                    },
                    Chance = 3,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeHorseman, 1, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 5, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 2, "type3"),
                    },
                    Chance = 1,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeGorilla, 1, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 5, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 2, "type3"),
                    },
                    Chance = 1,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 2, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 4, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 2, "type3"),
                    },
                    Chance = 3,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeGorilla, 1, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeHorseman, 1, "type3"),
                    },
                    Chance = 1,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 3, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 3, "type3"),
                    },
                    Chance = 5,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 4, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 4, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 4, "type3"),
                    },
                    Chance = 1,
                },
            };
            this.SectionMultipliers = new Dictionary<string, float>()
            {
                { "secA", 1 },
                { "secB", 1 },
                { "secC", 1.2f },
            };
            this.ZoneName = "str";
            this.MobDeathResetNumber = 20;
            
            this.Start();
        }
    }
}