namespace Uchu.World.Parsers
{
    public class CameraPath : IPath
    {
        public string Name { get; set; }
        public PathType Type { get; set; }
        public PathBehavior Behavior { get; set; }
        public IPathWaypoint[] Waypoints { get; set; }

        public string NextPathName { get; set; }
    }
}