using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.StandardScripts.Base.SurvivalConfiguration;
using Uchu.World;

namespace Uchu.StandardScripts.Base
{
    /// <summary>
    /// Native implementation of scripts/ai/minigame/survival/base_survival_server.lua
    /// </summary>
    public class BaseSurvivalGame : GenericActivityManager
    {
        /// <summary>
        /// Configuration for the round.
        /// </summary>
        private SurvivalConfiguration.SurvivalConfiguration _configuration;

        /// <summary>
        /// Mob sets used for the round.
        /// </summary>
        private SurvivalMobSets _mobSets;

        /// <summary>
        /// Spawner networks used for the round.
        /// </summary>
        private SurvivalSpawnerNetworks _spawnerNetworks;
        
        /// <summary>
        /// Missions to update when the round ends.
        /// </summary>
        private Dictionary<int, int> _missionsToUpdate;
        
        /// <summary>
        /// Players who entered the game.
        /// </summary>
        private List<Player> _players = new List<Player>();
        
        /// <summary>
        /// Players who haven't accepted yet.
        /// </summary>
        private List<Player> _waitingPlayers = new List<Player>();

        /// <summary>
        /// Total number of enemies spawned.
        /// </summary>
        private int _totalSpawned = 0;
        
        /// <summary>
        /// Current wave number.
        /// </summary>
        private int _waveNumber = 1;

        /// <summary>
        /// Number of rewards given.
        /// </summary>
        private int _rewardTick = 1;

        /// <summary>
        /// Randomizer for the minigame.
        /// </summary>
        private Random _random = new Random();
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public BaseSurvivalGame(GameObject gameObject) : base(gameObject)
        {
            // Start the script.
            this.OnZoneLoadedInfo();
            this.Startup();
            
            // Listen for players.
            Listen(Zone.OnPlayerLoad,(player) =>
            {
                Listen(player.OnWorldLoad, () =>
                {
                    this.PlayerLoaded(player);

                    if (!player.TryGetComponent<DestructibleComponent>(out var destructibleComponent)) return;
                    Listen(destructibleComponent.OnSmashed, (otherGameObject, otherPlayer) =>
                    {
                        this.PlayerDied(player);
                    });
                    Listen(destructibleComponent.OnResurrect, this.PlayerResurrected);
                });
                Listen(player.OnMessageBoxRespond, (buttonId, identifier, _) =>
                {
                    this.MessageBoxResponse(player, identifier, buttonId);
                });
            });
            Listen(Zone.OnPlayerLeave, this.PlayerExit);
        }

        /// <summary>
        /// Invoked when the zone loads.
        /// Technically called when the script loads, but it works
        /// if it is called on startup. Meant to be a direct mapping
        /// of onZoneLoadedInfo from the actual script.
        /// </summary>
        private void OnZoneLoadedInfo()
        {
            this.SetNetworkVar("NumberOfPlayers", 4L);
        }

        /// <summary>
        /// Invoked when the object starts up.
        /// </summary>
        private void Startup()
        {
            this.SetVar("playersAccepted", 0);
            this.SetVar("playersReady", false);
            // TODO: self:MiniGameSetParameters{numTeams = 1, playersPerTeam = 4} (Teams don't function as expected in Uchu yet)
        }

        /// <summary>
        /// Invoked when a player confirms playing.
        /// </summary>
        private void PlayerConfirmed()
        {
            var confirmedPlayers = this._players.Where(player => !this._waitingPlayers.Contains(player)).ToList();
            this.SetNetworkVar("PlayerConfirm_ScoreBoard", confirmedPlayers);
        }

