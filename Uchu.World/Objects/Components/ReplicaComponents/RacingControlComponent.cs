// Thanks to Simon Nitzsche for his research into racing
// https://github.com/SimonNitzsche/OpCrux-Server/
// https://www.youtube.com/watch?v=X5qvEDmtE5U

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Luz;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Physics;

namespace Uchu.World
{
    public class RacingControlComponent : ScriptedActivityComponent
    {
        public override ComponentId Id => ComponentId.RacingControlComponent;

        private List<RacingPlayerInfo> _players = new List<RacingPlayerInfo>();

        private RaceInfo _raceInfo = new RaceInfo();

        private RacingStatus _racingStatus = RacingStatus.None;

        private LuzPathData _path;

        private int _deathPlaneHeight;

        private MainWorldReturnData _returnData;

        public RacingControlComponent()
        {
            _raceInfo.LapCount = 3;
            _raceInfo.PathName = "MainPath"; // MainPath
            Listen(OnStart, async () =>
            {
                await LoadAsync();
            });
        }

        private async Task LoadAsync()
        {
            _path = Zone.ZoneInfo.LuzFile.PathData.FirstOrDefault(path => path.PathName == "MainPath");

            switch (Zone.ZoneId)
            {
                case 1203:
                    _deathPlaneHeight = 100;
                    _returnData = new MainWorldReturnData
                    {
                        ZoneId = 1200,
                        Position = new Vector3(248.8f, 287.4f, 186.9f),
                        Rotation = new Quaternion(0, 0.7f, 0, 0.7f),
                    };
                    break;
                case 1303:
                    _deathPlaneHeight = 0;
                    _returnData = new MainWorldReturnData
                    {
                        ZoneId = 1300,
                        Position = new Vector3(63.5f, 314.8f, 493.1f),
                        Rotation = new Quaternion(0, 0.45f, 0, 0.89f),
                    };
                    break;
                case 1403:
                    _deathPlaneHeight = 300;
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

            Listen(this.GameObject.OnMessageBoxRespond, OnMessageBoxRespond);

            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnWorldLoad, () => OnPlayerLoad(player));
                Listen(player.OnRequestDie, (msg) => OnPlayerRequestDie(player));
                Listen(player.OnRacingPlayerInfoResetFinished, () => OnRacingPlayerInfoResetFinished(player));
                Listen(player.OnAcknowledgePossession, vehicle => OnAcknowledgePossession(player, vehicle));
            });
        }

        /// <summary>
        /// Message box response handler
        /// </summary>
        /// <param name="player"></param>
        /// <param name="message"></param>
        private void OnMessageBoxRespond(Player player, MessageBoxRespondMessage message)
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

                SendPlayerToMainWorld(player);
            }
            else if (message.Identifier == "rewardButton")
            {
                // TODO: send rewards - relies on activityID being present in object settings
                // await this.DropLootAsync(player);

                player.Message(new NotifyRacingClientMessage
                {
                    Associate = GameObject,
                    EventType = RacingClientNotificationType.RewardPlayer,
                    SingleClient = player,
                });
            }
        }

        /// <summary>
        /// This runs when the player loads into the world, it registers the player
        /// </summary>
        /// <param name="player"></param>
        private void OnPlayerLoad(Player player)
        {
            Logger.Information("Player loaded into racing");

            // If race has already started return player to main world
            if (_racingStatus != RacingStatus.None)
            {
                SendPlayerToMainWorld(player);
                return;
            }

            // Register player
            this._players.Add(new RacingPlayerInfo
            {
                Player = player,
                PlayerLoaded = true,
                PlayerIndex = (uint) _players.Count + 1,
                NoSmashOnReload = true,
            });

            LoadPlayerCar(player);

            Listen(player.OnPositionUpdate, (position, rotation) =>
            {
                if (position.Y < _deathPlaneHeight && _racingStatus == RacingStatus.Started)
                {
                    OnPlayerRequestDie(player);
                }
            });

            Zone.Schedule(InitRace, 10000);
        }

        /// <summary>
        /// Send the player back to the world he came from
        /// </summary>
        /// <param name="player"></param>
        private void SendPlayerToMainWorld(Player player)
        {
            _players.RemoveAll(info => info.Player == player);
            player.SendToWorldAsync(_returnData.ZoneId, _returnData.Position, _returnData.Rotation);
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
            var positionOffset = startRotation.VectorMultiply(Vector3.UnitX) * _players.Count * spacing;
            startPosition += positionOffset + Vector3.UnitY * 3;

            // Create car
            player.Teleport(startPosition, startRotation);
            GameObject car = GameObject.Instantiate(this.GameObject.Zone.ZoneControlObject, 8092, startPosition, startRotation);

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
            if (carItem == null)
                moduleAssemblyComponent.SetAssembly("1:8129;1:8130;1:13513;1:13512;1:13515;1:13516;1:13514;"); // Fallback
            else
                moduleAssemblyComponent.SetAssembly(carItem.Settings["assemblyPartLOTs"].ToString());

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

            // Register car for player
            for (var i = 0; i < _players.Count; i++)
            {
                RacingPlayerInfo info = _players[i];
                if (info.Player == player)
                {
                    info.Vehicle = car;
                    _players[i] = info;
                }
            }
        }

        private void InitRace()
        {
            if (_racingStatus != RacingStatus.None)
                return;

            _racingStatus = RacingStatus.Loaded;

            // Start imagination spawners
            var minSpawner = Zone.SpawnerNetworks.FirstOrDefault(gameObject => gameObject.Name == "ImaginationSpawn_Min");
            var medSpawner = Zone.SpawnerNetworks.FirstOrDefault(gameObject => gameObject.Name == "ImaginationSpawn_Med");
            var maxSpawner = Zone.SpawnerNetworks.FirstOrDefault(gameObject => gameObject.Name == "ImaginationSpawn_Max");

            minSpawner?.Activate();
            minSpawner?.SpawnAll();

            if (_players.Count > 2)
            {
                medSpawner?.Activate();
                medSpawner?.SpawnAll();
            }

            if (_players.Count > 4)
            {
                maxSpawner?.Activate();
                maxSpawner?.SpawnAll();
            }

            // Respawn points
            // This is not the most efficient way to do this.
            // This checks the distance to every respawn point every physics tick,
            // but we only really need to check the next one (or nearest few).
            for (var i = 0; i < _path.Waypoints.Length; i++)
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
                this.Listen(physics.OnEnter, component =>
                {
                    if (!(component.GameObject is Player player)) return;
                    if (playersInPhysicsObject.Contains(player)) return;
                    playersInPhysicsObject.Add(player);

                    this.PlayerReachedCheckpoint(player, index);
                });
                this.Listen(physics.OnLeave, component =>
                {
                    if (!(component.GameObject is Player player)) return;
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
            foreach (var info in _players)
            {
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
            var playerInfoIndex = _players.FindIndex(x => x.Player == player);
            var playerInfo = _players[playerInfoIndex];
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
            _players[playerInfoIndex] = playerInfo;
        }

        /// <summary>
        /// Handler for when the client tells the server that the car crashed
        /// </summary>
        /// <param name="player"></param>
        public void OnPlayerRequestDie(Player player)
        {
            var racingPlayer = _players.Find(info => info.Player == player);

            Logger.Information($"{player} requested death - respawning to {racingPlayer.RespawnPosition}");

            if (racingPlayer.RespawnPosition == Vector3.Zero)
                racingPlayer.RespawnPosition = _path.Waypoints.First().Position;

            player.Zone.ExcludingMessage(new VehicleRemovePassiveBoostAction
            {
                Associate = racingPlayer.Vehicle,
            }, player);

            Zone.Schedule(() => {
                Zone.BroadcastMessage(new RacingSetPlayerResetInfoMessage
                {
                    Associate = GameObject,
                    CurrentLap = (int) racingPlayer.Lap,
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
            }, 2000);
        }

        private void PlayerReachedCheckpoint(Player player, int index)
        {
            var waypoint = _path.Waypoints[index];
            var playerInfoIndex = _players.FindIndex(x => x.Player == player);
            var playerInfo = _players[playerInfoIndex];
            playerInfo.RespawnIndex = (uint) index;
            playerInfo.RespawnPosition = waypoint.Position;
            playerInfo.RespawnRotation = player.Transform.Rotation;
            _players[playerInfoIndex] = playerInfo;

            Logger.Information($"Player reached checkpoint: {index}");
        }

        public override void Construct(BitWriter writer)
        {
            this.Serialize(writer);
        }

        // override to be able to use ScriptedActivityComponent as base
        public override void Serialize(BitWriter writer)
        {
            for (var i = 0; i < this.Parameters.Count; i++)
            {
                var parameters = Parameters[i];
                parameters[0] = 1;
                parameters[1] = 265f; // doesn't appear on client
                parameters[2] = 87f; // doesn't appear on client
            }

            base.Serialize(writer);
            StructPacketParser.WritePacket(this.GetSerializePacket(), writer);
        }

        public new RacingControlSerialization GetSerializePacket()
        {
            var packet = this.GetPacket<RacingControlSerialization>();

            packet.ExpectedPlayerCount = (ushort)this.Participants.Count;

            packet.PreRacePlayerInfos = this._players.Select(info => new PreRacePlayerInfo
            {
                Player = info.Player,
                Vehicle = info.Vehicle,
                StartingPosition = info.PlayerIndex,
                IsReady = info.PlayerLoaded,
            }).ToArray();

            packet.RaceInfo = _raceInfo;

            // packet.DuringRacePlayerInfos = this._players.Select(info => new DuringRacePlayerInfo
            // {
            //     Player = info.Player,
            //     BestLapTime = (float) info.BestLapTime.TotalSeconds,
            //     RaceTime = (float) info.RaceTime.TotalSeconds
            // }).ToArray();

            // TODO

            return packet;
        }

        private enum RacingStatus {
            None,
            Loaded,
            Started,
        }

        private struct MainWorldReturnData {
            public ZoneId ZoneId { get; set; }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
        }

        struct RacingPlayerInfo {
            public GameObject Player { get; set; }
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
            public TimeSpan LapTime { get; set; }
            public uint SmashedTimes { get; set; }
            public bool NoSmashOnReload { get; set; }
            public bool CollectedRewards { get; set; }
            public TimeSpan RaceTime { get; set; }
        };
    }
}
