using System;
using System.Collections.Generic;
using System.IO;
using Calcifer.Engine.Components;
using Calcifer.Engine.Content;
using Calcifer.Engine.Scenery;

namespace Calcifer.Engine.Graphics.Animation
{
    public class BlendAnimationController : AnimationComponent, ISaveable, IConstructable
    {
        private readonly Dictionary<string, AnimationData> anims;
        private Pose invRestPose;
        private LinearSequence backup;
        private LinearSequence current;
        private float fadeLeft;
        private float fadeTime;
        private Pose pose;

        public BlendAnimationController()
        {
            anims = new Dictionary<string, AnimationData>();
        }

        public override string Name
        {
            get { return current != null ? current.Name : ""; }
        }

        public float Speed
        {
            get { return current != null ? current.Speed : 0; }
            set
            {
                if (current != null) current.Speed = value;
                if (backup != null) backup.Speed = value;
            }
        }

        public float Length
        {
            get { return current != null ? current.Length : 0; }
        }

        public float Time
        {
            get { return current != null ? current.Time : 0; }
        }

        public override Pose Pose
        {
            get { return pose; }
        }

        public void SaveState(BinaryWriter writer)
        {
            writer.Write(current == null ? "" : current.Name);
            if (current != null)
            {
                writer.Write(current.Loop);
                writer.Write(current.Speed);
                writer.Write(current.Time);
            } 
            writer.Write(backup == null ? "" : backup.Name);
            if (backup != null)
            {
                writer.Write(backup.Loop);
                writer.Write(backup.Speed);
                writer.Write(backup.Time);
            }
            writer.Write(fadeLeft);
            writer.Write(fadeTime);
        }

        public void RestoreState(BinaryReader reader)
        {
            var curName = reader.ReadString();
            if (curName != "")
            {
                var loop = reader.ReadBoolean();
                current = new LinearSequence(anims[curName], loop)
                              {
                                  Speed = reader.ReadSingle(),
                                  Time = reader.ReadSingle()
                              };
            }
            var backupName = reader.ReadString();
            if (backupName != "")
            {
                var loop = reader.ReadBoolean();
                backup = new LinearSequence(anims[backupName], loop)
                {
                    Speed = reader.ReadSingle(),
                    Time = reader.ReadSingle()
                };
            }
            fadeLeft = reader.ReadSingle();
            fadeTime = reader.ReadSingle();
        }

        public void AddAnimation(AnimationData anim)
        {
            anims.Add(anim.Name, anim);
        }

        public void Start(string name, bool loop)
        {
            current = new LinearSequence(anims[name], loop);
        }

        public void Crossfade(string name, float time, bool loop)
        {
            if (current != null)
            {
                var linearSequence = new LinearSequence(anims[name], loop);
                backup = linearSequence;
                fadeLeft = time;
                fadeTime = time;
            }
            else Start(name, loop);
        }

        public override void Update(double time)
        {
            if (fadeLeft > 0f)
            {
                fadeLeft = Math.Max(fadeLeft - (float) time, 0f);
            }
            if (current == null)
            {
                return;
            }
            current.Update((float) time);
            if (backup != null)
            {
                backup.Update((float) time);
            }
            pose = new Pose(current.Pose);

            if (backup != null && fadeLeft > 0f)
                for (int i = 0; i < pose.BoneCount; i++)
                    pose.SetTransform(i,
                                      Transform.Interpolate(current.Pose[i].Transform, backup.Pose[i].Transform,
                                                            1f - fadeLeft/fadeTime));
            pose.CalculateWorld();
            if (backup != null && fadeLeft <= 0.01f)
            {
                current = backup;
                backup = null;
            }
            pose.MergeWith(invRestPose);
        }

        void IConstructable.Construct(IDictionary<string, string> param)
        {
            invRestPose = new Pose(ResourceFactory.LoadAsset<AnimationData>(param["restPose"]).Frames[0]);
            invRestPose.Invert();
            pose = new Pose(invRestPose.BoneCount);
            if (param["animations"] != null)
                foreach (var animName in param["animations"].Split(';'))
                    AddAnimation(ResourceFactory.LoadAsset<AnimationData>(animName));
        }
    }
}