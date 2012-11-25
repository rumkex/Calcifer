using System;
using Calcifer.Engine.Graphics.Primitives;

namespace Calcifer.Engine.Graphics.Animation
{
    public class LinearSequence : Sequence
    {
        private AnimationData anim;
        private Pose pose;
        private float time;
        private float length;
        public override float Time
        {
            get
            {
                return this.time;
            }
        }
        public override float Speed
        {
            get
            {
                return this.anim.Speed;
            }
        }
        public override float Length
        {
            get
            {
                return this.length;
            }
        }
        public override string Name
        {
            get
            {
                return this.anim.Name;
            }
        }
        public bool Loop
        {
            get;
            set;
        }
        public override bool Finished
        {
            get
            {
                return this.Time > this.Length;
            }
        }
        public override Pose Pose
        {
            get
            {
                return this.pose;
            }
        }
        public LinearSequence(AnimationData anim, bool loop)
        {
            this.Loop = loop;
            this.anim = anim;
            this.pose = new Pose(anim.Frames[0]);
            this.length = (float)anim.Frames.Count / anim.Speed;
        }
        public override void Update(float timedelta)
        {
            this.time += timedelta;
            if (this.time > this.Length)
            {
                if (!this.Loop)
                {
                    return;
                }
                this.time %= this.Length;
            }
            float num = this.Time * this.Speed;
            double num2 = Math.Floor((double)num);
            double num3 = Math.Ceiling((double)num);
            double num4 = ((double)num - num2) / (num3 - num2);
            int index = (int)num2 % this.anim.Frames.Count;
            int index2 = (int)num3 % this.anim.Frames.Count;
            for (int i = 0; i < this.pose.BoneCount; i++)
            {
                Transform t = Transform.Interpolate(this.anim.Frames[index][i].Transform, this.anim.Frames[index2][i].Transform, (float)num4);
                this.pose.SetTransform(i, t);
            }
            this.pose.CalculateWorld();
        }
    }
}