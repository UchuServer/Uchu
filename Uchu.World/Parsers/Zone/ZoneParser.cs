using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.IO;
using Uchu.World.Collections;

namespace Uchu.World.Parsers
{
    public class ZoneParser
    {
        private readonly IFileResources _resources;

        public readonly LevelParser LevelParser;
        public readonly Dictionary<ZoneId, ZoneInfo> Zones;

        public ZoneParser(IFileResources resources)
        {
            _resources = resources;

            LevelParser = new LevelParser(resources);
            Zones = new Dictionary<ZoneId, ZoneInfo>();
        }

        public async Task LoadZoneDataAsync()
        {
            Zones.Clear();

            var luzFiles = _resources.GetAllFilesWithExtension("luz");

            foreach (var luzFile in luzFiles)
                try
                {
                    var zoneInfo = await ParseAsync(luzFile);

                    Zones[(ZoneId) zoneInfo.ZoneId] = zoneInfo;
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to parse {luzFile}: {e.Message}\n{e.StackTrace}");
                }
        }

        private async Task<ZoneInfo> ParseAsync(string path)
        {
            byte[] data;

            try
            {
                data = await _resources.ReadBytesAsync(path);
            }
            catch
            {
                Logger.Error($"Failed to find {path}");
                return null;
            }

            await using var stream = new MemoryStream(data);
            using var reader = new BitReader(stream);
            var version = reader.Read<uint>();

            if (version >= 0x24)
                reader.Read<uint>();

            var worldId = reader.Read<uint>();

            var zone = new ZoneInfo
            {
                ZoneId = worldId,
                SpawnPosition = version >= 0x26 ? reader.Read<Vector3>() : Vector3.Zero,
                SpawnRotation = version >= 0x26 ? reader.Read<Quaternion>() : Quaternion.Identity
            };

            var sceneCount = version < 0x25 ? reader.Read<byte>() : reader.Read<uint>();

            zone.ScenesInfo = new SceneInfo[sceneCount];

            for (var i = 0; i < sceneCount; i++)
            {
                var scene = new SceneInfo();
                var filename = reader.ReadString(reader.Read<byte>());
                scene.SceneId = reader.Read<byte>();

                stream.Position += 3;

                scene.Audio = reader.Read<byte>() == 1;

                stream.Position += 3;

                scene.Name = reader.ReadString(reader.Read<byte>());

                var dir = Path.GetDirectoryName(path);

                try
                {
                    scene.Objects = await LevelParser.ParseAsync(Path.Combine(dir, filename));
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to parse {dir}: {e.Message}\n{e.StackTrace}");
                }

                stream.Position += 3;

                zone.ScenesInfo[i] = scene;
            }

            reader.Read<byte>();

            zone.MapFilename = reader.ReadString(reader.Read<byte>());
            zone.MapName = reader.ReadString(reader.Read<byte>());
            zone.MapDescription = reader.ReadString(reader.Read<byte>());

            if (version >= 0x20)
            {
                var transCount = reader.Read<uint>();
                zone.TransitionsInfo = new TransitionInfo[transCount][];

                for (var i = 0; i < transCount; i++)
                {
                    zone.TransitionsInfo[i] = new TransitionInfo[2];

                    for (var ii = 0; ii < 2; ii++)
                        zone.TransitionsInfo[i][ii] = new TransitionInfo
                        {
                            SceneId = reader.Read<ulong>(),
                            Position = reader.Read<Vector3>()
                        };
                }
            }

            if (version >= 0x23)
            {
                reader.Read<uint>();

                reader.Read<uint>();

                var pathCount = reader.Read<uint>();

                zone.Paths = new IPath[pathCount];

                for (var i = 0; i < pathCount; i++)
                {
                    IPath pth;

                    var pathVersion = reader.Read<uint>();
                    var pathName = reader.ReadString(reader.Read<byte>(), true);
                    var pathType = (PathType) reader.Read<uint>();

                    reader.Read<uint>();

                    var pathBehavior = (PathBehavior) reader.Read<uint>();

                    switch (pathType)
                    {
                        case PathType.MovingPlatform:
                            pth = new MovingPlatformPath();

                            if (pathVersion >= 18)
                                stream.ReadByte();
                            else if (pathVersion >= 13)
                                reader.ReadString(reader.Read<byte>(), true);
                            break;

                        case PathType.Property:
                            reader.Read<int>();

                            var propPath = new PropertyPath
                            {
                                Price = reader.Read<int>(),
                                RentalTime = reader.Read<int>(),
                                Zone = reader.Read<ulong>(),
                                PropertyName = reader.ReadString(reader.Read<byte>(), true),
                                PropertyDescription = reader.ReadString((int) reader.Read<uint>(), true)
                            };

                            reader.Read<int>();

                            propPath.CloneLimit = reader.Read<int>();
                            propPath.ReputationMultiplier = reader.Read<float>();
                            propPath.RentalTimeUnit = (RentalTimeUnit) reader.Read<int>();
                            propPath.AchievementRequired = (PropertyAchievement) reader.Read<int>();
                            propPath.PlayerPosition = reader.Read<Vector3>();
                            propPath.MaxBuildHeight = reader.Read<float>();

                            pth = propPath;
                            break;

                        case PathType.Camera:
                            pth = new CameraPath {NextPathName = reader.ReadString(reader.Read<byte>(), true)};

                            if (pathVersion >= 14)
                                stream.ReadByte();

                            break;

                        case PathType.Spawner:
                            pth = new SpawnerPath
                            {
                                SpawnLot = reader.Read<uint>(),
                                RespawnTime = reader.Read<uint>(),
                                MaxSpawnCount = reader.Read<int>(),
                                MaintainCount = reader.Read<uint>(),
                                SpawnerObjectId = reader.Read<long>(),
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

                    var wayPointCount = reader.Read<uint>();

                    pth.Waypoints = new IPathWaypoint[wayPointCount];

                    for (var ii = 0; ii < wayPointCount; ii++)
                    {
                        IPathWaypoint wp;

                        var pos = reader.Read<Vector3>();

                        switch (pathType)
                        {
                            case PathType.MovingPlatform:
                                wp = new MovingPlatformWaypoint
                                {
                                    Rotation = new Quaternion
                                    {
                                        W = reader.Read<float>(),
                                        X = reader.Read<float>(),
                                        Y = reader.Read<float>(),
                                        Z = reader.Read<float>()
                                    },
                                    LockPlayer = stream.ReadByte() == 1,
                                    Speed = reader.Read<float>(),
                                    WaitTime = reader.Read<float>()
                                };

                                if (pathVersion >= 13)
                                {
                                    reader.ReadString(reader.Read<byte>(), true);
                                    reader.ReadString(reader.Read<byte>(), true);
                                }

                                break;

                            case PathType.Camera:
                                var camWp = new CameraWaypoint
                                {
                                    Rotation = new Quaternion
                                    {
                                        W = reader.Read<float>(),
                                        X = reader.Read<float>(),
                                        Y = reader.Read<float>(),
                                        Z = reader.Read<float>()
                                    },
                                    Time = reader.Read<float>()
                                };

                                reader.Read<float>();

                                camWp.Tension = reader.Read<float>();
                                camWp.Continuity = reader.Read<float>();
                                camWp.Bias = reader.Read<float>();

                                wp = camWp;
                                break;

                            case PathType.Spawner:
                                wp = new SpawnerWaypoint
                                {
                                    Rotation = new Quaternion
                                    {
                                        W = reader.Read<float>(),
                                        X = reader.Read<float>(),
                                        Y = reader.Read<float>(),
                                        Z = reader.Read<float>()
                                    }
                                };
                                break;

                            case PathType.Race:
                                wp = new RaceWaypoint
                                {
                                    Rotation = new Quaternion
                                    {
                                        W = reader.Read<float>(),
                                        X = reader.Read<float>(),
                                        Y = reader.Read<float>(),
                                        Z = reader.Read<float>()
                                    }
                                };

                                reader.Read<byte>();
                                reader.Read<byte>();
                                reader.Read<float>();
                                reader.Read<float>();
                                reader.Read<float>();
                                break;

                            case PathType.Rail:
                                wp = new RailWaypoint
                                {
                                    Rotation = new Quaternion
                                    {
                                        W = reader.Read<float>(),
                                        X = reader.Read<float>(),
                                        Y = reader.Read<float>(),
                                        Z = reader.Read<float>()
                                    }
                                };

                                if (pathVersion >= 17)
                                    reader.Read<float>(); // possibly rail speed?
                                break;

                            default:
                                wp = new GenericWaypoint();
                                break;
                        }

                        wp.Config = new LegoDataDictionary();

                        if (pathType == PathType.Movement || pathType == PathType.Spawner ||
                            pathType == PathType.Rail)
                        {
                            var entryCount = reader.Read<uint>();

                            for (var iii = 0; iii < entryCount; iii++)
                            {
                                var key = reader.ReadString(reader.Read<byte>(), true);
                                var tv = reader.ReadString(reader.Read<byte>(), true);

                                /*
                                    var colonIndex = tv.IndexOf(':');
                                    var type = int.Parse(tv.Substring(0, colonIndex));
                                    var value = tv.Substring(colonIndex + 1);

                                    wp.Config[key, (byte) type] = value;
                                    */
                            }
                        }

                        wp.Position = pos;

                        pth.Waypoints[ii] = wp;
                    }

                    zone.Paths[i] = pth;
                }
            }

            return zone;
        }
    }
}