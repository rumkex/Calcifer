using System;
using System.Diagnostics;

namespace Calcifer.Engine.Graphics.Animation
{
    public sealed class LinearSequence : Sequence
    {
        private readonly AnimationData anim;
        private readonly float length;
        private readonly Pose pose;

        public LinearSequence(AnimationData anim, bool loop)
        {
            Loop = loop;
            this.anim = anim;
            Speed = 1.0f;
            pose = new Pose(anim.Frames[0]);
            length = anim.Frames.Count/anim.Speed;
        }

        public override float Time { get; set; }

        /// <summary>
        /// Animation speed factor
        /// </summary>
        public override float Speed { get; set; }

        public override float Length
        {
            get { return length; }
        }

        public override string Name
        {
            get { return anim.Name; }
        }

        public bool Loop { get; set; }

        public override bool Finished
        {
            get { return Time >= Length; }
        }

        public override Pose Pose
        {
            get { return pose; }
        }

        public override void Update(float timedelta)
        {
            Time += timedelta;
            if (Time > Length)
            {
                if (!Loop) return;
                Time %= Length;
            }
            var frame = Time*Speed*anim.Speed;
            var firstFrame = Math.Floor(frame);
            var secondFrame = Math.Ceiling(frame);
            var mixFactor = (float)((frame - firstFrame)/(secondFrame - firstFrame));
            var i1 = (int)(firstFrame - anim.Frames.Count * Math.Floor(firstFrame / anim.Frames.Count));
            var i2 = (int)(secondFrame - anim.Frames.Count * Math.Floor(secondFrame / anim.Frames.Count));
            if (i1 == i2) mixFactor = 1f;
            for (var i = 0; i < pose.BoneCount; i++)
            {
                var t = Transform.Interpolate(anim.Frames[i1][i].Transform, anim.Frames[i2][i].Transform, mixFactor);
                pose.SetTransform(i, t);
            }
            pose.CalculateWorld();
        }
    }
}