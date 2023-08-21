// Thanks to Simon Nitzsche for his research into racing
// https://github.com/SimonNitzsche/OpCrux-Server/
// https://www.youtube.com/watch?v=X5qvEDmtE5U
//
// Thanks to Darkflame Universe for releasing the source code
// of their server. https://github.com/DarkflameUniverse/
// https://www.darkflameuniverse.org/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Luz;
using Microsoft.Scripting.Utils;
using RakDotNet.IO;
using Uchu.Api;
using Uchu.Api.Models;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Physics;
using Uchu.World.Filters;

namespace Uchu.World
{
    public class RacingControlComponent : ScriptedActivityComponent
    {
        public override ComponentId Id => ComponentId.RacingControlComponent;

        public List<RacingPlayerInfo> Players = new();
        
        public Event<RacingPlayerInfo> OnPlayerLap { get; set; }

        private RaceInfo _raceInfo = new();

        private RacingStatus _racingStatus = RacingStatus.None;

        private LuzPathData _path;

        private int _deathPlaneHeight;

        private MainWorldReturnData _returnData;

        private uint _rankCounter = 0;

        public RacingControlComponent()
        {
            OnPlayerLap = new Event<RacingPlayerInfo>();
            _raceInfo.LapCount = 3;
            _raceInfo.PathName = "MainPath"; // MainPath
            Listen(OnStart, async () => await LoadAsync());
        }

