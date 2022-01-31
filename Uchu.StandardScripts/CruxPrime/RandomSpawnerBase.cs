using System;
using System.Collections.Generic;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.CruxPrime
{
    /// <summary>
    /// Struct for storing the spawn information in a zone part.
    /// </summary>
    public struct RandomSpawnerZoneEntry
    {
        /// <summary>
        /// LOT of the enemies to spawn.
        /// </summary>
        public Lot Lot { get; set; }
        
        /// <summary>
        /// Number of the enemies to spawn.
        /// </summary>
        public int Number { get; set; }
        
        /// <summary>
        /// Name used with the spawner.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates the zone entry.
        /// </summary>
        /// <param name="lot">Lot of the enemies to spawn.</param>
        /// <param name="number">Number of the enemies to spawn.</param>
        /// <param name="name">Name used with the spawner.</param>
        public RandomSpawnerZoneEntry(Lot lot, int number, string name)
        {
            this.Lot = lot;
            this.Number = number;
            this.Name = name;
        }
    };

    /// <summary>
    /// Struct for storing the spawn information for a zone.
    /// </summary>
    public struct RandomSpawnerZone
    {
        /// <summary>
        /// Parts of the zone spawning to use.
        /// </summary>
        public List<RandomSpawnerZoneEntry> Entries { get; set; }

        /// <summary>
        /// Weight of the spawning option being selected.
        /// </summary>
        public int Chance { get; set; }
    };
    
    /// <summary>
    /// Base random spawner used in Crux Prime.
    /// </summary>
    public abstract class RandomSpawnerBase : ObjectScript
    {
        /// <summary>
        /// Options for spawning zones.
        /// </summary>
        public List<RandomSpawnerZone> Zones { get; set; }
        
        /// <summary>
        /// Multipliers for the zones for spawners.
        /// </summary>
        public Dictionary<string, float> SectionMultipliers { get; set; }
        
        /// <summary>
        /// Name of the zone.
        /// </summary>
        public string ZoneName { get; set; }

        /// <summary>
        /// Number of enemy deaths to change the spawners at.
        /// </summary>
        public int MobDeathResetNumber { get; set; } = 30;

        /// <summary>
        /// Named enemies to spawn.
        /// </summary>
        public static readonly List<Lot> NamedMobs = new List<Lot>()
        {
            Lot.CruxPrimeNamedStromling,
            Lot.CruxPrimeNamedMech,
            Lot.CruxPrimeNamedSpider,
            Lot.CruxPrimeNamedPirate,
            Lot.CruxPrimeNamedAdmiral,
            Lot.CruxPrimeNamedRonin,
            Lot.CruxPrimeNamedHorseman,
        };

        /// <summary>
        /// Spawners that are being watched for events. To prevent being reconnected.
        /// </summary>
        private readonly List<SpawnerNetwork> _spawnersWatched = new List<SpawnerNetwork>();

        /// <summary>
        /// Randomizer for selecting spawn options.
        /// </summary>
        private readonly Random _random = new Random();
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public RandomSpawnerBase(GameObject gameObject) : base(gameObject)
        {
            
        }

        /// <summary>
        /// Starts the random spawner.
        /// </summary>
        public void Start()
        {
            this.SetVar("SpawnState", "min");
            this.SetVar("JustChanged", false);
            this.SpawnMapZones();
        }

        /// <summary>
        /// Spawns the enemy zones.
        /// </summary>
        private void SpawnMapZones()
        {
            // Spawn the sections.
            foreach (var (multiplierKey, multiplierValue) in this.SectionMultipliers)
            {
                this.SpawnSection("em_" + this.ZoneName + "_" + multiplierKey, multiplierValue);
            }

            // Spawn a named enemy if the map is correct.
            if (this.ZoneName == "str")
            {
                this.SpawnNamedEnemy();
            }
            this.SetVar("bInit", true);
        }

        /// <summary>
        /// Spawns a section of the zone.
        /// </summary>
        /// <param name="sectionName">Name of the section to spawn.</param>
        /// <param name="multiplier">Multiplier of the enemies to use.</param>
        private void SpawnSection(string sectionName, float multiplier)
        {
            var spawnLoad = this.GetRandomLoad(sectionName);
            foreach (var spawnerData in spawnLoad.Entries)
            {
                if (spawnerData.Name == "") continue;
                this.SetSpawnerNetwork(sectionName + "_" + spawnerData.Name, (uint) Math.Floor(spawnerData.Number * multiplier), spawnerData.Lot);
            }
        }

        /// <summary>
        /// Sets a spawner network.
        /// </summary>
        /// <param name="spawnerName">Name of the spawner network to set.</param>
        /// <param name="spawnNumber">Number of the enemies to maintainer.</param>
        /// <param name="spawnLot">LOT of the enemies to spawn.</param>
        private void SetSpawnerNetwork(string spawnerName, uint spawnNumber, Lot spawnLot)
        {
            // Get the spawner.
            var spawner = this.GetSpawnerByName(spawnerName);
            if (spawner == null) return;

            // Ensure only 1 Crux Prime Gorilla can spawn at a time.
            if (spawnLot == Lot.CruxPrimeGorilla && spawnNumber > 1)
            {
                spawnNumber = 1;
            }

            // Set the LOT and respawn time of the spawner.
            if (spawnLot != 0)
            {
                spawner.SetLot(spawnLot);
                spawner.SetRespawnTime(80);
            }

            // Set the number of enemies to maintain.
            if (spawnNumber != 0)
            {
                spawner.SpawnsToMaintain = spawnNumber;
                spawner.MaxToSpawn = (int) spawnNumber;
            }
            
            // Activate the spawner.
            if (spawnerName != "Named_Enemies")
            {
                spawner.Activate();
                spawner.SpawnAll();
            }
            else
            {
                spawner.TrySpawn();
            }

            // Listen for enemies being smashed.
            if (this._spawnersWatched.Contains(spawner)) return;
            this._spawnersWatched.Add(spawner);
            Listen(spawner.OnRespawnInitiated, (_) =>
            {
                this.NotifySpawnerOfDeath(spawner);
            });
        }

        /// <summary>
        /// Gets a random set of enemies to spawn.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns>The random set to spawn.</returns>
        private RandomSpawnerZone GetRandomLoad(string sectionName)
        {
            // Get the total weight,
            var zoneInfo = sectionName.Split('_');
            var totalWeight = 0;
            foreach (var zone in this.Zones)
            {
                totalWeight += zone.Chance;
            }

            // Return a random entry.
            var randomWeight = this._random.Next(0, totalWeight);
            var weight = 0;
            foreach (var zone in this.Zones)
            {
                weight += zone.Chance;
                if (randomWeight <= weight)
                {
                    return zone;
                }
            }
            return this.Zones[0];
        }

        /// <summary>
        /// Event handler for an enemy being smashed.
        /// </summary>
        /// <param name="spawner">Spawner the enemy is part of.</param>
        private void NotifySpawnerOfDeath(SpawnerNetwork spawner)
        {
            // Handle the named enemy death.
            if (spawner.Name == "Named_Enemies")
            {
                this.NamedEnemyDeath();
                return;
            }

            // Get the current deaths for the current zone.
            var sectionName = spawner.Name.Substring(0, spawner.Name.Length - 6);
            var variableName = "mobsDead" + sectionName;
            var mobDeathCount = this.GetVar<uint>(variableName);
            mobDeathCount += 1;

            // Change the spawners if the reset number was reached.
            if (mobDeathCount >= this.MobDeathResetNumber)
            {
                var zoneInfo = sectionName.Split('_');
                this.SpawnSection(sectionName, this.SectionMultipliers[zoneInfo[2]]);
                mobDeathCount = 0;
            }
            
            // Store the counter.
            this.SetVar(variableName, mobDeathCount);
        }

        /// <summary>
        /// Handles a named enemy being smashed.
        /// </summary>
        private void NamedEnemyDeath()
        {
            // Start a timer for respawning the named enemy.
            var respawnDelay = (this._random.NextSingle() + 1.0f) * 450;
            this.AddTimerWithCancel(respawnDelay, "SpawnNewEnemy");
        }

        /// <summary>
        /// Callback for the timer completing.
        /// </summary>
        /// <param name="timerName">Timer that was completed.</param>
        public override void OnTimerDone(string timerName)
        {
            if (timerName == "SpawnNewEnemy")
            {
                this.SpawnNamedEnemy();
            }
        }

        /// <summary>
        /// Spawns a named enemy.
        /// </summary>
        private void SpawnNamedEnemy()
        {
            var enemy = NamedMobs[this._random.Next(NamedMobs.Count)];
            this.SetSpawnerNetwork("Named_Enemies", 1, enemy);
        }
    }
}