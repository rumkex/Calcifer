using OpenTK;

namespace Calcifer.Engine.Primitives
{
    public struct ScalableTransform
    {
        public Quaternion Rotation;
        public Vector3 Translation;
        public Vector3 Scale;

        public ScalableTransform(Quaternion rotation, Vector3 translation)
            : this(rotation, translation, Vector3.One)
        {
        }

        public ScalableTransform(Quaternion rotation, Vector3 translation, Vector3 scale)
        {
            Rotation = rotation;
            Translation = translation;
            Scale = scale;
        }

        public Matrix4 Matrix
        {
            get { return Matrix4.Scale(Scale) * Matrix4.Rotate(Rotation) * Matrix4.CreateTranslation(Translation); }
        }
    }
}
