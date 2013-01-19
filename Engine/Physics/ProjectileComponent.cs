using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calcifer.Utilities.Logging;
using ComponentKit.Model;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
    // Defines physics behaviour as following:
    // * Body collides only with static geometry
    // * Component detects when body is passing through certain non-static objects
    // * Component raises Hit event.
    class ProjectileComponent: DependencyComponent
    {
        private RigidBody owner;
        [RequireComponent] private PhysicsComponent physics;

        public event EventHandler<SensorEventArgs> EntityHit;
		
		public ProjectileComponent()
		{
		    postStepHandler = PostStep;
			syncHandler = Synchronized;
		}

		private World.WorldStep postStepHandler;
		private EventHandler<ComponentStateEventArgs> syncHandler;

		protected override void OnAdded(ComponentStateEventArgs registrationArgs)
		{
			base.OnAdded(registrationArgs);
			if (physics.Body.Shape is Multishape) throw new NotSupportedException("Multishapes not supported!");
            physics.Body.BroadphaseTag |= (int)BodyTags.Ghost;
            physics.Body.BroadphaseTag |= (int)BodyTags.Projectile;
			physics.Synchronized += syncHandler;
		}

		protected override void OnRemoved(ComponentStateEventArgs registrationArgs)
        {
            physics.Body.BroadphaseTag ^= (int)BodyTags.Ghost;
            physics.Body.BroadphaseTag ^= (int)BodyTags.Projectile;
            physics.Synchronized -= syncHandler;
            if (postStepHandler != null) physics.World.Events.PostStep -= postStepHandler;
			base.OnRemoved(registrationArgs);
		}

		private void Synchronized(object sender, ComponentStateEventArgs componentStateEventArgs)
		{
            if (IsOutOfSync) return;
            physics.World.Events.PostStep += postStepHandler;
		    foreach (RigidBody body in physics.World.RigidBodies)
		    {
		        var orientation = physics.Body.Orientation;
		        var position = physics.Body.Position;
		        var bodyPosition = body.Position;
		        var bodyOrientation = body.Orientation;
		        JVector point, normal;
		        float penetration;
		        var collide = XenoCollide.Detect(physics.Body.Shape, body.Shape, ref orientation, ref bodyOrientation,
		                                         ref position, ref bodyPosition, out point, out normal, out penetration);
		        if (collide 
                    && (body.BroadphaseTag == 0) // Projectiles and ghosts can't be projectile owners
                    && !(body.Shape is Multishape)) // Multishapes aren't supported
		        {
                    owner = body;
                    return;
		        }
		    }
		}

        private void PostStep(float timeStep)
        {
            if (owner != null)
            {
                var collide = physics.World.CollisionSystem.CheckBoundingBoxes(physics.Body, owner); 
                if (collide) return;
                var orientation = physics.Body.Orientation;
                var position = physics.Body.Position;
                var ownerPosition = owner.Position;
                var ownerOrientation = owner.Orientation;
                JVector point, normal;
                float penetration;
                collide = XenoCollide.Detect(physics.Body.Shape, owner.Shape, ref orientation, ref ownerOrientation,
                                                ref position, ref ownerPosition, out point, out normal, out penetration);
                if (collide) return;
            }
            // Left the owner, deghostifying
            if (owner != null)
                Log.WriteLine(LogLevel.Debug, "Projectile {0} left its owner, {1}", Record.Name, owner.Tag);
            physics.Body.BroadphaseTag ^= (int)BodyTags.Ghost;
            physics.World.Events.PostStep -= postStepHandler;
            postStepHandler = null;
        }
    }
}
