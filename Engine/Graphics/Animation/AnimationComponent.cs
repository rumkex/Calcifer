using Calcifer.Engine.Components;
using ComponentKit.Model;

namespace Calcifer.Engine.Graphics.Animation
{
    public abstract class AnimationComponent: Component, IUpdateable
    {
        public abstract Pose Pose { get; }
        public abstract void Update(double time);
        public abstract string Name { get; }
    }
}
