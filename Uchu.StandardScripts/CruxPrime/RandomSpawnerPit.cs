using System.Collections.Generic;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.CruxPrime
{
    /// <summary>
    /// Native implementation of scripts/02_server/map/am/l_random_spawner_pit.lua
    /// </summary>
    [ScriptName("ScriptComponent_1422_script_name__removed")]
    public class RandomSpawnerPit : RandomSpawnerBase
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public RandomSpawnerPit(GameObject gameObject) : base(gameObject)
        {
            this.Zones = new List<RandomSpawnerZone>()
            {
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 4, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 3, "type2"),
                    },
                    Chance = 5,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 4, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 7, "type2"),
                    },
                    Chance = 15,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 2, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 1, "type2"),
                    },
                    Chance = 15,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 1, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeHorseman, 4, "type2"),
                    },
                    Chance = 6,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeGorilla, 1, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 4, "type2"),
                    },
                    Chance = 2,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 7, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 6, "type2"),
                    },
                    Chance = 5,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 9, "type2"),
                    },
                    Chance = 10,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeGorilla, 1, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 8, "type2"),
                    },
                    Chance = 2,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 2, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 4, "type2"),
                    },
                    Chance = 2,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeHorseman, 2, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 3, "type2"),
                    },
                    Chance = 1,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 5, "type2"),
                    },
                    Chance = 15,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 5, "type2"),
                    },
                    Chance = 15,
                },
            };
            this.SectionMultipliers = new Dictionary<string, float>()
            {
                { "secA", 1 },
                { "secB", 1.2f },
                { "secC", 1.2f },
                { "secD", 1 },
            };
            this.ZoneName = "pit";
            this.MobDeathResetNumber = 20;

            this.Start();
        }
    }
}