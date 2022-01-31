using System.Collections.Generic;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.CruxPrime {

    /// <summary>
    /// Native implementation of scripts/02_server/map/am/l_random_spawner_fin.lua
    /// </summary>
    [ScriptName("ScriptComponent_1423_script_name__removed")]
    public class RandomSpawnerFin : RandomSpawnerBase
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public RandomSpawnerFin(GameObject gameObject) : base(gameObject)
        {
            this.Zones = new List<RandomSpawnerZone>()
            {
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 2, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 2, "type4"),
                    },
                    Chance = 10,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 2, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 2, "type3"),
                    },
                    Chance = 5,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeHorseman, 2, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 3, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 5, "type3"),
                    },
                    Chance = 10,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeHorseman, 1, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeGorilla, 1, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 4, "type3"),
                    },
                    Chance = 2,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 1, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 2, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeGorilla, 2, "type3"),
                    },
                    Chance = 1,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 2, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 4, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeHorseman, 1, "type3"),
                    },
                    Chance = 10,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 1, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeHorseman, 1, "type3"),
                    },
                    Chance = 5,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 2, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeGorilla, 1, "type3"),
                    },
                    Chance = 2,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 2, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 1, "type3"),
                    },
                    Chance = 10,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 1, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeHorseman, 1, "type3"),
                    },
                    Chance = 10,
                },
            };
            this.SectionMultipliers = new Dictionary<string, float>()
            {
                { "secA", 1 },
                { "secB", 1 },
                { "secC", 1.2f },
                { "secD", 1.3f },
                { "secE", 1.6f },
                { "secF", 1 },
                { "secG", 1 },
                { "secH", 1.2f },
            };
            this.ZoneName = "fin";

            this.Start();
        }
    }
}