        /// <summary>
        /// Invoked when a player loads.
        /// </summary>
        /// <param name="player">Player to load.</param>
        private void PlayerLoaded(Player player)
        {
            // Store the player.
            this._players.Add(player);
            this._waitingPlayers.Add(player);
            
            // Add the player to the minigame.
            this.MiniGameAddPlayer(player);
            // TODO: self:MiniGameSetTeam{playerID = msg.playerID, teamID = 1} (Teams don't function as expected in Uchu yet)
            
            // Set up the player UI.
            this.SetNetworkVar("Define_Player_To_UI", player);
            if (!this.GetNetworkVar<bool>("wavesStarted"))
            {
                this.SetNetworkVar("Update_ScoreBoard_Players", this._players);
                this.SetNetworkVar("Show_ScoreBoard", true);
            }
            
            // Move the players to the spawn point.
            this.SetPlayerSpawnPoints();
            player.Message(new PlayerSetCameraCyclingModeMessage()
            {
                AllowCyclingWhileDeadOnly = true,
                CyclingMode = CyclingMode.AllowCycleTeammates,
            });
            
            if (!this.GetNetworkVar<bool>("wavesStarted"))
            {
                // Update the confirmed player.
                this.PlayerConfirmed();
            }
            else
            {
                // Start the player.
                this._waitingPlayers.Remove(player);
                this.UpdatePlayer(player);
                this.GetLeaderboardData(player, 5, 50);
                this.ResetStats(player);
            }
        }

        /// <summary>
        /// Invoked when a player leaves.
        /// </summary>
        /// <param name="player">Player that left.</param>
        private void PlayerExit(Player player)
        {
            // Remove the player.
            if (this._players.Contains(player))
            {
                // TODO: msg.playerID:SetPlayerAllowedRespawn{dontPromptForRespawn=false}
                this._players.Remove(player);
            }
            this._waitingPlayers.Remove(player);

            if (!this.GetNetworkVar<bool>("wavesStarted"))
            {
                // Return if there are no players.
                if (this._players.Count == 0)
                {
                    return;
                }
                
                // Update the waiting players.
                this.PlayerConfirmed();
                if (this._waitingPlayers.Count == 0)
                {
                    this.ActivityTimerStopAllTimers();
                    this.ActivityTimerStart("AllAcceptedDelay", 1, this._configuration.StartDelay);
                }
                else
                {
                    if (!this.GetVar<bool>("AcceptedDelayStarted"))
                    {
                        this.SetVar("AcceptedDelayStarted", true);
                        this.ActivityTimerStart("AcceptedDelay", 1, this._configuration.AcceptedDelay);
                    }
                }
            }
            else
            {
                // End the game.
                this.UpdatePlayer(player, true);
                if (this.CheckAllPlayersDead())
                {
                    this.GameOver(player);
                }
            }
            
            // Reduce the total players.
            this.SetActivityValue(player, 1, 0);
            this.SetNetworkVar("NumberOfPlayers", this.GetNetworkVar<long>("NumberOfPlayers") - 1);
        }

        /// <summary>
        /// Performs an event.
        /// </summary>
        /// <param name="message">Name of the event.</param>
        private void FireEvent(string message)
        {
            if (message == "start")
            {
                this.StartWaves();
            }
            else
            {
                this.SpawnerReset(this._spawnerNetworks.RewardNetworks);
            }
        }

        /// <summary>
        /// Invoked when a player dies.
        /// </summary>
        /// <param name="player">Player that has died.</param>
        private void PlayerDied(Player player)
        {
            if (this.GetVar<bool>("wavesStarted"))
            {
                // Set the final time, notify the clients, and end the game.
                var finalTime = this.ActivityTimerGetCurrentTime("ClockTick");
                this.SetActivityValue(player, 1, finalTime);
                // TODO: self:NotifyClientZoneObject{name = 'Player_Died', paramObj = msg.playerID, rerouteID = msg.playerID, param1 = finalTime, paramStr = tostring(checkAllPlayersDead())}
                this.GameOver(player);
            }
            else
            {
                // Resurrect the player (most likely smashed themself).
                if (!player.TryGetComponent<DestructibleComponent>(out var destructibleComponent)) return;
                Task.Run(async () =>
                {
                    await destructibleComponent.ResurrectAsync();
                    this.SetPlayerSpawnPoints();
                });
            }
        }

        // TODO: Implement and determine arguments.
        private void NotifyObject()
        {
            
        }

