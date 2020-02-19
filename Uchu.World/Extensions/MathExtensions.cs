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
            var unit = @this.X * @this.X + @this.Y * @this.Y + @this.Z * @this.Z + @this.W * @this.W;

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
                euler.Y = (float) Math.Atan2(2f * @this.W * @this.Y + 2f * @this.Z * @this.X,
                    1 - 2f * (@this.X * @this.X + @this.Y * @this.Y));
                euler.Z = (float) Math.Atan2(2f * @this.W * @this.Z + 2f * @this.X * @this.Y,
                    1 - 2f * (@this.Z * @this.Z + @this.X * @this.X));
            }

            // all the math so far has been done in radians. Before returning, we convert to degrees...
            euler *= 57.29578f;

            //...and then ensure the degree values are between 0 and 360
            euler.X %= 360;
            euler.Y %= 360;
            euler.Z %= 360;

            return euler;
        }

        public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta) =>
            MoveTowards(current, target, maxDistanceDelta, out _);
        
        public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta, out Vector3 changeVector)
        {
            //
            // Stolen from Unity.
            //

            var deltaX = target.X - current.X;
            var deltaY = target.Y - current.Y;
            var deltaZ = target.Z - current.Z;

            var delta = (float) (deltaX * (double) deltaX + deltaY * (double) deltaY + deltaZ * (double) deltaZ);

            if (Math.Abs(delta) < 0.001f || delta <= maxDistanceDelta * (double) maxDistanceDelta)
            {
                changeVector = Vector3.Zero;

                return target;
            }

            var change = (float) Math.Sqrt(delta);

            changeVector = new Vector3
            {
                X = deltaX / change * maxDistanceDelta,
                Y = deltaY / change * maxDistanceDelta,
                Z = deltaZ / change * maxDistanceDelta
            };
            
            return new Vector3(
                current.X + deltaX / change * maxDistanceDelta,
                current.Y + deltaY / change * maxDistanceDelta,
                current.Z + deltaZ / change * maxDistanceDelta
            );
        }

        public static Quaternion QuaternionLookRotation(this Vector3 forward, Vector3 up)
        {
            forward = Vector3.Normalize(forward);
 
            var vector = Vector3.Normalize(forward);
            var vector2 = Vector3.Normalize(Vector3.Cross(up, vector));
            var vector3 = Vector3.Cross(vector, vector2);
            var m00 = vector2.X;
            var m01 = vector2.Y;
            var m02 = vector2.Z;
            var m10 = vector3.X;
            var m11 = vector3.Y;
            var m12 = vector3.Z;
            var m20 = vector.X;
            var m21 = vector.Y;
            var m22 = vector.Z;
 
 
            var num8 = m00 + m11 + m22;
            var quaternion = new Quaternion();
            if (num8 > 0f)
            {
                var num = (float)Math.Sqrt(num8 + 1f);
                quaternion.W = num * 0.5f;
                num = 0.5f / num;
                quaternion.X = (m12 - m21) * num;
                quaternion.Y = (m20 - m02) * num;
                quaternion.Z = (m01 - m10) * num;
                return quaternion;
            }
            if (m00 >= m11 && m00 >= m22)
            {
                var num7 = (float)Math.Sqrt(1f + m00 - m11 - m22);
                var num4 = 0.5f / num7;
                quaternion.X = 0.5f * num7;
                quaternion.Y = (m01 + m10) * num4;
                quaternion.Z = (m02 + m20) * num4;
                quaternion.W = (m12 - m21) * num4;
                return quaternion;
            }
            if (m11 > m22)
            {
                var num6 = (float)Math.Sqrt(1f + m11 - m00 - m22);
                var num3 = 0.5f / num6;
                quaternion.X = (m10+ m01) * num3;
                quaternion.Y = 0.5f * num6;
                quaternion.Z = (m21 + m12) * num3;
                quaternion.W = (m20 - m02) * num3;
                return quaternion; 
            }
            var num5 = (float)Math.Sqrt(1f + m22 - m00 - m11);
            var num2 = 0.5f / num5;
            quaternion.X = (m20 + m02) * num2;
            quaternion.Y = (m21 + m12) * num2;
            quaternion.Z = 0.5f * num5;
            quaternion.Y = (m01 - m10) * num2;
            return quaternion;
        }
        
        public static Vector3 VectorMultiply(this Quaternion rotation, Vector3 point)
        {
            var num1 = rotation.X * 2f;
            var num2 = rotation.Y * 2f;
            var num3 = rotation.Z * 2f;
            var num4 = rotation.X * num1;
            var num5 = rotation.Y * num2;
            var num6 = rotation.Z * num3;
            var num7 = rotation.X * num2;
            var num8 = rotation.X * num3;
            var num9 = rotation.Y * num3;
            var num10 = rotation.W * num1;
            var num11 = rotation.W * num2;
            var num12 = rotation.W * num3;
            Vector3 vector3;
            vector3.X = (float) ((1.0 - (num5 + (double) num6)) * point.X + (num7 - (double) num12) * point.Y + (num8 + (double) num11) * point.Z);
            vector3.Y = (float) ((num7 + (double) num12) * point.X + (1.0 - (num4 + (double) num6)) * point.Y + (num9 - (double) num10) * point.Z);
            vector3.Z = (float) ((num8 - (double) num11) * point.X + (num9 + (double) num10) * point.Y + (1.0 - (num4 + (double) num5)) * point.Z);
            return vector3;
        }
    }
}