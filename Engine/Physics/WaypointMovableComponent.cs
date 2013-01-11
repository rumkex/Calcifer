using System;
using Calcifer.Engine.Components;
using Calcifer.Engine.Scripting;
using Calcifer.Utilities;
using ComponentKit.Model;
using Jitter.LinearMath;
using OpenTK;

namespace Calcifer.Engine.Physics
{
    public class WaypointMovableComponent: DependencyComponent, IUpdateable
    {
        [RequireComponent] private TransformComponent transform;
        [RequireComponent] private PhysicsComponent physics;
        [RequireComponent] private WaypointComponent nodes;

        public WaypointMovableComponent()
        {
            Speed = 1.0f;
        }

        protected override void OnAdded(ComponentStateEventArgs registrationArgs)
        {
            base.OnAdded(registrationArgs);
            physics.Body.IsStatic = true;
            physics.Body.Material.Restitution = 0f;
            physics.Body.Material.KineticFriction = 0.5f;
            physics.Body.Material.StaticFriction = 0.5f;
        }

        public void Update(double dt)
        {
            if (!Active) return;
            var target = nodes.Nodes[nodes.CurrentNode];
            var otherTransform = target.GetComponent<TransformComponent>();
            var delta = otherTransform.Translation.ToJVector() - physics.Body.Position;
            if (delta.Length() < dt*Speed)
            {
                physics.Body.Position = otherTransform.Translation.ToJVector();
                Active = false;
            }
            else
                physics.Body.Position += JVector.Normalize(delta) * (float)dt * Speed;
        }

        public float Speed { get; set; }
        public bool Active { get; private set; }

        public void Activate()
        {
            Active = true;
        }
    }
}
