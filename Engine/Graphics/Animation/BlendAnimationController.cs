using System;
using System.Collections.Generic;
using Calcifer.Engine.Graphics.Primitives;

namespace Calcifer.Engine.Graphics.Animation
{
    public class BlendAnimationController : AnimationComponent
    {
        private Pose pose;
        private Pose restPose;
        private Dictionary<string, AnimationData> anims;
        private Sequence current;
        private Sequence backup;
        private float fadeTime;
        private float fadeLeft;
        public override string Name
        {
            get
            {
                return current != null ? current.Name : "";
            }
        }
        public float Speed
        {
            get
            {
                return current != null ? current.Speed : 0;
            }
        }
        public float Length
        {
            get
            {
                return current != null ? current.Length: 0;
            }
        }
        public float Time
        {
            get
            {
                return current != null ? current.Time: 0;
            }
        }
        public override Pose Pose
        {
            get
            {
                return this.pose;
            }
        }
        public BlendAnimationController(Pose rest)
        {
            this.restPose = rest;
            this.pose = new Pose(rest.BoneCount);
            this.anims = new Dictionary<string, AnimationData>();
        }
        public void AddAnimation(AnimationData anim)
        {
            this.anims.Add(anim.Name, anim);
        }
        public void Start(string name, bool loop)
        {
            this.current = new LinearSequence(this.anims[name], loop);
        }
        public void Crossfade(string name, float time, bool loop)
        {
            LinearSequence linearSequence = new LinearSequence(this.anims[name], loop);
            this.backup = linearSequence;
            this.fadeLeft = time;
            this.fadeTime = time;
        }
        public override void Update(double time)
        {
            if (this.fadeLeft > 0f)
            {
                this.fadeLeft = Math.Max(this.fadeLeft - (float)time, 0f);
            }
            if (this.current == null)
            {
                return;
            }
            this.current.Update((float)time);
            if (this.backup != null)
            {
                this.backup.Update((float)time);
            }
            this.pose = new Pose(this.current.Pose);
            for (int i = 0; i < this.pose.BoneCount; i++)
            {
                if (this.backup != null && this.fadeLeft > 0f)
                {
                    this.pose.SetTransform(i, Transform.Interpolate(this.current.Pose[i].Transform, this.backup.Pose[i].Transform, 1f - this.fadeLeft / this.fadeTime));
                }
            }
            this.pose.CalculateWorld();
            if (this.backup != null && this.fadeLeft <= 0.01f)
            {
                this.current = this.backup;
                this.backup = null;
            }
            this.pose.MergeWith(this.restPose);
        }
    }
}