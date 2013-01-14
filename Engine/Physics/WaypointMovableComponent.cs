using System;
using Calcifer.Engine.Components;
using Calcifer.Engine.Scripting;
using Calcifer.Utilities;
using ComponentKit.Model;
using Jitter.LinearMath;
using OpenTK;

namespace Calcifer.Engine.Physics
{
    public class WaypointMovableComponent: DependencyComponent
    {
        [RequireComponent] private PhysicsComponent physics;
        [RequireComponent] private WaypointComponent nodes;

        private TargetConstraint constraint;
        
        protected override void OnAdded(ComponentStateEventArgs registrationArgs)
        {
            base.OnAdded(registrationArgs);
            physics.Body.Material.Restitution = 0f;
            physics.Body.Material.KineticFriction = 0.5f;
            physics.Body.Material.StaticFriction = 0.5f;
            physics.Body.Mass = 1000f;
            physics.Body.SetMassProperties(JMatrix.Zero, 1/physics.Body.Mass, true);
            constraint = new TargetConstraint(physics.Body) {Velocity = 3.0f};
            Activate();
            physics.Synchronized += (sender, args) =>
                                        {
                                            if (!IsOutOfSync)
                                                physics.World.AddConstraint(constraint);
                                            else
                                                physics.World.RemoveConstraint(constraint);
                                        };
        }
        
        public void Activate()
        {
            var target = nodes.Nodes[nodes.CurrentNode];
            var otherTransform = target.GetComponent<TransformComponent>();
            constraint.Target = (otherTransform.Translation + physics.Offset).ToJVector();
        }
    }
}
