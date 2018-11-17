using System.Numerics;
using Uchu.Core.Collections;

namespace Uchu.Core
{
    public class LevelObject
    {
        public ulong ObjectId { get; set; }
        public int LOT { get; set; }
        public Vector3 Position { get; set; }
        public Vector4 Rotation { get; set; }
        public float Scale { get; set; }
        public LegoDataDictionary Settings { get; set; }
    }
}