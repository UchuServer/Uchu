using System.Numerics;
using Uchu.World.Collections;

namespace Uchu.World.Client
{
    public class MovingPlatformWaypoint : IPathWaypoint
    {
        public Quaternion Rotation { get; set; }
        public bool LockPlayer { get; set; }
        public float Speed { get; set; }
        public float WaitTime { get; set; }
        public Vector3 Position { get; set; }
        public LegoDataDictionary Config { get; set; }
    }
}