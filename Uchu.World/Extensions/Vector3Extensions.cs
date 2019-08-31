using System;
using System.Numerics;

namespace Uchu.World
{
    public static class Vector3Extensions
    {
        public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            //
            // Stolen from Unity.
            //
            
            var deltaX = target.X - current.X;
            var deltaY = target.Y - current.Y;
            var deltaZ = target.Z - current.Z;
            
            var delta = (float) (deltaX * (double) deltaX + deltaY * (double) deltaY + deltaZ * (double) deltaZ);
            
            if (Math.Abs(delta) < 0.001f || delta <= maxDistanceDelta * (double) maxDistanceDelta)
                return target;
            
            var change = (float) Math.Sqrt(delta);
            
            return new Vector3(
                current.X + deltaX / change * maxDistanceDelta,
                current.Y + deltaY / change * maxDistanceDelta,
                current.Z + deltaZ / change * maxDistanceDelta
            );
        }
    }
}