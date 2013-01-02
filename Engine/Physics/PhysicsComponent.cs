﻿using System;
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

		public PhysicsComponent()
		{
			Body = new RigidBody(new SphereShape(0.0f))
			{
				IsStatic = true
			};
		}

        public PhysicsComponent(Shape shape, bool isStatic)
        {
            Body = new RigidBody(shape)
                       {
                           IsStatic = isStatic
                       };
        }

        public RigidBody Body { get; private set; }

        public World World { get; set; }

	    protected override void OnAdded(ComponentStateEventArgs registrationArgs)
        {
            base.OnAdded(registrationArgs);
            Body.Orientation = JMatrix.CreateFromQuaternion(transform.Rotation.ToQuaternion());
            Body.Position = transform.Translation.ToJVector();
            transform.Bind(TransformFeedback);
        }

        private ScalableTransform TransformFeedback()
        {
            return new ScalableTransform(JQuaternion.CreateFromMatrix(Body.Orientation).ToQuaternion(),
                                         Body.Position.ToVector3());
        }
    }
}