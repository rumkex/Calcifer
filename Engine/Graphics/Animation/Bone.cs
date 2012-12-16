using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Calcifer.Engine.Graphics.Primitives;

namespace Calcifer.Engine.Graphics.Animation
{
    public struct Bone
    {
        public int Timestamp;

        public int Parent;
        public int ID;
        public string Name;
        public Transform Transform;
        public Transform WorldTransform;

        public Bone(int parent, int id, string name)
        {
            Parent = parent;
            ID = id;
            Name = name;
            Transform = Transform.Identity;
            WorldTransform = Transform.Identity;
            Timestamp = 0;
        }

        public void GetMatrix(float[] buffer, int offset)
        {
            var matrix = WorldTransform.Matrix;
            buffer[offset + 0] = matrix.M11; buffer[offset + 1] = matrix.M12; buffer[offset + 2] = matrix.M13; buffer[offset + 3] = matrix.M14;
            buffer[offset + 4] = matrix.M21; buffer[offset + 5] = matrix.M22; buffer[offset + 6] = matrix.M23; buffer[offset + 7] = matrix.M24;
            buffer[offset + 8] = matrix.M31; buffer[offset + 9] = matrix.M32; buffer[offset + 10] = matrix.M33; buffer[offset + 11] = matrix.M34;
            buffer[offset + 12] = matrix.M41; buffer[offset + 13] = matrix.M42; buffer[offset + 14] = matrix.M43; buffer[offset + 15] = matrix.M44;
        }

        public void UpdateStamp()
        {
            unchecked { Timestamp++; }
        }
    }

    public class Pose : IEnumerable<Bone>
    {
        private Bone[] bones;

        public Pose(int count)
        {
            bones = new Bone[count];
	        for (int i = 0; i < count; i++)
	        {
				bones[i] = new Bone(-1, i, "") { WorldTransform = Transform.Identity };
	        }
        }

        public Pose(Pose self)
        {
            bones = self.bones.ToArray();
        }

        public Pose(IEnumerable<Bone> b)
        {
            bones = b.ToArray();
        }

        public int BoneCount
        {
            get { return bones.Length; }
        }

        public void CalculateWorld()
        {
            for (var index = 0; index < bones.Length; index++)
            {
                if (bones[index].Parent != -1 && bones[index].Timestamp != bones[bones[index].Parent].Timestamp)
                {
                    Transform.Multiply(ref bones[bones[index].Parent].WorldTransform, ref bones[index].Transform,
                                       out bones[index].WorldTransform);
                    bones[index].Timestamp = bones[bones[index].Parent].Timestamp;
                }
            }
        }

        public Bone this[int id]
        {
            get { return bones[id]; }
        }

        public void SetTransform(int id, Transform t)
        {
            bones[id].Transform = t;
            if (bones[id].Parent == -1) SetWorldTransform(id, t);
        }

        public void SetWorldTransform(int id, Transform t)
        {
            bones[id].WorldTransform = t;
            bones[id].UpdateStamp();
        }

        public void MergeWith(Pose other)
        {
            for (int i = 0; i < BoneCount; i++)
                Transform.Multiply(ref bones[i].WorldTransform, ref other.bones[i].WorldTransform, out bones[i].WorldTransform);
        }

        public IEnumerator<Bone> GetEnumerator()
        {
            return ((IEnumerable<Bone>) bones).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
