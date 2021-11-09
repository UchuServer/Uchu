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
                Listen(player.OnWorldLoad, async () =>
                {
                    Logger.Information("Racing Control Component");
                    // Entity::GameObject * playerObject = owner->GetZoneInstance()->objectsManager->GetObjectByID(playerID);
                    // -> player


                    // // Set ObjectID
                    // myCar->SetObjectID(owner->GetZoneInstance()->objectsManager->GenerateSpawnedID());
                    // -> handled automatically

                    // // Set Position/Rotation
                    // auto path = reinterpret_cast<FileTypes::LUZ::LUZonePathRace*>(owner->GetZoneInstance()->luZone->paths.find(u"MainPath")->second);
                    // myCar->SetPosition(path->waypoints.at(0)->position);
                    var mainPath = Zone.ZoneInfo.LuzFile.PathData.FirstOrDefault(path => path.PathName == "MainPath");
                    if (mainPath == default)
                        throw new Exception("Path not found");

                    var startPosition = mainPath.Waypoints.First().Position;

                    // // Make car
                    // Entity::GameObject* myCar = new Entity::GameObject(this->owner->GetZoneInstance(), 8092);
                    // if (!myCar->isSerializable) {
                    // Spawn Error Object
                    // delete myCar;
                    // myCar = new Entity::GameObject(this->owner->GetZoneInstance(), 1845);
                    GameObject car = GameObject.Instantiate(this.GameObject.Zone.ZoneControlObject, 8092, startPosition + new Vector3(20, 50, 0));
                    // car.Transform.Position = startPosition;

                    player.Teleport(startPosition + new Vector3(20, 50, 0));

                    // auto modAComp = myCar->GetComponent<ModuleAssemblyComponent>();
                    var moduleAssemblyComponent = car.GetComponent<ModuleAssemblyComponent>();
                    // modAComp->SetAssembly(u"1:8129;1:8130;1:13513;1:13512;1:13515;1:13516;1:13514;");
                    moduleAssemblyComponent.SetAssembly("1:8129;1:8130;1:14709;1:14708;1:14711;1:14712;1:14710;");

                    // // Set Parent
                    // myCar->SetParent(this->owner->GetZoneInstance()->zoneControlObject);
                    //
                    // // Finish & Send car
                    // myCar->Finish();
                    //
                    // // Register
                    // this->owner->GetZoneInstance()->objectsManager->RegisterObject(myCar);
                    //
                    // // Construct
                    // if (myCar->isSerializable)
                    //     this->owner->GetZoneInstance()->objectsManager->Construct(myCar);

                    Start(car);
                    GameObject.Construct(car);

                    // // Add player to activity
                    // this->AddPlayerToActivity(playerID);
                    AddPlayer(player);


                    // await UiHelper.StateAsync(player, "Race");

                    var preInfo = new PreRacePlayerInfo
                    {
                        IsReady = true,
                        PlayerId = player,
                        StartingPosition = 0,
                        VehicleId = car,
                    };
                    this._preRacePlayerInfos.Add(preInfo);
                    var duringInfo = new DuringRacePlayerInfo
                    {
                        PlayerId = player,
                        BestLapTime = 7.24f, //testing, doesnt appear on client so far
                        RaceTime = 8.97f, //testing, doesnt appear on client so far
                    };
                    this._duringRacePlayerInfos.Add(duringInfo);

                    // // Tell player car is ready and added to race
                    // this->owner->GetZoneInstance()->objectsManager->Serialize(this->owner);
                    GameObject.Serialize(this.GameObject);

                    // playerObject->Possess(myCar);
                    Listen(player.OnAcknowledgePossession, this.OnAcknowledgePossession);
                    car.GetComponent<PossessableComponent>().Driver = player;
                    player.GetComponent<CharacterComponent>().VehicleObject = car;

                    // { GM::NotifyVehicleOfRacingObject msg; msg.racingObjectID = this->owner->GetObjectID(); GameMessages::Broadcast(this->owner->GetZoneInstance(), myCar, msg); }
                    Zone.BroadcastMessage(new NotifyVehicleOfRacingObjectMessage
                    {
                        Associate = car,
                        RacingObjectId = this.GameObject, // zonecontrol
                    });
                    // { GM::RacingPlayerLoaded msg; msg.playerID = playerID; msg.vehicleID = myCar->GetObjectID(); GameMessages::Broadcast(owner->GetZoneInstance(), owner, msg); }
                    Zone.BroadcastMessage(new RacingPlayerLoadedMessage
                    {
                        Associate = this.GameObject,
                        PlayerId = player,
                        VehicleId = car,
                    });
                    // {GM::VehicleUnlockInput msg; msg.bLockWheels = false; GameMessages::Broadcast(owner->GetZoneInstance(), myCar, msg); }
                    Zone.BroadcastMessage(new VehicleUnlockInputMessage
                    {
                        Associate = car,
                        LockWheels = false,
                    });

                    GameObject.Serialize(car);
                    GameObject.Serialize(player);

                    //test
                    Zone.Schedule(() =>
                    {
                        Logger.Information("delay thingy");
                        car.Transform.Position = new Vector3(0, 250, 0);
                        GameObject.Serialize(car);
                        Zone.SendSerialization(car, new []{ player });
                    }, 10000);
                });
            });
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
    }
}
