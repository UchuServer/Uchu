namespace Uchu.Core
{
    public class MovingPlatformPath : IPath
    {
        public string Name { get; set; }
        public PathType Type { get; set; }
        public PathBehavior Behavior { get; set; }
        public IPathWaypoint[] Waypoints { get; set; }
    }
}