        private async Task LoadAsync()
        {
            UchuServer.Api.RegisterCommandCollection<RacingMatchCommands>(this);

            _path = Zone.ZoneInfo.LuzFile.PathData.FirstOrDefault(path => path.PathName == "MainPath");

            // In live this was done with zone specific scripts
            switch (Zone.ZoneId)
            {
                case 1203:
                    _deathPlaneHeight = 100;
                    await SetupActivityInfo(42);
                    _returnData = new MainWorldReturnData
                    {
                        ZoneId = 1200,
                        Position = new Vector3(248.8f, 287.4f, 186.9f),
                        Rotation = new Quaternion(0, 0.7f, 0, 0.7f),
                    };
                    break;
                case 1303:
                    _deathPlaneHeight = 0;
                    await SetupActivityInfo(39);
                    _returnData = new MainWorldReturnData
                    {
                        ZoneId = 1300,
                        Position = new Vector3(63.5f, 314.8f, 493.1f),
                        Rotation = new Quaternion(0, 0.45f, 0, 0.89f),
                    };
                    break;
                case 1403:
                    _deathPlaneHeight = 300;
                    await SetupActivityInfo(54);
                    _returnData = new MainWorldReturnData
                    {
                        ZoneId = 1400,
                        Position = new Vector3(775.4f, 238.4f, 455.2f),
                        Rotation = new Quaternion(0, 0.59f, 0, 0.8f),
                    };
                    break;
                default:
                    _deathPlaneHeight = 0;
                    _returnData = new MainWorldReturnData
                    {
                        ZoneId = 1200,
                        Position = new Vector3(248.8f, 287.4f, 186.9f),
                        Rotation = new Quaternion(0, 0.7f, 0, 0.7f),
                    };
                    break;
            }

            if (_path == default)
                throw new Exception("Path not found");

            Listen(GameObject.OnMessageBoxRespond, OnMessageBoxRespond);

            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnWorldLoad, () => OnPlayerLoad(player));
                Listen(player.OnRequestDie, (msg) => OnPlayerRequestDie(player));
                Listen(player.OnRacingPlayerInfoResetFinished, () => OnRacingPlayerInfoResetFinished(player));
                Listen(player.OnAcknowledgePossession, vehicle => OnAcknowledgePossession(player, vehicle));
            });

            Listen(Zone.OnPlayerLeave, player => RemoveRacingPlayer(player));
        }

        /// <summary>
        /// Message box response handler
        /// </summary>
        /// <param name="player"></param>
        /// <param name="message"></param>
        private async Task OnMessageBoxRespond(Player player, MessageBoxRespondMessage message)
        {
            Logger.Information($"Button - {message.Button} {message.Identifier} {message.UserData}");
            if (message.Identifier == "ACT_RACE_EXIT_THE_RACE?" && message.Button == 1 || message.Identifier == "Exit")
            {
                player.Message(new NotifyRacingClientMessage
                {
                    Associate = GameObject,
                    EventType = RacingClientNotificationType.Exit,
                    SingleClient = player,
                });

                await SendPlayerToMainWorldAsync(player);
            }
            else if (message.Identifier == "rewardButton")
            {
                await this.DropLootAsync(player, true, true);

                player.Message(new NotifyRacingClientMessage
                {
                    Associate = GameObject,
                    EventType = RacingClientNotificationType.RewardPlayer,
                    SingleClient = player,
                });
            }
        }

        private void AddExpectedPlayer(ulong objectId) {
            Logger.Information("PreLoad Player " + objectId);

            var fakePlayer = GameObject.Instantiate<Player>(
                Zone,
                position: Zone.SpawnPosition,
                rotation: Zone.SpawnRotation,
                scale: 1,
                objectId: new ObjectId(objectId),
                lot: 1
            );

            if (!IsPlayerRegistered(fakePlayer)) {
                Players.Add(new RacingPlayerInfo
                {
                    Player = fakePlayer,
                    PlayerLoaded = false,
                    PlayerIndex = (uint)Players.Count + 1,
                    NoSmashOnReload = true,
                    RaceTime = new Stopwatch(),
                    LapTime = new Stopwatch(),
                    BestLapTime = default,
                    ResetTimer = new SimpleTimer(6000),
                });
            }
        }

        private bool IsPlayerRegistered(Player player) {
            foreach (var info in Players) {
                if (info.Player.Id == player.Id)
                    return true;
            }

            return false;
        }

        private bool AllRegisteredPlayersReady() {
            foreach (var info in Players) {
                if (!info.PlayerLoaded)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// This runs when the player loads into the world, it registers the player
        /// </summary>
        /// <param name="player"></param>
        private async Task OnPlayerLoad(Player player)
        {
            Logger.Information($"{player} loaded into racing");

            // If race has already started return player to main world
            if (_racingStatus != RacingStatus.None)
            {
                await SendPlayerToMainWorldAsync(player);
                return;
            }

            // Set respawn point so when the players leave unexpectedly
            // and relogin they don't spawn in a racing world
            if (player.TryGetComponent<CharacterComponent>(out var characterComponent))
                {
                    characterComponent.LastZone = _returnData.ZoneId;
                    characterComponent.SpawnPosition = _returnData.Position;
                    characterComponent.SpawnRotation = _returnData.Rotation;
                }
            await player.GetComponent<SaveComponent>().SaveAsync();

            // Disable render distance filter
            player.Perspective.GetFilter<RenderDistanceFilter>().Distance = 10000;

            if (player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent))
                await missionInventoryComponent.RacingEnterWorld(GameObject.Zone.ZoneId);

            RacingPlayerInfo playerInfo;

            if (IsPlayerRegistered(player)) {
                playerInfo = Players.Find(info => info.Player.Id == player.Id);
                playerInfo.Player = player;
                playerInfo.PlayerLoaded = true;
            } else {
                playerInfo = new RacingPlayerInfo
                {
                    Player = player,
                    PlayerLoaded = true,
                    PlayerIndex = (uint)Players.Count + 1,
                    NoSmashOnReload = true,
                    RaceTime = new Stopwatch(),
                    LapTime = new Stopwatch(),
                    BestLapTime = default,
                    ResetTimer = new SimpleTimer(6000),
                };
                Players.Add(playerInfo);
            }

            LoadPlayerCar(player);

            Listen(playerInfo.ResetTimer.OnElapsed, () => ResetPlayer(player));

            Listen(player.OnPositionUpdate, (position, rotation) =>
            {
                if (!(position.Y < _deathPlaneHeight) || _racingStatus != RacingStatus.Started) return;
                var car = Players.First(i => i.Player == player).Vehicle;
                Zone.BroadcastMessage(new DieMessage
                {
                    Associate = car,
                    Killer = GameObject,
                    //KillType = KillType.Violent,
                    //ClientDeath = true,
                    SpawnLoot = false,
                });
                OnPlayerRequestDie(player);
            });

            // Wait for players to join
            // If all players from the queue are loaded wait 
            // another 10 seconds max
            // NOTE: This code schedules 'InitRace' multiple times
            // but 'InitRace' will only do it's thing once. There is 
            // room for improvement here...
            if (AllRegisteredPlayersReady())
                Zone.Schedule(InitRace, 10000);
            else 
                Zone.Schedule(InitRace, 30000);
        }

        /// <summary>
        /// Send the player back to the world he came from
        /// </summary>
        /// <param name="player"></param>
        private async Task SendPlayerToMainWorldAsync(Player player)
        {
            await player.SendToWorldAsync(_returnData.ZoneId, _returnData.Position, _returnData.Rotation);
            RemoveRacingPlayer(player);
        }

        private void RemoveRacingPlayer(Player player)
        {
            Players.RemoveAll(info => info.Player == player);

            if (Players.Count == 0)
            {
                Zone.Schedule(async () => await this.GameObject.UchuServer.StopAsync(), 10000);
                Logger.Information("No players left in racing world. Closing instance");
            }
        }

        /// <summary>
        /// Set up the player's car
        /// </summary>
        /// <param name="player"></param>
        /// <exception cref="Exception"></exception>
        private void LoadPlayerCar(Player player)
        {
            // Get position and rotation
            var waypoint = (LuzRaceWaypoint)_path.Waypoints.First();
            var startPosition = waypoint.Position;
            var startRotation = waypoint.Rotation;

            var spacing = 15;
            var positionOffset = startRotation.VectorMultiply(Vector3.UnitX) * Players.Count * spacing;
            startPosition += positionOffset + Vector3.UnitY * 3;

            // Create car
            player.Teleport(startPosition, startRotation);
            var car = GameObject.Instantiate(this.GameObject.Zone.ZoneControlObject, 8092, startPosition, startRotation);

            // Setup imagination
            var destroyableComponent = car.GetComponent<DestroyableComponent>();
            destroyableComponent.MaxImagination = 60;
            destroyableComponent.Imagination = 0;

            // Let the player posses the car
            // Listen(player.OnAcknowledgePossession, this.OnAcknowledgePossession);
            var possessableComponent = car.GetComponent<PossessableComponent>();
            possessableComponent.Driver = player;
            var characterComponent = player.GetComponent<CharacterComponent>();
            characterComponent.VehicleObject = car;

            // Get custom parts
            var inventory = player.GetComponent<InventoryManagerComponent>();
            var carItem = inventory.FindItem(Lot.ModularCar);
            var moduleAssemblyComponent = car.GetComponent<ModuleAssemblyComponent>();
            moduleAssemblyComponent.SetAssembly(carItem == null
                ? "1:8129;1:8130;1:13513;1:13512;1:13515;1:13516;1:13514;"
                : carItem.Settings["assemblyPartLOTs"].ToString());

            Start(car);
            GameObject.Construct(car);
            GameObject.Serialize(player);

            AddPlayer(player);

            Zone.BroadcastMessage(new RacingSetPlayerResetInfoMessage
            {
                Associate = this.GameObject,
                CurrentLap = 0,
                FurthestResetPlane = 0,
                PlayerId = player,
                RespawnPos = startPosition,
                UpcomingPlane = 1,
            });

            // TODO Set Jetpack mode?

            Zone.BroadcastMessage(new NotifyVehicleOfRacingObjectMessage
            {
                Associate = car,
                RacingObjectId = this.GameObject, // zonecontrol
            });

            Zone.BroadcastMessage(new VehicleSetWheelLockStateMessage
            {
                Associate = car,
                ExtraFriction = false,
                Locked = true
            });

            Zone.BroadcastMessage(new RacingPlayerLoadedMessage
            {
                Associate = this.GameObject,
                PlayerId = player,
                VehicleId = car,
            });

            var playerInfoIndex = Players.FindIndex(info => info.Player.Id == player.Id);
            var tempInfo = Players[playerInfoIndex];
            tempInfo.Vehicle = car;
            Players[playerInfoIndex] = tempInfo;
        }

        private void InitRace()
        {
            if (_racingStatus != RacingStatus.None)
                return;

            // Remove all players that aren't loaded yet
            for (var i = Players.Count - 1; i > 0; i--) {
                if (!Players[i].PlayerLoaded)
                    Players.RemoveAt(i);
            }

            _racingStatus = RacingStatus.Loaded;
            this.Zone.Server.SetMaxPlayerCount((uint)Players.Count);

            // Start imagination spawners
            var minSpawner = Zone.SpawnerNetworks.FirstOrDefault(gameObject => gameObject.Name == "ImaginationSpawn_Min");
            var medSpawner = Zone.SpawnerNetworks.FirstOrDefault(gameObject => gameObject.Name == "ImaginationSpawn_Med");
            var maxSpawner = Zone.SpawnerNetworks.FirstOrDefault(gameObject => gameObject.Name == "ImaginationSpawn_Max");

            minSpawner?.Activate();
            minSpawner?.SpawnAll();

            if (Players.Count > 2)
            {
                medSpawner?.Activate();
                medSpawner?.SpawnAll();
            }

            if (Players.Count > 4)
            {
                maxSpawner?.Activate();
                maxSpawner?.SpawnAll();
            }

            // Respawn points
            // This is not the most efficient way to do this.
            // This checks the distance to every respawn point every physics tick,
            // but we only really need to check the next one (or nearest few).
            for (uint i = 0; i < _path.Waypoints.Length; i++)
            {
                var proximityObject = GameObject.Instantiate(GameObject.Zone);
                var physics = proximityObject.AddComponent<PhysicsComponent>();
                var physicsObject = SphereBody.Create(
                    GameObject.Zone.Simulation,
                    _path.Waypoints[i].Position,
                    50f);
                physics.SetPhysics(physicsObject);

                // Listen for players entering and leaving.
                var playersInPhysicsObject = new List<Player>();
                var index = i;
                Listen(physics.OnEnter, async component =>
                {
                    if (component.GameObject is not Player player) return;
                    if (playersInPhysicsObject.Contains(player)) return;
                    playersInPhysicsObject.Add(player);

                    // TODO: Check for players driving in the wrong direction
                    await PlayerReachedCheckpoint(player, index);
                });
                Listen(physics.OnLeave, component =>
                {
                    if (component.GameObject is not Player player) return;
                    if (!playersInPhysicsObject.Contains(player)) return;
                    playersInPhysicsObject.Remove(player);
                });
            }

            Zone.BroadcastMessage(new NotifyRacingClientMessage
            {
                Associate = this.GameObject,
                EventType = RacingClientNotificationType.ActivityStart,
            });

            // Start after 7 seconds
            Task.Run(async () =>
            {
                await Task.Delay(7000);
                this.StartRace();
            });
        }

        private void StartRace()
        {
            if (_racingStatus != RacingStatus.Loaded)
                return;

            _racingStatus = RacingStatus.Started;

            // Go!
            foreach (var info in Players)
            {
                info.RaceTime.Start();
                info.LapTime.Start();

                Zone.BroadcastMessage(new VehicleUnlockInputMessage
                {
                    Associate = info.Vehicle,
                    LockWheels = false,
                });
            }

            Zone.BroadcastMessage(new ActivityStartMessage
            {
                Associate = this.GameObject,
            });

            GameObject.Serialize(this.GameObject);
        }

        public void OnAcknowledgePossession(Player player, GameObject vehicle)
        {
        }

        /// <summary>
        /// Respawns the player's car after a crash
        /// </summary>
        /// <param name="player"></param>
        private void OnRacingPlayerInfoResetFinished(Player player)
        {
            var playerInfoIndex = Players.FindIndex(x => x.Player == player);

            if (playerInfoIndex < 0 || playerInfoIndex >= Players.Count) {
                Logger.Warning("Invalid playerInfoIndex");
                return;
            }

            var playerInfo = Players[playerInfoIndex];
            var car = playerInfo.Vehicle;

            if (!playerInfo.NoSmashOnReload)
            {
                Zone.BroadcastMessage(new DieMessage
                {
                    Associate = car,
                    DeathType = "violent",
                    Killer = player,
                    SpawnLoot = false,
                });

                player.Message(new VehicleUnlockInputMessage
                {
                    Associate = car,
                    LockWheels = false,
                });

                player.Message(new VehicleSetWheelLockStateMessage
                {
                    Associate = car,
                    ExtraFriction = false,
                    Locked = false,
                });

                Zone.BroadcastMessage(new ResurrectMessage
                {
                    Associate = car,
                });
            }

            playerInfo.NoSmashOnReload = false;
            Players[playerInfoIndex] = playerInfo;
        }

        /// <summary>
        /// Handler for when the client tells the server that the car crashed
        /// </summary>
        /// <param name="player"></param>
        public void OnPlayerRequestDie(Player player)
        {
            var racingPlayer = Players.Find(info => info.Player == player);
            racingPlayer.SmashedTimes++;
            racingPlayer.ResetTimer.Stop();

            Logger.Information($"{player} requested death - respawning to {racingPlayer.RespawnPosition}");

            if (racingPlayer.RespawnPosition == Vector3.Zero)
                racingPlayer.RespawnPosition = _path.Waypoints.First().Position;

            player.Zone.ExcludingMessage(new VehicleRemovePassiveBoostAction
            {
                Associate = racingPlayer.Vehicle,
            }, player);

            Zone.Schedule(() =>
            {
                Zone.BroadcastMessage(new RacingSetPlayerResetInfoMessage
                {
                    Associate = GameObject,
                    CurrentLap = (int)racingPlayer.Lap,
                    FurthestResetPlane = racingPlayer.RespawnIndex,
                    PlayerId = player,
                    RespawnPos = racingPlayer.RespawnPosition + Vector3.UnitY * 3,
                    UpcomingPlane = racingPlayer.RespawnIndex + 1,
                });

                Zone.BroadcastMessage(new RacingResetPlayerToLastResetMessage
                {
                    Associate = GameObject,
                    PlayerId = player,
                });
            }, 2000);
        }

        private async Task PlayerReachedCheckpoint(Player player, uint index)
        {
            Logger.Information($"Player reached checkpoint: {index}");
            var waypoint = _path.Waypoints[index];
            var playerInfoIndex = Players.FindIndex(x => x.Player == player);
            var playerInfo = Players[playerInfoIndex];

            // Only count up
            if (!IsCheckpointValid(playerInfo.RespawnIndex, index))
            {
                // Don't reset player after respawning
                if (playerInfo.RespawnIndex == index)
                {
                    playerInfo.ResetTimer.Stop();
                    return;
                }

                // Don't restart timer if it's already running 
                if (!playerInfo.ResetTimer.IsRunning)
                    playerInfo.ResetTimer.Start();
                
                return;
            }

            playerInfo.ResetTimer.Stop();
            playerInfo.RespawnIndex = index;
            playerInfo.RespawnPosition = waypoint.Position;
            playerInfo.RespawnRotation = player.Transform.Rotation;

            // If start point is reached (with 40 sec cheat protection)
            if (index == 0 && playerInfo.LapTime.ElapsedMilliseconds > 40000)
            {
                var lapTime = (int)playerInfo.LapTime.ElapsedMilliseconds;
                playerInfo.LapTime.Restart();
                playerInfo.Lap++;
                OnPlayerLap.Invoke(playerInfo);
                Logger.Information($"{playerInfo.Player} now in lap {playerInfo.Lap}");

                // Set new best lap if applicable
                if (playerInfo.BestLapTime == default || lapTime < playerInfo.BestLapTime.Milliseconds)
                {
                    playerInfo.BestLapTime = new TimeSpan(0, 0, 0, 0, lapTime);

                    if (playerInfo.Player.TryGetComponent<MissionInventoryComponent>(out MissionInventoryComponent missionInventoryComponent))
                        await missionInventoryComponent.RacingLaptimeAsync(Zone.ZoneId, lapTime);
                }

                // If player finished race
                if (playerInfo.Lap >= _raceInfo.LapCount)
                {
                    playerInfo.RaceTime.Stop();
                    playerInfo.Finished = ++_rankCounter;

                    // Progress missions
                    if (playerInfo.Player.TryGetComponent<MissionInventoryComponent>(out MissionInventoryComponent missionInventoryComponent))
                        await missionInventoryComponent.RaceFinishedAsync(Zone.ZoneId, playerInfo.Finished, playerInfo.RaceTime.ElapsedMilliseconds, playerInfo.SmashedTimes, Players.Count);

                    // Set player score and leaderboard
                    SetParameter(playerInfo.Player, 1, Players.Count * 10 + playerInfo.Finished);
                    UpdateLeaderboard(playerInfo);
                    Logger.Information($"Race finished: {playerInfo.Player}, place {playerInfo.Finished}, time: {playerInfo.RaceTime.ElapsedMilliseconds}, smashed: {playerInfo.SmashedTimes}, best lap: {playerInfo.BestLapTime.TotalMilliseconds}");
                }
            }

            Players[playerInfoIndex] = playerInfo;
            GameObject.Serialize(this.GameObject);
        }

        private void ResetPlayer(Player player)
        {
            Logger.Information("Reset Player");
            var racingPlayer = Players.Find(info => info.Player == player);

            if (racingPlayer.RespawnPosition == Vector3.Zero)
                racingPlayer.RespawnPosition = _path.Waypoints.First().Position;

            player.Zone.ExcludingMessage(new VehicleRemovePassiveBoostAction
            {
                Associate = racingPlayer.Vehicle,
            }, player);

            Zone.BroadcastMessage(new RacingSetPlayerResetInfoMessage
            {
                Associate = GameObject,
                CurrentLap = (int)racingPlayer.Lap,
                FurthestResetPlane = racingPlayer.RespawnIndex,
                PlayerId = player,
                RespawnPos = racingPlayer.RespawnPosition + Vector3.UnitY * 5,
                UpcomingPlane = racingPlayer.RespawnIndex + 1,
            });

            Zone.BroadcastMessage(new RacingResetPlayerToLastResetMessage
            {
                Associate = GameObject,
                PlayerId = player,
            });
        }

        private bool IsCheckpointValid(uint oldIndex, uint newIndex)
        {
            // Only allow forward (with a bit of tolerance)
            if (newIndex > oldIndex && (newIndex - oldIndex) < 10)
                return true;

            // Only go from last checkpoint to first (with a bit of tolerance)
            if (newIndex == 0 && (_path.Waypoints.Length - oldIndex) < 10)
                return true;

            return false;
        }

        private void UpdateLeaderboard(RacingPlayerInfo playerInfo)
        {
            var player = playerInfo.Player;
            int lapTime = (int)(playerInfo.BestLapTime.TotalMilliseconds / 1000d);
            int raceTime = (int)(playerInfo.RaceTime.ElapsedMilliseconds / 1000d);
            var rank = playerInfo.Finished;

            Logger.Debug("RaceTime: " + raceTime + " LapTime: " + lapTime);

            var yearAndWeek = ISOWeek.GetYear(DateTime.Now) * 100 + ISOWeek.GetWeekOfYear(DateTime.Now);
            using var ctx = new UchuContext();

            var existingWeekly = ctx.ActivityScores.FirstOrDefault(entry =>
                entry.Activity == this.ActivityInfo.ActivityID
                && entry.Zone == Convert.ToInt32(player.Zone.ZoneId)
                && entry.CharacterId == (long)player.Id
                && entry.Week == yearAndWeek);

            var existingAllTime = ctx.ActivityScores.FirstOrDefault(entry =>
                entry.Activity == this.ActivityInfo.ActivityID
                && entry.Zone == Convert.ToInt32(player.Zone.ZoneId)
                && entry.CharacterId == (long)player.Id
                && entry.Week == 0);

            // Update existing weekly leaderboard entry
            if (existingWeekly != null)
            {
                existingWeekly.Time = Math.Min(existingWeekly.Time, raceTime);
                existingWeekly.BestLapTime = Math.Min(existingWeekly.BestLapTime, lapTime);
                existingWeekly.Wins += rank == 1 ? 1 : 0;

                Logger.Debug("Weekly: " + existingWeekly.Time + " Lap: " + existingWeekly.BestLapTime);
                ctx.ActivityScores.Update(existingWeekly);
            }
            // Add new entry
            else
            {
                ctx.ActivityScores.Add(new ActivityScore
                {
                    Activity = this.ActivityInfo.ActivityID ?? 0,
                    Zone = player.Zone.ZoneId,
                    CharacterId = player.Id,
                    Time = raceTime,
                    BestLapTime = lapTime,
                    Wins = rank == 1 ? 1 : 0,
                    Week = yearAndWeek,
                });
            }

            // Update existing all-time leaderboard entry.
            if (existingAllTime != null)
            {
                existingAllTime.NumPlayed++;
                existingAllTime.Time = Math.Min(existingAllTime.Time, raceTime);
                existingAllTime.BestLapTime = Math.Min(existingAllTime.BestLapTime, lapTime);
                existingAllTime.Wins += rank == 1 ? 1 : 0;
                existingAllTime.LastPlayed = DateTimeOffset.Now.ToUnixTimeSeconds();

                Logger.Debug("AllTime: " + existingAllTime.Time + " Lap: " + existingAllTime.BestLapTime);
                ctx.ActivityScores.Update(existingAllTime);
            }
            // Add new entry.
            else
            {
                ctx.ActivityScores.Add(new ActivityScore
                {
                    Activity = this.ActivityInfo.ActivityID ?? 0,
                    Zone = player.Zone.ZoneId,
                    CharacterId = player.Id,
                    Time = raceTime,
                    BestLapTime = lapTime,
                    Wins = rank == 1 ? 1 : 0,
                    LastPlayed = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    NumPlayed = 1,
                    Week = 0,
                });
            }

            ctx.SaveChanges();
        }

        public override void Construct(BitWriter writer)
        {
            this.Serialize(writer);
        }

        // override to be able to use ScriptedActivityComponent as base
        public override void Serialize(BitWriter writer)
        {
            base.Serialize(writer);
            StructPacketParser.WritePacket(this.GetSerializePacket(), writer);
        }

        public new RacingControlSerialization GetSerializePacket()
        {
            var packet = this.GetPacket<RacingControlSerialization>();

            packet.ExpectedPlayerCount = (ushort)this.Participants.Count;

            packet.PreRacePlayerInfos = this.Players.Select(info => new PreRacePlayerInfo
            {
                Player = info.Player,
                Vehicle = info.Vehicle,
                StartingPosition = info.PlayerIndex,
                IsReady = info.PlayerLoaded,
            }).ToArray();

            packet.RaceInfo = _raceInfo;

            packet.DuringRacePlayerInfos = this.Players.Select(info => new DuringRacePlayerInfo
            {
                Player = info.Player,
                BestLapTime = (float)info.BestLapTime.TotalSeconds,
                RaceTime = (float)info.RaceTime.Elapsed.TotalSeconds,
            }).ToArray();

            packet.PostRacePlayerInfos = this.Players.Select(info => new PostRacePlayerInfo
            {
                Player = info.Player,
                CurrentRank = info.Finished
            }).ToArray();

            return packet;
        }

        private enum RacingStatus
        {
            None,
            Loaded,
            Started,
        }

        private struct MainWorldReturnData
        {
            public ZoneId ZoneId { get; set; }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
        }

        public struct RacingPlayerInfo
        {
            public Player Player { get; set; }
            public GameObject Vehicle { get; set; }
            public uint PlayerIndex { get; set; }
            public bool PlayerLoaded { get; set; }
            public float[] Data { get; set; }
            public Vector3 RespawnPosition { get; set; }
            public Quaternion RespawnRotation { get; set; }
            public uint RespawnIndex { get; set; }
            public uint Lap { get; set; }
            public uint Finished { get; set; }
            public uint ReachedPoints { get; set; }
            public TimeSpan BestLapTime { get; set; }
            public Stopwatch LapTime { get; set; }
            public uint SmashedTimes { get; set; }
            public bool NoSmashOnReload { get; set; }
            public bool CollectedRewards { get; set; }
            public Stopwatch RaceTime { get; set; }
            public SimpleTimer ResetTimer { get; set; }
        };

        public class RacingMatchCommands {

            RacingControlComponent RacingControl;

            public RacingMatchCommands(RacingControlComponent racingControl) {
                this.RacingControl = racingControl;
            }

            [ApiCommand("match/addMatchPlayer")]
            public object AddMatchPlayer(string id)
            {
                var response = new BaseResponse();

                if (ulong.TryParse(id, out var objectId))
                {
                    RacingControl.AddExpectedPlayer(objectId);
                    response.Success = true;
                    return response;
                }

                response.FailedReason = "invalid id";
                return response;
            }
        }
    }
}
