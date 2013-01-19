using System;
using System.Globalization;
using System.Linq;
using Jitter.LinearMath;
using OpenTK;

namespace Calcifer.Utilities
{
    static class MathUtils
    {
        public static float Clamp(float value, float min, float max)
        {
            return Math.Max(Math.Min(value, max), min);
        }

        public static float Project(Vector3 point, Vector3 normal, Vector3 origin)
        {
            var d = point - origin;
            return Vector3.Dot(d, normal);
        }

        public static Vector3 MinVector(Vector3 a, Vector3 b)
        {
            var x = Math.Min(a.X, b.X);
            var y = Math.Min(a.Y, b.Y);
            var z = Math.Min(a.Z, b.Z);

            return new Vector3(x, y, z);
        }

        public static Vector3 MaxVector(Vector3 a, Vector3 b)
        {
            var x = Math.Max(a.X, b.X);
            var y = Math.Max(a.Y, b.Y);
            var z = Math.Max(a.Z, b.Z);

            return new Vector3(x, y, z);
        }

        public static Matrix4 FromQuaternion(Quaternion q)
        {
            return new Matrix4(
                1 - 2 * q.Y * q.Y - 2 * q.Z * q.Z, 2 * q.X * q.Y - 2 * q.Z * q.W, 2 * q.X * q.Z + 2 * q.Y * q.W, 0,
                2 * q.X * q.Y + 2 * q.Z * q.W, 1 - 2 * q.X * q.X - 2 * q.Z * q.Z, 2 * q.Y * q.Z - 2 * q.X * q.W, 0,
                2 * q.X * q.Z - 2 * q.Y * q.W, 2 * q.Y * q.Z + 2 * q.X * q.W, 1 - 2 * q.X * q.X - 2 * q.Y * q.Y, 0,
                0, 0, 0, 1
            );
        }

        public static Quaternion FromMatrix(Matrix4 m)
        {
            var w = (float)Math.Sqrt(1.0f + m.M11 + m.M22 + m.M33) / 2.0f;
            var w4 = (4.0f * w);
            var x = (m.M32 - m.M23) / w4;
            var y = (m.M13 - m.M31) / w4;
            var z = (m.M21 - m.M12) / w4;
            return new Quaternion(x, y, z, w);
        }

        public static Quaternion CreateRotation(Vector3 v1, Vector3 v2)
        {
            return Quaternion.Normalize(new Quaternion(Vector3.Cross(v1, v2), (float)Math.Sqrt(v1.LengthSquared * v2.LengthSquared) + Vector3.Dot(v1, v2)));
        }
    }

    public static class ExtensionMethods
    {
        public static string ConvertToString(this Vector3 v)
        {
            return v.X.ToString(CultureInfo.InvariantCulture) + ";"
                + v.Y.ToString(CultureInfo.InvariantCulture) + ";"
                + v.Z.ToString(CultureInfo.InvariantCulture);
        }

        public static Vector3 ConvertToVector(this string s)
        {
            var coords = s.Split(';').Select(t => float.Parse(t, CultureInfo.InvariantCulture)).ToList();
            return new Vector3(coords[0], coords[1], coords[2]);
        }

        public static Vector3 ToVector3(this JVector v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static JVector ToJVector(this Vector3 v)
        {
            return new JVector(v.X, v.Y, v.Z);
        }

        public static JQuaternion ToQuaternion(this Quaternion v)
        {
            return new JQuaternion(v.X, v.Y, v.Z, v.W);
        }
        
        public static Quaternion ToQuaternion(this JQuaternion v)
        {
            return new Quaternion(v.X, v.Y, v.Z, v.W);
        }

        public static Vector3 ToEuler(this Quaternion q)
        {
            var euler = new Vector3();

            // Z needs to be the rotation along Z axis, minus pi/2 for some reason
            var newX = Vector3.Transform(Vector3.UnitX, q);
            var cos = Vector3.Dot(Vector3.UnitX, newX);
            var sin = -Vector3.Cross(Vector3.UnitX, newX).Z;
            euler.Z = (float) Math.Atan2(cos, sin) + MathHelper.PiOver2;
            return euler;
        }
    }
}
