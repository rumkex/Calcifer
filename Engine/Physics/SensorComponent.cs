using System;
using System.Collections.Generic;
using ComponentKit.Model;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
	public class SensorComponent: DependencyComponent
	{
		[RequireComponent] private PhysicsComponent physics;

        public event EventHandler<SensorEventArgs> TriggerEntered;
		public event EventHandler<SensorEventArgs> TriggerExited;
		public HashSet<RigidBody> CollidingBodies { get; private set; }
		
		public SensorComponent()
		{
			CollidingBodies = new HashSet<RigidBody>();
			postStepHandler = PostStep;
		}

		private World.WorldStep postStepHandler;
		private EventHandler<EventArgs> syncHandler;
		
		protected override void OnAdded(ComponentStateEventArgs registrationArgs)
		{
			base.OnAdded(registrationArgs);
			syncHandler = Syncronization;
			Record.Registry.Synchronized += syncHandler;
			if (physics.Body.Shape is Multishape) throw new NotSupportedException("Multishapes not supported!");
			physics.Body.BroadphaseTag |= (int)BodyTags.Ghost;
		}

		protected override void OnRemoved(ComponentStateEventArgs registrationArgs)
		{
			base.OnRemoved(registrationArgs);
			physics.Body.BroadphaseTag ^= (int)BodyTags.Ghost;
			Record.Registry.Synchronized -= syncHandler;
		}

		private void Syncronization(object sender, EventArgs e)
		{
			if (IsOutOfSync)
			{
				physics.World.Events.PostStep -= postStepHandler;
			}
			else
			{
				physics.World.Events.PostStep += postStepHandler;
			}
		}

		private void PostStep(float timeStep)
        {
            foreach (RigidBody body in physics.World.RigidBodies)
            {
                if (body.Shape is Multishape) continue; // no multishape support!

	            if (body.IsStaticOrInactive) continue; // not interested in static bodies like terrain, and inactive objects don't change their trigger state
				var collide = physics.World.CollisionSystem.CheckBoundingBoxes(physics.Body, body);
	            if (collide)
				{
					var otherPosition = body.Position;
					var otherOrientation = body.Orientation;
					var sensorOrientation = physics.Body.Orientation;
					var sensorPosition = physics.Body.Position;
					JVector point, normal;
					float penetration;
					collide = XenoCollide.Detect(physics.Body.Shape, body.Shape, ref sensorOrientation, ref otherOrientation,
					ref sensorPosition, ref otherPosition, out point, out normal, out penetration);
					if (collide && !CollidingBodies.Contains(body))
					{
						// okay, we detected a collision, but it's not already in the list!
						CollidingBodies.Add(body);
						if (TriggerEntered != null) TriggerEntered(this, new SensorEventArgs(body));
					}
	            }
				if (!collide && CollidingBodies.Contains(body))
				{
					// okay, we detected no collisions, but is in the list!
					CollidingBodies.Remove(body);
					if (TriggerExited != null) TriggerExited(this, new SensorEventArgs(body));
				}

            }
        }
	}

	public class SensorEventArgs : EventArgs
	{
		public RigidBody Body { get; private set; }

		public SensorEventArgs(RigidBody body)
		{
			Body = body;
		}
	}
}
