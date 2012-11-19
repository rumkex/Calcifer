using Calcifer.Engine.Graphics;
using Calcifer.Engine.Graphics.Animation;

namespace Calcifer.Engine.Scenegraph
{
    public class AnimationNode: SceneNode
    {
        private AnimationComponent animation;

        public Pose Pose
        {
            get { return animation.Pose; }
        }

        public AnimationNode(SceneNode parent, AnimationComponent animation): base(parent)
        {
            this.animation = animation;
        }

        public override void AcceptPass(RenderPass pass)
        {
            pass.Visit(this);
        }
    }
}