        /// <summary>
        /// Invoked when a player responds to a message box.
        /// </summary>
        /// <param name="player">Player who responded.</param>
        /// <param name="name">Name of the box responded to.</param>
        /// <param name="buttonId">Id of the button responded to.</param>
        private void MessageBoxResponse(Player player, string name, int buttonId)
        {
            if (name == "RePlay")
            {
                // Accept the player.
                this.PlayerAccepted(player);
                this.PlayerConfirmed();
            } else if (name == "Exit_Question" && buttonId == 1)
            {
                // Return the player to Avant Gardens.
                this.ResetStats(player);
                this.SetNetworkVar("Exit_Waves", player);
                // TODO: msg.sender:TransferToLastNonInstance{ playerID = msg.sender, bUseLastPosition = false, pos_x = 131.83, pos_y = 376, pos_z = -180.31, rot_x = 0, rot_y = -0.268720, rot_z = 0, rot_w = 0.963218}
            }
        }
        
        /// <summary>
        /// Sets the configuration of the round.
        /// </summary>
        /// <param name="configuration">Configuration of the round.</param>
        /// <param name="mobSets">Mob sets of the round.</param>
        /// <param name="spawnerNetworks">Spawner networks of the round.</param>
        /// <param name="missionsToUpdate">Missions to update at the end of the round.</param>
        public void SetGameVariables(SurvivalConfiguration.SurvivalConfiguration configuration, SurvivalMobSets mobSets, SurvivalSpawnerNetworks spawnerNetworks, Dictionary<int, int> missionsToUpdate)
        {
            this._configuration = configuration;
            this._mobSets = mobSets;
            this._spawnerNetworks = spawnerNetworks;
            this._missionsToUpdate = missionsToUpdate;
        }

        /// <summary>
        /// Invoked when a player accepts.
        /// </summary>
        /// <param name="player">Player that has accepted.</param>
        private void PlayerAccepted(Player player)
        {
            // Remove the waiting player.
            if (!this._waitingPlayers.Contains(player)) return;
            this._waitingPlayers.Remove(player);
            
            // Start the delay.
            if (this._waitingPlayers.Count == 0) // TODO: Check that this._players.Count == this.GetNetworkVar<long>("NumberOfPlayers"). Currently, NumberOfPlayers is 4 instead of the expected players.
            {
                this.ActivityTimerStopAllTimers();
                this.ActivityTimerStart("AllAcceptedDelay", 1, this._configuration.StartDelay);
            }
            else
            {
                if (!this.GetVar<bool>("AcceptedDelayStarted"))
                {
                    this.SetVar("AcceptedDelayStarted", true);
                    this.ActivityTimerStart("AcceptedDelay", 1, this._configuration.AcceptedDelay);
                }
            }
        }

        /// <summary>
        /// Resets the stats of the player.
        /// </summary>
        /// <param name="player">Player to reset stats of.</param>
        private void ResetStats(Player player)
        {
            if (!player.TryGetComponent<DestroyableComponent>(out var destroyableComponent)) return;
            destroyableComponent.Health = destroyableComponent.MaxHealth;
            destroyableComponent.Armor = destroyableComponent.MaxArmor;
            destroyableComponent.Imagination = destroyableComponent.MaxImagination;
        }

        /// <summary>
        /// Starts the waves.
        /// </summary>
        private void StartWaves()
        {
            // Start the activity.
            this.SetupActivity(4);
            this.ActivityStart();
            this.SetVar("playersReady", true);
            this.SetVar("BaseMobSetNum", 1);
            this.SetVar("RandomMobSetNum", 1);
            this.SetVar("AcceptedDelayStarted", false);
            this._waitingPlayers.Clear();
            
            // Prepare the players.
            foreach (var player in this._players)
            {
                this._waitingPlayers.Add(player);
                this.UpdatePlayer(player);
                this.GetLeaderboardData(player, 5, 50);
                this.ResetStats(player);
                
                // Charge the player for the activity.
                if (!this.GetVar<bool>("firstTimeDone"))
                {
                    // TODO: Either remove this or move charging for activities here.
                }
            }
            this.SetVar("firstTimerDone", true);
            
            // Set the mission type to update.
            this.SetVar("missionType", this._players.Count == 1 ? "survival_time_solo" : "survival_time_team");
            
            // Start the waves.
            this.ActivateSpawnerNetwork(this._spawnerNetworks.SmashNetworks);
            this.SetNetworkVar("wavesStarted", true);
            this.SetNetworkVar("Start_Wave_Message", "Start!");
        }

