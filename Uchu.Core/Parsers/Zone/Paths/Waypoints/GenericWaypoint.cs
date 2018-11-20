using System.Numerics;
using Uchu.Core.Collections;

namespace Uchu.Core
{
    public class GenericWaypoint : IPathWaypoint
    {
        public Vector3 Position { get; set; }
        public LegoDataDictionary Config { get; set; }
    }
}