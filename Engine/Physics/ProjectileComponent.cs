using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calcifer.Engine.Scripting;
using Calcifer.Utilities.Logging;
using ComponentKit.Model;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;

namespace Calcifer.Engine.Physics
{
    // Defines physics behaviour as following:
    // * Body collides only with static geometry
    // * Component detects when body is passing through certain non-static objects
    // * Component raises Hit event.
    class ProjectileComponent: DependencyComponent
    {
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
            physics.Body.BroadphaseTag |= (int)BodyTags.Projectile;
            physics.Body.SetMassProperties(JMatrix.Identity, 0.1f, false);
			physics.Synchronized += syncHandler;
		}

		protected override void OnRemoved(ComponentStateEventArgs registrationArgs)
        {
            physics.Body.BroadphaseTag &= ~(int)BodyTags.Projectile;
            physics.Synchronized -= syncHandler;
            physics.World.Events.PostStep -= postStepHandler;
			base.OnRemoved(registrationArgs);
		}

		private void Synchronized(object sender, ComponentStateEventArgs componentStateEventArgs)
		{
            if (IsOutOfSync) return;
            physics.World.Events.PostStep += postStepHandler;
		}

        private void PostStep(float timeStep)
        {
            foreach (Arbiter arbiter in physics.Body.Arbiters)
            {
                var other = (arbiter.Body1 != physics.Body) ? arbiter.Body1 : arbiter.Body2;
                var dir = JVector.Normalize(other.Position - physics.Body.Position);
                var vel = JVector.Normalize(physics.Body.LinearVelocity);
                if (JVector.Dot(vel, dir) > -0.5f) // Hit only if the projectile is moving towards the body
                { 
                    var entity = Entity.Find(other.Tag.ToString());
                    if (entity == null) continue;
                    //if (entity.Name == "heroe") continue;
                    if (!entity.HasComponent<HealthComponent>()) continue;
                    if (!entity.HasComponent<LuaComponent>()) continue;
                    // The same ugly hax that is used in LSA
                    // TODO: Fix ugly hax
                    entity.GetComponent<LuaComponent>().Service.SetWounded(entity.Name, true);
                }
            }
        }
    }
}
