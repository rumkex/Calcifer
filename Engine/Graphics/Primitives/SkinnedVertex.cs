using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;

namespace Calcifer.Engine.Graphics.Primitives
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SkinnedVertex
    {
        public SkinnedVertex(Vector3 pos, Vector3 normal, Vector2 uv)
        {
            Position = pos;
            Normal = normal;
            Texture = uv;
            Weights = Vector4.Zero;
            Bones = Vector4.Zero;
        }

        public SkinnedVertex(Vector3 pos, Vector3 normal, Vector2 uv, IList<int> bones, IList<float> weights): this(pos, normal, uv)
        {
            if (weights.Count > 0)
            {
                Weights.X = weights[0];
                Bones.X = bones[0];
            }
            if (weights.Count > 1)
            {
                Weights.Y = weights[1];
                Bones.Y = bones[1];
            }
            if (weights.Count > 2)
            {
                Weights.Z = weights[2];
                Bones.Z = bones[2];
            }
            if (weights.Count > 3)
            {
                Weights.W = weights[3];
                Bones.W = bones[3];
            }
        }

        public static readonly int Size = Marshal.SizeOf(typeof(SkinnedVertex));
        //[FieldOffset(0)]
        public Vector3 Position;
        //[FieldOffset(12)]
        public Vector3 Normal;
        //[FieldOffset(24)]
        public Vector2 Texture;
        //[FieldOffset(32)]
        public Vector4 Weights;
        //[FieldOffset(48)]
        public Vector4 Bones;
    }
}