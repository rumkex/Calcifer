using System.Collections.Generic;
using Calcifer.Engine.Content;
using Calcifer.Engine.Scenery;

namespace Calcifer.Engine.Graphics.Animation
{
    public class SimpleAnimationController : AnimationComponent, IConstructable
    {
        private Sequence seq;

        public override string Name
        {
            get { return seq.Name; }
        }

        private Pose pose, invRest;
        public override Pose Pose
        {
            get { return pose; }
        }

        void IConstructable.Construct(IDictionary<string, string> param)
        {
            var anim = ResourceFactory.LoadAsset<AnimationData>(param["animData"]);
            seq = new LinearSequence(anim, true);
            var rest = ResourceFactory.LoadAsset<AnimationData>(param["restPose"]).Frames[0];
            invRest = new Pose(rest);
            pose = new Pose(rest);
            invRest.Invert();
        }

        public override void Update(double t)
        {
            seq.Update((float) t);
            pose = new Pose(seq.Pose);
            pose.MergeWith(invRest);
        }
    }
}