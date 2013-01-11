using System;
using Calcifer.Engine.Components;
using Calcifer.Engine.Graphics.Primitives;
using Calcifer.Utilities;
using ComponentKit;
using ComponentKit.Model;
using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
	[Flags]
	public enum BodyTags
	{
		None = 0,
		Ghost = 1,
	}

	public class PhysicsComponent : DependencyComponent
    {
        [RequireComponent] private TransformComponent transform = null;

		private Transform baseTransform = Transform.Identity;

		public event EventHandler<ComponentStateEventArgs> Synchronized;

		public PhysicsComponent()
		{
			Body = new RigidBody(new SphereShape(0.0f))
			{
				IsStatic = true
			};
		}

        public PhysicsComponent(RigidBody body)
        {
	        Body = body;
        }

        public RigidBody Body { get; private set; }

        public World World { get; set; }

	    protected override void OnAdded(ComponentStateEventArgs registrationArgs)
        {
            base.OnAdded(registrationArgs);
			Body.Tag = Record.Name;
		    Body.Position += transform.Translation.ToJVector();
			baseTransform = new Transform(JQuaternion.CreateFromMatrix(Body.Orientation).ToQuaternion(),
										 Body.Position.ToVector3()).Invert() * new Transform(transform.Rotation, transform.Translation);
            transform.Bind(TransformFeedback);
        }

        private ScalableTransform TransformFeedback()
        {
			var current = new Transform(JQuaternion.CreateFromMatrix(Body.Orientation).ToQuaternion(),
										 Body.Position.ToVector3());
	        var t = current*baseTransform;
			return new ScalableTransform(t.Rotation, t.Translation);
        }

        public bool CollidesWith(RigidBody other)
        {
            return Record.HasComponent<SensorComponent>() ? 
                Record.GetComponent<SensorComponent>().CollidingBodies.Contains(other) : 
                Body.CollisionIsland.Bodies.Contains(other);
        }

		public void Synchronize()
		{
			if (Synchronized != null) Synchronized(this, new ComponentStateEventArgs(Record));
		}
    }
}