using System.Numerics;
using Uchu.Core.Collections;

namespace Uchu.Core
{
    public class CameraWaypoint : IPathWaypoint
    {
        public Vector3 Position { get; set; }
        public LegoDataDictionary Config { get; set; }

        public Vector4 Rotation { get; set; }
        public float Time { get; set; }
        public float Tension { get; set; }
        public float Continuity { get; set; }
        public float Bias { get; set; }
    }
}