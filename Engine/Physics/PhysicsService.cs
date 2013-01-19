using System;
using System.Collections.Generic;
using System.Linq;
using Calcifer.Engine.Components;
using Calcifer.Utilities.Logging;
using ComponentKit;
using ComponentKit.Model;
using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
    public class PhysicsService : IUpdateable, IService
    {
        private readonly ComponentSyncTriggerPredicate trigger;

        public World World { get; private set; }

        public PhysicsService()
        {
            trigger = c => c is PhysicsComponent;
            World = new World(new CollisionSystemSAP{UseTriangleMeshNormal = false})
	                    {
							Gravity = new JVector(0, 0, -9.81f)
	                    };
			World.ContactSettings.MaterialCoefficientMixing = ContactSettings.MaterialCoefficientMixingType.TakeMinimum;
	        World.CollisionSystem.PassedBroadphase += CheckBroadphase;
        }

        private bool CheckBroadphase(IBroadphaseEntity e1, IBroadphaseEntity e2)
        {
            // Ghost objects have "Ghost" tag, so we skip them during the broadphase
            var tag1 = (BodyTags) (e1.BroadphaseTag);
            var tag2 = (BodyTags) (e2.BroadphaseTag);
            if ((tag1 & tag2 & BodyTags.Projectile) == BodyTags.Projectile) return false;
            return ((tag1 | tag2) & BodyTags.Ghost) == BodyTags.None;
        }

        public void Synchronize(IEnumerable<IComponent> components)
        {
            foreach (var c in components.OfType<PhysicsComponent>())
            {
                if (c.IsOutOfSync)
                {
					c.World = null;
					World.RemoveBody(c.Body);
                }
                else
                {
					c.World = World;
	                c.Body.EnableDebugDraw = true;
					World.AddBody(c.Body);
				}
				c.Synchronize();
			}
        }

        public void Update(double dt)
        {
            World.Step((float) dt, false);
        }
    }
}