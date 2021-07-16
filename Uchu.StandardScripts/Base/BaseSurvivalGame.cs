using System;
using System.Collections.Generic;
using System.Linq;
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
                // These need to be sent together, otherwise the players don't show up.
                this.SetNetworkVar(new (string, object)[]
                {
                    ("Update_ScoreBoard_Players", this._players),
                    ("Show_ScoreBoard", true)
                });
            }
            
            // Move the players to the spawn point.
            this.SetPlayerSpawnPoints();
            // TODO: msg.playerID:PlayerSetCameraCyclingMode{ cyclingMode = ALLOW_CYCLE_TEAMMATES, bAllowCyclingWhileDeadOnly = true }
            
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
                // TODO: GetLeaderboardData(self, playerID, self:GetActivityID().activityID, 50)
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
            this.SetNetworkVar("NumberOfPlayers", this.GetNetworkVar<int>("NumberOfPlayers") - 1);
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
                // TODO: spawnerResetT(tSpawnerNetworks.rewardNetworks)
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
                // TODO: player.Resurrect();
                this.SetPlayerSpawnPoints();
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
        public void SetGameVariables(SurvivalConfiguration.SurvivalConfiguration configuration, object mobSets, object spawnerNetworks, object missionsToUpdate)
        {
            this._configuration = configuration;
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
            if (this._waitingPlayers.Count == 0) // TODO: Check that this._players.Count == this.GetNetworkVar<bool>("NumberOfPlayers"). Currently, NumberOfPlayers is 4 instead of the expected players.
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
            // TODO: Set health, armor, and imagination to max
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
            this.SetVar("baseMobSetNum", 1);
            this.SetVar("randMobSetNum", 1);
            this.SetVar("AcceptedDelayStarted", false);
            this._waitingPlayers.Clear();
            
            // Prepare the players.
            foreach (var player in this._players)
            {
                this._waitingPlayers.Add(player);
                this.UpdatePlayer(player);
                // TODO: this.GetLeaderboardData(player, self:GetActivityID().activityID, 50);
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
            // TODO: activateSpawnerNetwork(tSpawnerNetworks.smashNetworks)
            // TODO: These may need to be sent together.
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
            // TODO: Implement
            
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
            // TODO: spawnerResetT(tSpawnerNetworks.baseNetworks)
            // TODO: spawnerResetT(tSpawnerNetworks.randNetworks)
            // TODO: spawnerResetT(tSpawnerNetworks.rewardNetworks)
            
            // Finish the players.
            foreach (var player in this._players)
            {
                // Get the values.
                var timeVar = this.GetActivityValue(player, 1);
                var scoreVar = this.GetActivityValue(player, 0);
                // TODO: self:NotifyClientZoneObject{name = 'Update_ScoreBoard', paramObj = playerID, paramStr = tostring(scoreVar), param1 = timeVar}
                
                // Resurrect the player.
                // TODO: playerID:Resurrect()
                
                // Update the missions.
                var taskType = this.GetVar<string>("missionType") ?? "survival_time_team";
                // TODO: playerID:UpdateMissionTask{ taskType = taskType, value = timeVar, value2 = self:GetActivityID().activityID} --  target = self, 
                /* TODO:
                 for missionID,trgtTime in pairs(missionsToUpdate) do
		            -- Determine if we are on the desired mission
		            local missionState = playerID:GetMissionState{missionID = missionID}.missionState
		            
		            -- Are we on the mission?
		            -- Do we satisfy the associated pre-requisite challenge time?
		            if((missionState == 2 or missionState == 10) and (timeVar >= trgtTime)) then
		                -- Update the task
		                playerID:UpdateMissionTask{taskType = "complete", value = missionID, value2 = 1, target = self}
		            end
		        end*/
                
                // Stop the activity.
                StopActivity(player, scoreVar, timeVar);
            }
            
            // Reset the state.
            this._waveNumber = 1;
            this._rewardTick = 1;
            this._totalSpawned = 0;
            this.SetNetworkVar("wavesStarted", false); // TODO: Determine type
            
            /*
             * TODO:
             if gConstants.bUseMobLots then
                gConstants.iLotPhase = 1                -- put LotPhase back to 1
                updateMobLots(self, tSpawnerNetworks.baseNetworks)         
                updateMobLots(self, tSpawnerNetworks.randNetworks)   
            end   
             */
            
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
        /// Returns a new random number that doesn't match the previous one.
        /// </summary>
        /// <param name="oldNumber">Old number to compare.</param>
        /// <param name="maxRandom">Max random number to use.</param>
        /// <returns>The random number.</returns>
        private string NewRandom(string oldNumber, int maxRandom)
        {
            // Return 1 if the max is 1.
            if (maxRandom == 1)
            {
                return "01";
            }

            // Generate a new number until a new number is returned.
            string newRandomNumber = null;
            do
            {
                newRandomNumber = _random.Next(maxRandom).ToString();
                if (newRandomNumber.Length == 1)
                {
                    newRandomNumber = "0" + newRandomNumber;
                }
            } while (newRandomNumber == oldNumber);
            return newRandomNumber;
        }

        // TODO: Implement and determine arguments.
        public void ActivateSpawnerNetwork()
        {
            
        }
        
        // TODO: Implement and determine arguments.
        public void SpawnerReset()
        {
            
        }
        
        // TODO: Implement and determine arguments.
        public void SpawnNow()
        {
            
        }
        
        // TODO: Implement and determine arguments.
        public void GetRandomSet()
        {
            
        }
        
        // TODO: Implement and determine arguments.
        public void GetRandomSpawnerNumber()
        {
            
        }
        
        // TODO: Implement and determine arguments.
        public void UpdateSpawner()
        {
            
        }
        
        // TODO: Implement and determine arguments.
        public void UpdateMobLots()
        {
            
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
            
            // TODO: Implement rest
        }

        /// <summary>
        /// Invoked when the timer updates.
        /// </summary>
        /// <param name="name">Name of the timer.</param>
        /// <param name="timeRemaining">Time that is remaining.</param>
        public override void OnActivityTimerUpdate(string name, float timeRemaining)
        {
            if (name == "AcceptedDelay")
            {
                this.SetNetworkVar("Update_Default_Start_Timer", Math.Ceiling(timeRemaining));
            } else if (name == "ClockTick")
            {
                this.SetNetworkVar("Update_Timer", timeRemaining);
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
                this.ActivityTimerStart("AllAcceptedDelay", 1);
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
                // TODO: this.UpdateSpawner(tSpawnerNetworks.rewardNetworks[1], 1);
                this._rewardTick += 1;

                // Reset the spawners.
                // TODO: this.SpawnerReset(tSpawnerNetworks.baseNetworks, true, 0);
                // TODO: this.SpawnerReset(tSpawnerNetworks.randNetworks, true, 0);
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