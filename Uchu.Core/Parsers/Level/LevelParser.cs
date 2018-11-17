using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet;
using Uchu.Core.Collections;
using Uchu.Core.IO;

namespace Uchu.Core
{
    public class LevelParser
    {
        private readonly IResources _resources;

        public LevelParser(IResources resources)
        {
            _resources = resources;
        }

        private LevelObject[] _parseChunk2001(BitStream stream)
        {
            var objCount = stream.ReadUInt();
            var objects = new LevelObject[objCount];

            for (var i = 0; i < objCount; i++)
            {
                var obj = new LevelObject
                {
                    ObjectId = stream.ReadULong() | 70368744177664,
                    LOT = stream.ReadInt()
                };

                stream.ReadUInt();
                stream.ReadUInt();

                obj.Position = new Vector3
                {
                    X = stream.ReadFloat(),
                    Y = stream.ReadFloat(),
                    Z = stream.ReadFloat()
                };

                obj.Rotation = new Vector4
                {
                    W = stream.ReadFloat(),
                    X = stream.ReadFloat(),
                    Y = stream.ReadFloat(),
                    Z = stream.ReadFloat()
                };

                obj.Scale = stream.ReadFloat();

                var settings = stream.ReadString((int) stream.ReadUInt(), true);

                obj.Settings = LegoDataDictionary.FromString(settings);

                objects[i] = obj;

                stream.ReadUInt();
            }

            return objects;
        }

        public async Task<LevelObject[]> ParseAsync(string path)
        {
            var data = await _resources.ReadBytesAsync(path);
            var objects = new List<LevelObject>();
            var stream = new BitStream(data);

            var header = stream.ReadString(4);

            stream.ReadPosition = 0;

            if (header == "CHNK")
            {
                while (!stream.AllRead)
                {
                    var startPos = stream.ReadPosition / 8;

                    if (startPos % 16 != 0 || stream.ReadString(4) != "CHNK")
                        break;

                    var chunkType = stream.ReadUInt();

                    stream.ReadUShort();
                    stream.ReadUShort();

                    var chunkLength = stream.ReadUInt();
                    var dataStart = stream.ReadUInt();

                    stream.ReadPosition = BitStream.BytesToBits((int) dataStart);

                    if (stream.ReadPosition / 8 % 16 != 0)
                        break;

                    if (chunkType == 2001)
                        objects.AddRange(_parseChunk2001(stream));

                    stream.ReadPosition = BitStream.BytesToBits((int) (startPos + chunkLength));
                }
            }

            return objects.ToArray();
        }
    }
}