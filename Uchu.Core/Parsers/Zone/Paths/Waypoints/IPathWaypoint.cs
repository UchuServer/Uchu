using System.Numerics;
using Uchu.Core.Collections;

namespace Uchu.Core
{
    public interface IPathWaypoint
    {
        Vector3 Position { get; set; }
        LegoDataDictionary Config { get; set; }
    }
}