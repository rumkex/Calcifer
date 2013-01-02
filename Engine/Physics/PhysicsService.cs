using System;
using System.Linq;
using Calcifer.Engine.Components;
using ComponentKit;
using ComponentKit.Model;
using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
    public class PhysicsService : IUpdateable
    {
        private readonly IEntityRecordCollection entities;
        private readonly ComponentSyncTriggerPredicate trigger;

        public World World { get; private set; }

        public PhysicsService()
        {
            this.entities = EntityRegistry.Current;
            trigger = c => c is PhysicsComponent;
            World = new World(new CollisionSystemSAP{UseTriangleMeshNormal = false})
	                    {
		                    Gravity = new JVector(0, 0, -9.81f)
	                    };
	        // Ghost objects have "Ghost" tag, so we skip them during the broadphase
	        World.CollisionSystem.PassedBroadphase +=
		        (e1, e2) => (((BodyTags) e1.BroadphaseTag | (BodyTags) e1.BroadphaseTag) & BodyTags.Ghost) != 0;
            entities.SetTrigger(trigger, ComponentSync);
        }

	    private void ComponentSync(object sender, ComponentSyncEventArgs e)
        {
            foreach (var c in e.Components.OfType<PhysicsComponent>())
            {
                if (c.IsOutOfSync)
                {
					c.World = null;
					World.RemoveBody(c.Body);
                }
                else
                {
					c.World = World;
					World.AddBody(c.Body);
                }
            }
        }

        public void Update(double dt)
        {
            World.Step((float) dt, false);
        }

        ~PhysicsService()
        {
            entities.ClearTrigger(trigger);
        }
    }
}