        /// <summary>
        /// Returns if all the players are dead.
        /// </summary>
        /// <returns>Whether all the players are dead.</returns>
        private bool CheckAllPlayersDead()
        {
            // Return false if 1 player is alive.
            foreach (var player in this._players)
            {
                if (!player.TryGetComponent<DestroyableComponent>(out var destroyableComponent)) continue;
                if (destroyableComponent.Health > 0)
                {
                    return true;
                }
            }
            
            // Return true (no players are alive).
            return true;
        }

        /// <summary>
        /// Sets the spawn points of the players.
        /// </summary>
        private void SetPlayerSpawnPoints()
        {
            // Teleport the players, starting at the index 1 because of Lua table starting at 1.
            for (var i = 1; i <= this._players.Count; i++)
            {
                var spawnObjects = GetGroup("P" + i + "_Spawn");
                if (spawnObjects.Length == 0) continue;
                
                var player = this._players[i - 1];
                var spawnObject = spawnObjects[0];
                player.Message(new TeleportMessage
                    {
                        Associate = player,
                        Position = spawnObject.Transform.Position,
                        Rotation = spawnObject.Transform.Rotation,
                    });
            }            
        }

        /// <summary>
        /// Ends the current game.
        /// </summary>
        /// <param name="lastPlayer">Last player that died.</param>
        private void GameOver(Player lastPlayer)
        {
            if (!this.CheckAllPlayersDead()) return;
            
            // Reset the enemies.
            this.SpawnerReset(this._spawnerNetworks.BaseNetworks);
            this.SpawnerReset(this._spawnerNetworks.RandomNetworks);
            this.SpawnerReset(this._spawnerNetworks.RewardNetworks);
            
            // Finish the players.
            foreach (var player in this._players)
            {
                // Get the values.
                var timeVar = this.GetActivityValue(player, 1);
                var scoreVar = this.GetActivityValue(player, 0);
                // TODO: self:NotifyClientZoneObject{name = 'Update_ScoreBoard', paramObj = playerID, paramStr = tostring(scoreVar), param1 = timeVar}
                
                // Resurrect the player.
                if (player.TryGetComponent<DestructibleComponent>(out var destructibleComponent))
                {
                    destructibleComponent.ResurrectAsync();
                }
                
                // Update the missions.
                var taskType = this.GetVar<string>("missionType") ?? "survival_time_team";
                if (player.TryGetComponent<MissionInventoryComponent>(out var missionComponent))
                {
                    missionComponent.MinigameAchievementAsync(5, taskType, timeVar);
                    foreach (var (missionId, targetTime) in this._missionsToUpdate)
                    {
                        // Complete the custom mission if the mission is active and target was reached.
                        var mission = missionComponent.GetMission(missionId);
                        if (mission == null) continue;
                        if ((mission.State == MissionState.Active || mission.State == MissionState.CompletedActive) && timeVar >= targetTime)
                        {
                            missionComponent.ScriptAsync(mission.Tasks[0].TaskId, (int) timeVar);
                        }
                    }
                }
                
                // Stop the activity.
                StopActivity(player, scoreVar, timeVar);
            }
            
            // Reset the state.
            this._waveNumber = 1;
            this._rewardTick = 1;
            this._totalSpawned = 0;
            this.SetNetworkVar("wavesStarted", false);
            
            // Revert the enemies.
            if (this._configuration.UseMobLots)
            {
                this.UpdateMobLots(this._spawnerNetworks.BaseNetworks);
                this.UpdateMobLots(this._spawnerNetworks.RandomNetworks);
            }
            
            // Reset the spawn points.
            this.SetPlayerSpawnPoints();
        }

        /// <summary>
        /// Invoked when a player is resurrected.
        /// </summary>
        private void PlayerResurrected()
        {
            this.SetNetworkVar("Show_ScoreBoard", true);
        }

