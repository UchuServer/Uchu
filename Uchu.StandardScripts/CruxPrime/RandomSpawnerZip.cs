using System.Collections.Generic;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.CruxPrime
{
    /// <summary>
    /// Native implementation of scripts/02_server/map/am/l_random_spawner_zip.lua
    /// </summary>
    [ScriptName("ScriptComponent_1421_script_name__removed")]
    public class RandomSpawnerZip : RandomSpawnerBase
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public RandomSpawnerZip(GameObject gameObject) : base(gameObject)
        {
            this.Zones = new List<RandomSpawnerZone>()
            {
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 2, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 2, "type3"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 1, "type4"),
                    },
                    Chance = 19,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 1, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 2, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 1, "type3"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 1, "type4"),
                    },
                    Chance = 19,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 3, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 1, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 1, "type3"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 1, "type4"),
                    },
                    Chance = 10,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeHorseman, 1, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 2, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 1, "type3"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 1, "type4"),
                    },
                    Chance = 5,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeGorilla, 1, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 1, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 2, "type3"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 0, "type4"),
                    },
                    Chance = 1,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 2, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 2, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 2, "type3"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 1, "type4"),
                    },
                    Chance = 19,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 2, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 0, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 0, "type3"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 0, "type4"),
                    },
                    Chance = 1,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 4, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeAdmiral, 1, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 0, "type3"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 0, "type4"),
                    },
                    Chance = 3,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeSpider, 1, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeMech, 2, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 2, "type3"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 0, "type4"),
                    },
                    Chance = 18,
                },
                new RandomSpawnerZone
                {
                    Entries = new List<RandomSpawnerZoneEntry>()
                    {
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeHorseman, 1, "type1"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeStromling, 0, "type2"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimeRonin, 2, "type3"),
                        new RandomSpawnerZoneEntry(Lot.CruxPrimePirate, 0, "type4"),
                    },
                    Chance = 1,
                },
            };
            this.SectionMultipliers = new Dictionary<string, float>()
            {
                { "secA", 1.2f },
                { "secB", 1.2f },
            };
            this.ZoneName = "zip";
            this.MobDeathResetNumber = 20;
            
            this.Start();
        }
    }
}