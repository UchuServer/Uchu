using System.Collections.Generic;
using System.Numerics;

namespace Uchu.Core
{
    public class LevelObject
    {
        public ulong ObjectId { get; set; }
        public int LOT { get; set; }
        public Vector3 Position { get; set; }
        public Vector4 Rotation { get; set; }
        public float Scale { get; set; }
        public Dictionary<string, object> Settings { get; set; }
    }
}