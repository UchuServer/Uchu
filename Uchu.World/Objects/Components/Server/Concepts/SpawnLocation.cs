using System.Numerics;

namespace Uchu.World
{
    public class SpawnLocation
    {
        public Vector3 Position { get; set; }
            
        public Quaternion Rotation { get; set; }
            
        public bool InUse { get; set; }
    }
}