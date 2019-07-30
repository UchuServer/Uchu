using System.Numerics;

namespace Uchu.World.Parsers
{
    public class PropertyPath : IPath
    {
        public string Name { get; set; }
        public PathType Type { get; set; }
        public PathBehavior Behavior { get; set; }
        public IPathWaypoint[] Waypoints { get; set; }

        public int Price { get; set; }
        public int RentalTime { get; set; }
        public ulong Zone { get; set; }
        public string PropertyName { get; set; }
        public string PropertyDescription { get; set; }
        public int CloneLimit { get; set; }
        public float ReputationMultiplier { get; set; }
        public RentalTimeUnit RentalTimeUnit { get; set; }
        public PropertyAchievement AchievementRequired { get; set; }
        public Vector3 PlayerPosition { get; set; }
        public float MaxBuildHeight { get; set; }
    }
}