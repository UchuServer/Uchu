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
    public class LevelParser
    {
        private readonly AssemblyResources _resources;

        public LevelParser(AssemblyResources resources)
        {
            _resources = resources;
        }

        private static IEnumerable<LevelObject> ParseChunk2001(BitReader reader)
        {
            var objCount = reader.Read<uint>();
            var objects = new LevelObject[objCount];

            for (var i = 0; i < objCount; i++)
            {
                var obj = new LevelObject
                {
                    ObjectId = reader.Read<ulong>() | 70368744177664,
                    Lot = reader.Read<int>()
                };

                reader.Read<uint>();
                reader.Read<uint>();

                obj.Position = reader.Read<Vector3>();

                obj.Rotation = new Quaternion
                {
                    W = reader.Read<float>(),
                    X = reader.Read<float>(),
                    Y = reader.Read<float>(),
                    Z = reader.Read<float>()
                };

                obj.Scale = reader.Read<float>();

                var settings = reader.ReadString((int) reader.Read<uint>(), true);

                obj.Settings = LegoDataDictionary.FromString(settings);

                objects[i] = obj;

                reader.Read<uint>();
            }

            return objects;
        }

        public async Task<LevelObject[]> ParseAsync(string path)
        {
            var data = await _resources.ReadBytesAsync(path, false);
            var objects = new List<LevelObject>();
            var stream = new MemoryStream(data);

            using (var reader = new BitReader(stream))
            {
                var header = reader.ReadString(4);

                stream.Position = 0;

                if (header == "CHNK")
                    while (!(stream.Position >= data.Length))
                    {
                        var startPos = stream.Position;

                        if (startPos % 16 != 0 || reader.ReadString(4) != "CHNK")
                            break;

                        var chunkType = reader.Read<uint>();

                        reader.Read<ushort>();
                        reader.Read<ushort>();

                        var chunkLength = reader.Read<uint>();
                        var dataStart = reader.Read<uint>();

                        stream.Position = dataStart;

                        if (stream.Position % 16 != 0)
                            break;

                        if (chunkType == 2001)
                            objects.AddRange(ParseChunk2001(reader));

                        stream.Position = startPos + chunkLength;
                    }
                else
                    return objects.ToArray();
            }


            return objects.ToArray();
        }
    }
}