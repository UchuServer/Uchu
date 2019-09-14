using System;
using System.Numerics;

namespace Uchu.World
{
    public static class MathExtensions
    {
        public static Quaternion ToQuaternion(this Vector3 @this)
        {
            var xOver2 = @this.X * 57.29578f * 0.5f;
            var yOver2 = @this.Y * 57.29578f * 0.5f;
            var zOver2 = @this.Z * 57.29578f * 0.5f;

            var sinXOver2 = (float) Math.Sin(xOver2);
            var cosXOver2 = (float) Math.Cos(xOver2);
            var sinYOver2 = (float) Math.Sin(yOver2);
            var cosYOver2 = (float) Math.Cos(yOver2);
            var sinZOver2 = (float) Math.Sin(zOver2);
            var cosZOver2 = (float) Math.Cos(zOver2);

            Quaternion result;
            result.X = cosYOver2 * sinXOver2 * cosZOver2 + sinYOver2 * cosXOver2 * sinZOver2;
            result.Y = sinYOver2 * cosXOver2 * cosZOver2 - cosYOver2 * sinXOver2 * sinZOver2;
            result.Z = cosYOver2 * cosXOver2 * sinZOver2 - sinYOver2 * sinXOver2 * cosZOver2;
            result.W = cosYOver2 * cosXOver2 * cosZOver2 + sinYOver2 * sinXOver2 * sinZOver2;

            return result;
        }

        public static Vector3 ToEuler(this Quaternion @this)
        {
            Vector3 euler;

            // if the input quaternion is normalized, this is exactly one. Otherwise, this acts as a correction factor for the quaternion's not-normalizedness
            var unit = (@this.X * @this.X) + (@this.Y * @this.Y) + (@this.Z * @this.Z) + (@this.W * @this.W);

            // this will have a magnitude of 0.5 or greater if and only if this is a singularity case
            var test = @this.X * @this.W - @this.Y * @this.Z;

            if (test > 0.4995f * unit) // singularity at north pole
            {
                euler.X = (float) (Math.PI / 2);
                euler.Y = (float) (2f * Math.Atan2(@this.Y, @this.X));
                euler.Z = 0;
            }
            else if (test < -0.4995f * unit) // singularity at south pole
            {
                euler.X = (float) (-Math.PI / 2);
                euler.Y = (float) (-2f * Math.Atan2(@this.Y, @this.X));
                euler.Z = 0;
            }
            else // no singularity - this is the majority of cases
            {
                euler.X = (float) Math.Asin(2f * (@this.W * @this.X - @this.Y * @this.Z));
                euler.Y = (float) Math.Atan2(2f * @this.W * @this.Y + 2f * @this.Z * @this.X, 1 - 2f * (@this.X * @this.X + @this.Y * @this.Y));
                euler.Z = (float) Math.Atan2(2f * @this.W * @this.Z + 2f * @this.X * @this.Y, 1 - 2f * (@this.Z * @this.Z + @this.X * @this.X));
            }

            // all the math so far has been done in radians. Before returning, we convert to degrees...
            euler *= 57.29578f;

            //...and then ensure the degree values are between 0 and 360
            euler.X %= 360;
            euler.Y %= 360;
            euler.Z %= 360;

            return euler;
        }
        
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