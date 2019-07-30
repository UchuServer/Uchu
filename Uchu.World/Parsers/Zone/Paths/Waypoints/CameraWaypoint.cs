using System.Numerics;
using Uchu.World.Collections;

namespace Uchu.World.Parsers
{
    public class CameraWaypoint : IPathWaypoint
    {
        public Vector3 Position { get; set; }
        public LegoDataDictionary Config { get; set; }

        public Quaternion Rotation { get; set; }
        public float Time { get; set; }
        public float Tension { get; set; }
        public float Continuity { get; set; }
        public float Bias { get; set; }
    }
}