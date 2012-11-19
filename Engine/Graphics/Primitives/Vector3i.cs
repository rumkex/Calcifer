using System.Runtime.InteropServices;

namespace Calcifer.Engine.Graphics.Primitives
{
    [StructLayout(LayoutKind.Sequential)]
// ReSharper disable InconsistentNaming
    public struct Vector3i
    {
        public Vector3i(ushort x, ushort y, ushort z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3i(int[] data)
        {
            X = (ushort)data[0];
            Y = (ushort)data[1];
            Z = (ushort)data[2];
        }

        public static readonly int Size = Marshal.SizeOf(typeof(Vector3i));
        public ushort X;
        public ushort Y;
        public ushort Z;
    }
}
// ReSharper restore InconsistentNaming