        /// <summary>
        /// Starts a spawner network.
        /// </summary>
        /// <param name="spawnerNetwork">Network to start.</param>
        public void ActivateSpawnerNetwork(SurvivalSpawnerNetworkSet spawnerNetwork)
        {
            // Activate the network.
            foreach (var spawnerData in spawnerNetwork)
            {
                foreach (var name in spawnerData.SpawnerName)
                {
                    var spawner = this.GetSpawnerByName(name + spawnerData.SpawnerNumber);
                    if (spawner == null) continue;
                    spawner.Activate();
                    spawner.SpawnAll();
                }
            }
        }
        
        /// <summary>
        /// Resets a spawner network.
        /// </summary>
        /// <param name="spawnerNetwork">Spawner network to reset.</param>
        /// <param name="maintainSpawnNumber">Whether to maintain a number of spawned enemies.</param>
        /// <param name="numberToMaintain">Number of enemies to maintain.</param>
        public void SpawnerReset(SurvivalSpawnerNetworkSet spawnerNetwork, bool maintainSpawnNumber = false, int numberToMaintain = -1)
        {
            // Reset the spawners.
            var totalSpawned = 0;
            foreach (var spawnerData in spawnerNetwork)
            {
                foreach (var name in spawnerData.SpawnerName)
                {
                    // Get the spawner.
                    var spawner = this.GetSpawnerByName(name + spawnerData.SpawnerNumber);
                    if (spawner == null) continue;
                    
                    // Add the total spawned.
                    totalSpawned += spawner.ActiveCount;
                    
                    // Destroy the objects.
                    if (!maintainSpawnNumber)
                    {
                        spawner.DestroyActiveObjects();
                    }
                    
                    // Set the objects to maintain.
                    if (numberToMaintain != -1)
                    {
                        spawner.SpawnsToMaintain = (uint) numberToMaintain;
                    }
                    
                    // Deactivate the spawner.
                    spawnerData.IsActive = false;
                    spawner.Reset();
                    spawner.Deactivate();
                }
            }
            
            // Set the total spawned.
            if (totalSpawned > this._totalSpawned)
            {
                this._totalSpawned = totalSpawned;
            }
        }
        
        /// <summary>
        /// Runs a spawner.
        /// </summary>
        /// <param name="spawner">Spawner to run.</param>
        /// <param name="spawnNumber">Total enemies to spawn.</param>
        public void SpawnNow(SpawnerNetwork spawner, int spawnNumber)
        {
            // Run the spawner.
            if (spawner == null) return;
            spawner.MaxToSpawn = spawnNumber;
            spawner.Activate();
            spawner.SpawnAll();
        }
        
        /// <summary>
        /// Returns a random set to use.
        /// </summary>
        /// <param name="setName">Name of the set.</param>
        /// <param name="setNumber">Number of the set.</param>
        /// <returns>Random set to use.</returns>
        public List<int> GetRandomSet(string setName, int setNumber)
        {
            var randomNumber = this._random.Next(0, this._mobSets[setName]["tier" + setNumber].Count);
            var randomSet = this._mobSets[setName]["tier" + setNumber][randomNumber];
            return randomSet;
        }
        
        /// <summary>
        /// Returns a random number for the given spawners.
        /// </summary>
        /// <param name="spawner">Spawners to get a random number from.</param>
        /// <returns>A random index to use from the set.</returns>
        public int GetRandomSpawnerNumber(SurvivalSpawnerNetworkSet spawner)
        {
            var randomNumber = 0;
            var valid = false;

            // Get a random number until a valid number is reached.
            while (!valid)
            {
                // Get the max random number and new random number.
                randomNumber = spawner.Count(network => !network.IsLocked);
                randomNumber = this._random.Next(0, randomNumber);

                // Determine if the number is valid.
                if (randomNumber == 0)
                {
                    valid = true;
                } else if (!spawner[randomNumber].IsActive)
                {
                    valid = true;
                    spawner[randomNumber].IsActive = true;
                }
            }
            
            // Return the number.
            return randomNumber;
        }
        
