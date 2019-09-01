using System.Numerics;
using Uchu.World.Collections;

namespace Uchu.World.Parsers
{
    public class LevelObject
    {
        public ulong ObjectId { get; set; }
        public Lot Lot { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public float Scale { get; set; }
        public LegoDataDictionary Settings { get; set; }
    }
}