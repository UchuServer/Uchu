using System.Numerics;

namespace Uchu.World.Client
{
    public class ZoneInfo
    {
        public uint ZoneId { get; set; }
        public Vector3 SpawnPosition { get; set; }
        public Quaternion SpawnRotation { get; set; }
        public SceneInfo[] ScenesInfo { get; set; }
        public string MapFilename { get; set; }
        public string MapName { get; set; }
        public string MapDescription { get; set; }
        public TransitionInfo[][] TransitionsInfo { get; set; }
        public IPath[] Paths { get; set; }
        public Trigger[] Triggers { get; set; }
    }
}