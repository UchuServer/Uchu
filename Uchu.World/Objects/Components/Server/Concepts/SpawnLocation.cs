using System.Numerics;
using InfectedRose.Lvl;

namespace Uchu.World
{
    public class SpawnLocation
    {
        public Vector3 Position { get; set; }
            
        public Quaternion Rotation { get; set; }
            
        public bool InUse { get; set; }

        public LegoDataDictionary Config { get; set; }
    }
}