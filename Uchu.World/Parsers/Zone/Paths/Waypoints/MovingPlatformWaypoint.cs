using System.Numerics;
using Uchu.World.Collections;

namespace Uchu.World.Parsers
{
    public class MovingPlatformWaypoint : IPathWaypoint
    {
        public Vector3 Position { get; set; }
        public LegoDataDictionary Config { get; set; }

        public Quaternion Rotation { get; set; }
        public bool LockPlayer { get; set; }
        public float Speed { get; set; }
        public float WaitTime { get; set; }
    }
}