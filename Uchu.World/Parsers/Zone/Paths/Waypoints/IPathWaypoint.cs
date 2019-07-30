using System.Numerics;
using Uchu.World.Collections;

namespace Uchu.World.Parsers
{
    public interface IPathWaypoint
    {
        Vector3 Position { get; set; }
        LegoDataDictionary Config { get; set; }
    }
}