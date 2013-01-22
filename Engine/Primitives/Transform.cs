using OpenTK;

namespace Calcifer.Engine.Primitives
{
    public struct Transform
    {
        public Quaternion Rotation;
        public Vector3 Translation;

        public Transform(Quaternion q, Vector3 t)
        {
            Rotation = q;
            Translation = t;
        }

        public Matrix4 Matrix 
        { 
            get
            {
                var mat = Matrix4.Rotate(Rotation) * Matrix4.CreateTranslation(Translation);
                return mat;
            }
        }

        public void TransformVector(ref Vector3 src, out Vector3 dest)
        {
            Vector3.Transform(ref src, ref Rotation, out dest);
            Vector3.Add(ref dest, ref Translation, out dest);
        }

        public Transform Invert()
        {
            Quaternion q;
            Vector3 v;
            Quaternion.Conjugate(ref Rotation, out q);
            Vector3.Transform(ref Translation, ref q, out v);
            return new Transform(q, -v);
        }

        public override string ToString()
        {
            return Matrix.ToString();
        }

        public static Transform Identity
        {
            get { return new Transform(Quaternion.Identity, Vector3.Zero); }
        }

        public static void Invert(ref Transform t)
        {
            Vector3 v;
            Quaternion.Conjugate(ref t.Rotation, out t.Rotation);
            Vector3.Transform(ref t.Translation, ref t.Rotation, out v);
            t.Translation = -v;
        }


        public static Transform Interpolate(Transform t1, Transform t2, float blendFactor)
        {
            return new Transform(
                Quaternion.Slerp(t1.Rotation, t2.Rotation, blendFactor),
                Vector3.Lerp(t1.Translation, t2.Translation, blendFactor)
                );
        }

        public static Vector3 operator *(Transform t, Vector3 v)
        {
            Vector3 res;
            Vector3.Transform(ref v, ref t.Rotation, out res);
            Vector3.Add(ref res, ref t.Translation, out res);
            return res;
        }

        public static Transform operator *(Transform t1, Transform t2)
        {
            Transform t;
            Multiply(ref t1, ref t2, out t);
            return t;
        }

        public static void Multiply(ref Transform t1, ref Transform t2, out Transform t)
        {
            Vector3 temp;
            Vector3.Transform(ref t2.Translation, ref t1.Rotation, out temp);
            Vector3.Add(ref temp, ref t1.Translation, out t.Translation);
            Quaternion.Multiply(ref t1.Rotation, ref t2.Rotation, out t.Rotation);
            Quaternion.Normalize(ref t.Rotation, out t.Rotation);
        }
    }
}