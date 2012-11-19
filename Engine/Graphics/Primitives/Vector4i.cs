using System.Runtime.InteropServices;

namespace Calcifer.Engine.Graphics.Primitives
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4i
    {
        public Vector4i(ushort x, ushort y, ushort z, ushort w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4i(int[] data)
        {
            X = (ushort)data[0];
            Y = (ushort)data[1];
            Z = (ushort)data[2];
            W = (ushort)data[3];
        }

        public static readonly int Size = Marshal.SizeOf(typeof(Vector4i));
        public ushort X;
        public ushort Y;
        public ushort Z;
        public ushort W;

    }
}