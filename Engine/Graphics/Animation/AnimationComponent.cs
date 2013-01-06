using Calcifer.Engine.Components;
using ComponentKit.Model;

namespace Calcifer.Engine.Graphics.Animation
{
    public class AnimationComponent: Component, IUpdateable
    {
        public virtual Pose Pose
        {
	        get { throw new System.NotImplementedException(); }
        }

	    public virtual void Update(double time)
        {
	        throw new System.NotImplementedException();
        }

	    public virtual string Name
	    {
		    get { throw new System.NotImplementedException(); }
	    }
    }
}
