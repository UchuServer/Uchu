using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet;
using Uchu.Core.Collections;
using Uchu.Core.IO;

namespace Uchu.Core
{
    public class ZoneParser
    {
        private readonly IResources _resources;
        private readonly Dictionary<ushort, Zone> _zones;

        public LevelParser LevelParser { get; }

        public ZoneParser(IResources resources)
        {
            _resources = resources;
            _zones = new Dictionary<ushort, Zone>();

            LevelParser = new LevelParser(resources);
        }

        public async Task<Zone> ParseAsync(string path)
        {
            var data = await _resources.ReadBytesAsync(path);
            var stream = new BitStream(data);

            var version = stream.ReadUInt();

            if (version < 0x27)
                throw new InvalidDataException("File version must be >=0x27");

            var revision = stream.ReadUInt();
            var zoneId = stream.ReadUInt();

            if (_zones.ContainsKey((ushort) zoneId))
                return _zones[(ushort) zoneId];

            var zone = new Zone
            {
                ZoneId = zoneId,
                SpawnPosition = new Vector3
                {
                    X = stream.ReadFloat(),
                    Y = stream.ReadFloat(),
                    Z = stream.ReadFloat()
                },
                SpawnRotation = new Vector4
                {
                    X = stream.ReadFloat(),
                    Y = stream.ReadFloat(),
                    Z = stream.ReadFloat(),
                    W = stream.ReadFloat()
                }
            };

            var sceneCount = stream.ReadUInt();
            zone.Scenes = new Scene[sceneCount];

            for (var i = 0; i < sceneCount; i++)
            {
                var scene = new Scene();
                var filename = stream.ReadString(stream.ReadByte());
                scene.SceneId = stream.ReadByte();

                stream.Read(3);

                scene.Audio = stream.ReadByte() == 1;

                stream.Read(3);

                scene.Name = stream.ReadString(stream.ReadByte());

                var dir = Path.GetDirectoryName(path);

                scene.Objects = await LevelParser.ParseAsync(Path.Combine(dir, filename));

                stream.Read(3);

                zone.Scenes[i] = scene;
            }

            stream.ReadByte();

            zone.MapFilename = stream.ReadString(stream.ReadByte());
            zone.MapName = stream.ReadString(stream.ReadByte());
            zone.MapDescription = stream.ReadString(stream.ReadByte());

            var transCount = stream.ReadUInt();
            zone.Transitions = new Transition[transCount][];

            for (var i = 0; i < transCount; i++)
            {
                zone.Transitions[i] = new Transition[2];

                for (var ii = 0; ii < 2; ii++)
                {
                    zone.Transitions[i][ii] = new Transition
                    {
                        SceneId = stream.ReadULong(),
                        Position = new Vector3
                        {
                            X = stream.ReadFloat(),
                            Y = stream.ReadFloat(),
                            Z = stream.ReadFloat()
                        }
                    };
                }
            }

            var remainingLength = stream.ReadUInt();

            stream.ReadUInt();

            var pathCount = stream.ReadUInt();

            zone.Paths = new IPath[pathCount];

            for (var i = 0; i < pathCount; i++)
            {
                IPath pth;
                var pathVersion = stream.ReadUInt();
                var pathName = stream.ReadString(stream.ReadByte(), true);
                var pathType = (PathType) stream.ReadUInt();

                stream.ReadUInt();

                var pathBehavior = (PathBehavior) stream.ReadUInt();

                switch (pathType)
                {
                    case PathType.MovingPlatform:
                        pth = new MovingPlatformPath();

                        if (pathVersion >= 18)
                            stream.ReadByte();
                        else if (pathVersion >= 13)
                            stream.ReadString(stream.ReadByte(), true);
                        break;

                    case PathType.Property:
                        stream.ReadInt();

                        var propPath = new PropertyPath
                        {
                            Price = stream.ReadInt(),
                            RentalTime = stream.ReadInt(),
                            Zone = stream.ReadULong(),
                            PropertyName = stream.ReadString(stream.ReadByte(), true),
                            PropertyDescription = stream.ReadString((int) stream.ReadUInt(), true)
                        };

                        stream.ReadInt();

                        propPath.CloneLimit = stream.ReadInt();
                        propPath.ReputationMultiplier = stream.ReadFloat();
                        propPath.RentalTimeUnit = (RentalTimeUnit) stream.ReadInt();
                        propPath.AchievementRequired = (PropertyAchievement) stream.ReadInt();
                        propPath.PlayerPosition = new Vector3
                        {
                            X = stream.ReadFloat(),
                            Y = stream.ReadFloat(),
                            Z = stream.ReadFloat()
                        };
                        propPath.MaxBuildHeight = stream.ReadFloat();

                        pth = propPath;
                        break;

                    case PathType.Camera:
                        pth = new CameraPath {NextPathName = stream.ReadString(stream.ReadByte(), true)};

                        if (pathVersion >= 14)
                            stream.ReadByte();

                        break;

                    case PathType.Spawner:
                        pth = new SpawnerPath
                        {
                            SpawnLOT = stream.ReadUInt(),
                            RespawnTime = stream.ReadUInt(),
                            MaxSpawnCount = stream.ReadInt(),
                            MaintainCount = stream.ReadUInt(),
                            SpawnerObjectId = stream.ReadLong(),
                            ActivateOnNetworkLoad = stream.ReadByte() == 1
                        };
                        break;

                    default:
                        pth = new GenericPath();
                        break;
                }

                pth.Name = pathName;
                pth.Type = pathType;
                pth.Behavior = pathBehavior;

                var waypointCount = stream.ReadUInt();

                pth.Waypoints = new IPathWaypoint[waypointCount];

                for (var ii = 0; ii < waypointCount; ii++)
                {
                    IPathWaypoint wp;

                    var pos = new Vector3
                    {
                        X = stream.ReadFloat(),
                        Y = stream.ReadFloat(),
                        Z = stream.ReadFloat()
                    };

                    switch (pathType)
                    {
                        case PathType.MovingPlatform:
                            wp = new MovingPlatformWaypoint
                            {
                                Rotation = new Vector4
                                {
                                    W = stream.ReadFloat(),
                                    X = stream.ReadFloat(),
                                    Y = stream.ReadFloat(),
                                    Z = stream.ReadFloat()
                                },
                                LockPlayer = stream.ReadByte() == 1,
                                Speed = stream.ReadFloat(),
                                WaitTime = stream.ReadFloat()
                            };

                            if (pathVersion >= 13)
                            {
                                stream.ReadString(stream.ReadByte(), true);
                                stream.ReadString(stream.ReadByte(), true);
                            }
                            break;

                        case PathType.Camera:
                            var camWp = new CameraWaypoint
                            {
                                Rotation = new Vector4
                                {
                                    W = stream.ReadFloat(),
                                    X = stream.ReadFloat(),
                                    Y = stream.ReadFloat(),
                                    Z = stream.ReadFloat()
                                },
                                Time = stream.ReadFloat()
                            };

                            stream.ReadFloat();

                            camWp.Tension = stream.ReadFloat();
                            camWp.Continuity = stream.ReadFloat();
                            camWp.Bias = stream.ReadFloat();

                            wp = camWp;
                            break;

                        case PathType.Spawner:
                            wp = new SpawnerWaypoint
                            {
                                Rotation = new Vector4
                                {
                                    W = stream.ReadFloat(),
                                    X = stream.ReadFloat(),
                                    Y = stream.ReadFloat(),
                                    Z = stream.ReadFloat()
                                }
                            };
                            break;

                        case PathType.Race:
                            wp = new RaceWaypoint
                            {
                                Rotation = new Vector4
                                {
                                    W = stream.ReadFloat(),
                                    X = stream.ReadFloat(),
                                    Y = stream.ReadFloat(),
                                    Z = stream.ReadFloat()
                                }
                            };

                            stream.ReadByte();
                            stream.ReadByte();
                            stream.ReadFloat();
                            stream.ReadFloat();
                            stream.ReadFloat();
                            break;

                        case PathType.Rail:
                            wp = new RailWaypoint
                            {
                                Rotation = new Vector4
                                {
                                    W = stream.ReadFloat(),
                                    X = stream.ReadFloat(),
                                    Y = stream.ReadFloat(),
                                    Z = stream.ReadFloat()
                                }
                            };

                            if (pathVersion >= 17)
                                stream.ReadFloat(); // possibly rail speed?
                            break;

                        default:
                            wp = new GenericWaypoint();
                            break;
                    }

                    wp.Config = new LegoDataDictionary();

                    if (pathType == PathType.Movement || pathType == PathType.Spawner || pathType == PathType.Rail)
                    {
                        var entryCount = stream.ReadUInt();

                        for (var iii = 0; iii < entryCount; iii++)
                        {
                            var key = stream.ReadString(stream.ReadByte(), true);
                            var tv = stream.ReadString(stream.ReadByte(), true);

                            var colonIndex = tv.IndexOf(':');
                            var type = int.Parse(tv.Substring(0, colonIndex));
                            var value = tv.Substring(colonIndex + 1);

                            wp.Config[key, (byte) type] = value;
                        }
                    }

                    wp.Position = pos;

                    pth.Waypoints[ii] = wp;
                }

                zone.Paths[i] = pth;
            }

            _zones[(ushort) zoneId] = zone;

            return zone;
        }
    }
}