        /// <summary>
        /// Runs a spawner.
        /// </summary>
        /// <param name="network">Spawner to update.</param>
        /// <param name="spawnNumber">Number to spawn.</param>
        public void UpdateSpawner(SurvivalSpawnerNetwork network, int spawnNumber)
        {
            var spawner = this.GetSpawnerByName(network.SpawnerName[0] + network.SpawnerNumber);
            this.SpawnNow(spawner, spawnNumber);
        }
        
        /// <summary>
        /// Runs a random spawner.
        /// </summary>
        /// <param name="network">Set of spawners to randomize.</param>
        public void UpdateSpawner(SurvivalSpawnerNetworkSet network)
        {
            // Get the next spawner.
            var newSet = this.GetRandomSet(network.Set, this.GetVar<int>(network.Set + "Num"));
            var newSpawner = this.GetRandomSpawnerNumber(network);
            
            // Run the spawners.
            for (var key = 0; key < newSet.Count; key++)
            {
                var index = newSet[key] - 1;
                if (index != 0)
                {
                    var spawner = this.GetSpawnerByName(network[newSpawner].SpawnerName[key] + network[newSpawner].SpawnerNumber);
                    this.SpawnNow(spawner, index);
                }
            }
        }
        
        /// <summary>
        /// Updates the LOTs of the spawners.
        /// </summary>
        /// <param name="network">Spawner network to update.</param>
        public void UpdateMobLots(SurvivalSpawnerNetworkSet network)
        {
            var phase = this._configuration.LotPhase;
            foreach (var spawnerData in network)
            {
                foreach (var name in spawnerData.SpawnerName)
                {
                    // Get the spawner and LOT to use.
                    var spawner = this.GetSpawnerByName(name + spawnerData.SpawnerNumber);
                    var splitName = name.Split("_");
                    var lotName = splitName.Length > 1 && splitName[1] != "" ? splitName[1] : splitName[0];
                    
                    // Set the lot.
                    if (spawner == null) continue;
                    var lotSet = this._mobSets.MobLots[lotName];
                    var lot = lotSet[Math.Min(phase, lotSet.Count) - 1];
                    spawner.SetLot(lot);
                }
            }
        }

        /// <summary>
        /// Spawns the mobs.
        /// </summary>
        public void SpawnMobs()
        {
            if (!this.GetNetworkVar<bool>("wavesStarted")) return;
            
            // Increment the wave.
            this._waveNumber += 1;
            var spawnNumber = this._waveNumber;
            if (spawnNumber > this._configuration.RewardInterval)
            {
                spawnNumber += -(this._rewardTick - 1);
            }
            
            // Set the mob set numbers.
            for (var key = 0; key < this._configuration.BaseMobsStartTierAt.Count; key++)
            {
                if (this._configuration.BaseMobsStartTierAt[key] == spawnNumber)
                {
                    this.SetVar("BaseMobSetNum", key + 1);
                }
            }
            for (var key = 0; key < this._configuration.RandomMobsStartTierAt.Count; key++)
            {
                if (this._configuration.RandomMobsStartTierAt[key] == spawnNumber)
                {
                    this.SetVar("RandomMobSetNum", key + 1);
                }
            }
            
            // Unlock the third random set.
            if (this._waveNumber == this._configuration.UnlockNetwork3)
            {
                this._spawnerNetworks.RandomNetworks[2].IsLocked = false;
            }
            
            // Set the next mob set.
            this.UpdateSpawner(this._spawnerNetworks.BaseNetworks);
            if (spawnNumber >= this._configuration.StartMobSet2)
            {
                if (spawnNumber == this._configuration.StartMobSet2)
                {
                    this.SetNetworkVar("Spawn_Mob", "2");
                }
                this.UpdateSpawner(this._spawnerNetworks.RandomNetworks);
            }
            if (spawnNumber >= this._configuration.StartMobSet3)
            {
                if (spawnNumber == this._configuration.StartMobSet3)
                {
                    this.SetNetworkVar("Spawn_Mob", "3");
                }
                this.UpdateSpawner(this._spawnerNetworks.RandomNetworks);
            }
            
            // Switch to LOT phases.
            if (this._configuration.UseMobLots && this._configuration.LotPhase < this._spawnerNetworks.BaseNetworks[0].SpawnerName.Count)
            {
                if (spawnNumber >= this._configuration.BaseMobsStartTierAt[^1])
                {
                    this._configuration.LotPhase += 1;
                    this._waveNumber = 1;
                }
            }
            this.UpdateMobLots(this._spawnerNetworks.BaseNetworks);
            this.UpdateMobLots(this._spawnerNetworks.RandomNetworks);
        }

