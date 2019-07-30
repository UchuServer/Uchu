using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.Core.IO;
using Uchu.World.Collections;

namespace Uchu.World.Parsers
{
    public class ZoneParser
    {
        public static Dictionary<ZoneId, string> Zones;

        private readonly AssemblyResources _resources;
        private readonly Dictionary<ushort, ZoneInfo> _zones;

        public LevelParser LevelParser { get; }

        public ZoneParser(AssemblyResources resources)
        {
            _resources = resources;
            _zones = new Dictionary<ushort, ZoneInfo>();

            LevelParser = new LevelParser(resources);
            
            if (Zones != null) return;

            Zones = new Dictionary<ZoneId, string>();

            var luzFiles = _resources.GetAllPaths().Where(p => p.EndsWith("luz")).ToArray();

            using (var ctx = new CdClientContext())
            {
                var zones = ctx.ZoneTableTable;

                foreach (var zone in zones)
                {
                    var path = zone.ZoneName.Replace("01_Live_Maps", "Maps").Replace("/", ".");

                    if (luzFiles.All(l => l != $"{_resources.Namespace}.{path}")) continue;
                    if (zone.ZoneID == null || !Enum.IsDefined(typeof(ZoneId), (ushort) zone.ZoneID.Value)) continue;
                    
                    Zones.Add((ZoneId) zone.ZoneID, $"{_resources.Namespace}.{path}");
                    Logger.Debug($"Found {(ZoneId) zone.ZoneID}!");
                }
            }
        }

        public async Task<ZoneInfo> ParseAsync(string path)
        {
            var stream = new MemoryStream(await _resources.ReadBytesAsync(path));

            using (var reader = new BitReader(stream))
            {
                var version = reader.Read<uint>();

                if (version < 0x27)
                {
                    Logger.Error($"{path} version must be >=0x27");
                    return null;
                }

                reader.Read<uint>();
                var zoneId = reader.Read<uint>();

                if (_zones.ContainsKey((ushort) zoneId))
                    return _zones[(ushort) zoneId];

                var zone = new ZoneInfo
                {
                    ZoneId = zoneId,
                    SpawnPosition = reader.Read<Vector3>(),
                    SpawnRotation = reader.Read<Quaternion>()
                };

                var sceneCount = reader.Read<uint>();
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

                    scene.Objects = await LevelParser.ParseAsync(Path.Combine(dir, filename));

                    stream.Position += 3;

                    zone.ScenesInfo[i] = scene;
                }

                reader.Read<byte>();

                zone.MapFilename = reader.ReadString(reader.Read<byte>());
                zone.MapName = reader.ReadString(reader.Read<byte>());
                zone.MapDescription = reader.ReadString(reader.Read<byte>());

                var transCount = reader.Read<uint>();
                zone.TransitionsInfo = new TransitionInfo[transCount][];

                for (var i = 0; i < transCount; i++)
                {
                    zone.TransitionsInfo[i] = new TransitionInfo[2];

                    for (var ii = 0; ii < 2; ii++)
                    {
                        zone.TransitionsInfo[i][ii] = new TransitionInfo
                        {
                            SceneId = reader.Read<ulong>(),
                            Position = reader.Read<Vector3>()
                        };
                    }
                }

                var remainingLength = reader.Read<uint>();

                reader.Read<uint>();

                var pathCount = reader.Read<uint>();;

                zone.Paths = new IPath[pathCount];

                for (var i = 0; i < pathCount; i++)
                {
                    IPath pth;
                    var pathVersion = reader.Read<uint>();;
                    var pathName = reader.ReadString(reader.Read<byte>(), true);
                    var pathType = (PathType) reader.Read<uint>();;
                    
                    reader.Read<uint>();

                    var pathBehavior = (PathBehavior) reader.Read<uint>();;

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
                            propPath.RentalTimeUnit = (RentalTimeUnit) reader.Read<int>();;
                            propPath.AchievementRequired = (PropertyAchievement) reader.Read<int>();;
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
                                SpawnLOT = reader.Read<uint>(),
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

                    var waypointCount = reader.Read<uint>();

                    pth.Waypoints = new IPathWaypoint[waypointCount];

                    for (var ii = 0; ii < waypointCount; ii++)
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
                                    },
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
                                    },
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
                                    },
                                };

                                if (pathVersion >= 17)
                                    reader.Read<float>(); // possibly rail speed?
                                break;

                            default:
                                wp = new GenericWaypoint();
                                break;
                        }

                        wp.Config = new LegoDataDictionary();

                        if (pathType == PathType.Movement || pathType == PathType.Spawner || pathType == PathType.Rail)
                        {
                            var entryCount = reader.Read<uint>();;

                            for (var iii = 0; iii < entryCount; iii++)
                            {
                                var key = reader.ReadString(reader.Read<byte>(), true);
                                var tv = reader.ReadString(reader.Read<byte>(), true);

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
}