using System.Numerics;

namespace Uchu.Navigation
{
    public static class NodeExtensions
    {
        public static Vector3 ToVector3(this Node node) => new Vector3
        {
            X = (float) node.X,
            Y = (float) node.Y,
            Z = (float) node.Z
        };
        
        public static Vector3 ToVector3(this Point3D node) => new Vector3
        {
            X = (float) node.X,
            Y = (float) node.Y,
            Z = (float) node.Z
        };
    }
}