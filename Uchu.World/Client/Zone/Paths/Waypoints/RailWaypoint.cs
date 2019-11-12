using System.Numerics;
using Uchu.World.Collections;

namespace Uchu.World.Client
{
    public class RailWaypoint : IPathWaypoint
    {
        public Quaternion Rotation { get; set; }
        public Vector3 Position { get; set; }
        public LegoDataDictionary Config { get; set; }
    }
}