using Uchu.World.Collections;

namespace Uchu.World.Parsers
{
    public class GenericPath : IPath
    {
        public string Name { get; set; }
        public PathType Type { get; set; }
        public PathBehavior Behavior { get; set; }
        public IPathWaypoint[] Waypoints { get; set; }
        public LegoDataDictionary Config { get; set; }
    }
}