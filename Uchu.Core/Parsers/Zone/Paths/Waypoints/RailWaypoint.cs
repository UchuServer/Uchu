using System.Numerics;
using Uchu.Core.Collections;

namespace Uchu.Core
{
    public class RailWaypoint : IPathWaypoint
    {
        public Vector3 Position { get; set; }
        public LegoDataDictionary Config { get; set; }

        public Vector4 Rotation { get; set; }
    }
}