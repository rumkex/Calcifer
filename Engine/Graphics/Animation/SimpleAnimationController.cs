namespace Calcifer.Engine.Graphics.Animation
{
    public class SimpleAnimationController : AnimationComponent
	{
		private Sequence seq;
		public override string Name
		{
			get
			{
				return this.seq.Name;
			}
		}
		public override Pose Pose
		{
			get
			{
				return this.seq.Pose;
			}
		}
		public SimpleAnimationController(AnimationData anim)
		{
			this.seq = new LinearSequence(anim, true);
		}
		public override void Update(double t)
		{
			this.seq.Update((float)t);
		}
	}
}
