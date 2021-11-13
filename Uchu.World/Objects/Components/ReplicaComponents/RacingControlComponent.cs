// Thanks to Simon Nitzsche for his research into racing
// https://github.com/SimonNitzsche/OpCrux-Server/
// https://www.youtube.com/watch?v=X5qvEDmtE5U

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class RacingControlComponent : ScriptedActivityComponent
    {
        public override ComponentId Id => ComponentId.RacingControlComponent;

        private List<PreRacePlayerInfo> _preRacePlayerInfos = new List<PreRacePlayerInfo>();
        private List<DuringRacePlayerInfo> _duringRacePlayerInfos = new List<DuringRacePlayerInfo>();

        private RaceInfo _raceInfo = new RaceInfo();

        private RacingStatus racingStatus = RacingStatus.None;

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
            Listen(this.GameObject.OnMessageBoxRespond, (player, message) =>
            {
                Logger.Information($"Button - {message.Button} {message.Identifier} {message.UserData}");
                if (message.Identifier == "ACT_RACE_EXIT_THE_RACE?" && message.Button == 1)
                {
                    // TODO: Player wants to leave race
                }
            });

            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnWorldLoad, async () => OnPlayerLoad(player));
            });
        }

        private void OnPlayerLoad(Player player)
        {
            Logger.Information("Player loaded into racing");

            // Register player
            this._preRacePlayerInfos.Add(new PreRacePlayerInfo
            {
                PlayerId = player,
                IsReady = true,
                StartingPosition = (uint)_preRacePlayerInfos.Count + 1,
            });

            LoadPlayerCar(player);
            Zone.Schedule(InitRace, 10000);
        }

        private void LoadPlayerCar(Player player)
        {
            // Get position and rotation
            var mainPath = Zone.ZoneInfo.LuzFile.PathData.FirstOrDefault(path => path.PathName == "MainPath");
            if (mainPath == default)
                throw new Exception("Path not found");

            var startPosition = mainPath.Waypoints.First().Position + Vector3.UnitY * 3;
            var spacing = 15;
            var range = _preRacePlayerInfos.Count * spacing;
            startPosition += Vector3.UnitZ * range;

            var startRotation = Quaternion.CreateFromYawPitchRoll(((float) Math.PI) * -0.5f, 0 , 0);

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
            for (var i = 0; i < _preRacePlayerInfos.Count; i++)
            {
                PreRacePlayerInfo info = _preRacePlayerInfos[i];
                if (info.PlayerId == player)
                {
                    info.VehicleId = car;
                    _preRacePlayerInfos[i] = info;
                }
            }
        }

        private void InitRace()
        {
            if (racingStatus == RacingStatus.Loaded)
                return;

            racingStatus = RacingStatus.Loaded;

            Zone.BroadcastMessage(new NotifyRacingClientMessage
            {
                Associate = this.GameObject,
                EventType = RacingClientNotificationType.ActivityStart,
            });

            // Start after 6 seconds
            Zone.Schedule(StartRace, 6000);
        }

        private void StartRace()
        {
            if (racingStatus == RacingStatus.Started)
                return;

            racingStatus = RacingStatus.Started;

            foreach (PreRacePlayerInfo info in _preRacePlayerInfos)
            {
                Zone.BroadcastMessage(new VehicleUnlockInputMessage
                {
                    Associate = info.VehicleId,
                    LockWheels = false,
                });
            }
        
            Zone.BroadcastMessage(new ActivityStartMessage
            {
                Associate = this.GameObject,
            });

            GameObject.Serialize(this.GameObject);
        }

        // todo
        // RacingClientReady client->server
        // RacingPlayerLoaded server->client
        // RacingResetPlayerToLastReset server->client --> this is like player reached respawn checkpoint
        // RacingSetPlayerResetInfo server->client
        // RacingPlayerInfoResetFinished client->server

        // in capture even before acknowledge poss:
        // teleport msg [player]
        // bIgnoreY = False
        // bSetRotation = True
        // bSkipAllChecks = False
        // pos = (-1457.711669921875, 794.0, -351.61419677734375)
        // useNavmesh = False
        // w = 0.5037656426429749
        // x = 0.0
        // y = 0.8638404011726379
        // z = 0.0

        public void OnAcknowledgePossession(GameObject possessed)
        {
            GameObject.Serialize(this.GameObject);

            Zone.BroadcastMessage(new NotifyRacingClientMessage
            {
                Associate = this.GameObject,
                EventType = RacingClientNotificationType.ActivityStart,
            });

            Zone.BroadcastMessage(new ActivityStartMessage
            {
                Associate = this.GameObject,
            });
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

            packet.PreRacePlayerInfos = this._preRacePlayerInfos.ToArray();

            packet.RaceInfo = _raceInfo; // lap count displays correctly on the client

            packet.DuringRacePlayerInfos = this._duringRacePlayerInfos.ToArray();

            // TODO

            return packet;
        }

        private enum RacingStatus {
            None,
            Loaded,
            Started
        }
    }
}