        /// <summary>
        /// Invoked when the timer updates.
        /// </summary>
        /// <param name="name">Name of the timer.</param>
        /// <param name="timeRemaining">Time that is remaining.</param>
        /// <param name="timeElapsed">Time that is elapsed.</param>
        public override void OnActivityTimerUpdate(string name, float timeRemaining, float timeElapsed)
        {
            if (name == "AcceptedDelay")
            {
                this.SetNetworkVar("Update_Default_Start_Timer", Math.Ceiling(timeRemaining));
            } else if (name == "ClockTick")
            {
                this.SetNetworkVar("Update_Timer", timeElapsed);
            } else if (name == "SpawnTick" && !this.GetVar<bool>("isCoolDown"))
            {
                this.SpawnMobs();
            }
        }

        /// <summary>
        /// Invoked when the timer is done.
        /// </summary>
        /// <param name="name">Name of the timer.</param>
        public override void OnActivityTimeDone(string name)
        {
            if (name == "AcceptedDelay")
            {
                // Start the initial delay.
                this.SetNetworkVar("Update_Default_Start_Timer", 0);
                this.ActivityTimerStart("AllAcceptedDelay", 1, 1);
            } else if (name == "AllAcceptedDelay")
            {
                // Start the waves.
                this.SetNetworkVar("Clear_Scoreboard", true);
                this.ActivityTimerStart("StartDelay", 3, 3);
                this.StartWaves();
            } else if (name == "StartDelay")
            {
                // Start the game.
                this.ActivityTimerStart("ClockTick", 1);
                this.ActivityTimerStart("SpawnTick", this._configuration.WaveTime);
                this.SpawnMobs();
                this.ActivityTimerStart("CoolDownStart", (this._configuration.RewardInterval * this._configuration.WaveTime), (this._configuration.RewardInterval * this._configuration.WaveTime));
                this.ActivityTimerStart("PlaySpawnSound", 3, 3);
            } else if (name == "CoolDownStart")
            {
                // Start the cooldown.
                this.SetVar("isCoolDown", true);
                this.ActivityTimerStop("SpawnTick");
                this.ActivityTimerStart("CoolDownStop", this._configuration.CoolDownTime, this._configuration.CoolDownTime);
                
                // update the spawner.
                this.UpdateSpawner(this._spawnerNetworks.RewardNetworks[0], 1);
                this._rewardTick += 1;

                // Reset the spawners.
                this.SpawnerReset(this._spawnerNetworks.BaseNetworks, true, 0);
                this.SpawnerReset(this._spawnerNetworks.RandomNetworks, true, 0);
            } else if (name == "CoolDownStop")
            {
                // Start the timers for the next cooldown.
                this.SetVar("isCoolDown", false);
                this.ActivityTimerStart("SpawnTick", this._configuration.WaveTime);
                this.ActivityTimerStart("CoolDownStart", (this._configuration.RewardInterval * this._configuration.WaveTime), (this._configuration.RewardInterval * this._configuration.WaveTime));
                
                // Spawn the enemies.
                this.SpawnMobs();
                this.ActivityTimerStart("PlaySpawnSound", 3, 3);
            } else if (name == "PlaySpawnSound")
            {
                // Play the horn sound.
                foreach (var player in this._players)
                {
                    player.Message(new PlayNDAudioEmitterMessage()
                    {
                        Associate = player,
                        NDAudioEventGUID = "{ca36045d-89df-4e96-a317-1e152d226b69}",
                    });
                }
            }
        }
    }
}