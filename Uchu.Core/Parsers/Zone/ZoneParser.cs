using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet;
using Uchu.Core.IO;

namespace Uchu.Core
{
    public class ZoneParser
    {
        public static Dictionary<ushort, string> Zones = new Dictionary<ushort, string>
        {
            [1000] = "Maps/Space_Ship/nd_space_ship.luz",
            [1001] = "Maps/Space_Ship/Battle_Instance/nd_space_ship_battle_instance.luz",
            [1100] = "Maps/Avant_Gardens/nd_avant_gardens.luz",
            [1101] = "Maps/Avant_Gardens/Survival/nd_ag_survival_battlefield.luz",
            [1200] = "Maps/Nimbus_Station/nd_nimbus_station.luz",
            [1201] = "Maps/Nimbus_Station/Pet_Ranch/nd_ns_pet_ranch.luz",
            [1203] = "Maps/Nimbus_Station/Racetrack/nd_nimbus_station_racetrack.luz",
            [1204] = "Maps/Nimbus_Station/Waves/nd_ns_waves.luz",
            [2000] = "Maps/njhub/nd_nj_monastery.luz"
        };

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

            _zones[(ushort) zoneId] = zone;

            return zone;
        }
    }
}