using System.Linq;
using Calcifer.Engine.Components;
using ComponentKit;
using Jitter;
using Jitter.Collision;

namespace Calcifer.Engine.Physics
{
    public class PhysicsService : IUpdateable
    {
        private readonly IEntityRecordCollection entities;
        private readonly ComponentSyncTriggerPredicate trigger;

        public World World { get; private set; }

        public PhysicsService(IEntityRecordCollection entities)
        {
            this.entities = entities;
            trigger = c => c is PhysicsComponent;
            World = new World(new CollisionSystemSAP{UseTriangleMeshNormal = false});
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