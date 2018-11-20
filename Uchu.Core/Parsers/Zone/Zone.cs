using System.Numerics;

namespace Uchu.Core
{
    public class Zone
    {
        public uint ZoneId { get; set; }
        public Vector3 SpawnPosition { get; set; }
        public Vector4 SpawnRotation { get; set; }
        public Scene[] Scenes { get; set; }
        public string MapFilename { get; set; }
        public string MapName { get; set; }
        public string MapDescription { get; set; }
        public Transition[][] Transitions { get; set; }
        public IPath[] Paths { get; set; }
    }
}