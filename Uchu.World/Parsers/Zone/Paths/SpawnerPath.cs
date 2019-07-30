using Uchu.Core;

namespace Uchu.World.Parsers
{
    public class SpawnerPath : IPath
    {
        public string Name { get; set; }
        public PathType Type { get; set; }
        public PathBehavior Behavior { get; set; }
        public IPathWaypoint[] Waypoints { get; set; }

        public uint SpawnLOT { get; set; }
        public uint RespawnTime { get; set; }
        public int MaxSpawnCount { get; set; }
        public uint MaintainCount { get; set; }
        public long SpawnerObjectId { get; set; }
        public bool ActivateOnNetworkLoad { get; set; }
    }
}