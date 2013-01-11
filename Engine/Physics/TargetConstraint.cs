using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calcifer.Utilities.Logging;
using Jitter;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
    public class TargetConstraint: Constraint
    {
        private JVector targetVelocity = JVector.Zero;

        public TargetConstraint(RigidBody body)
            : base(body, null)
        {}

        public float Velocity { get; set; }
        public JVector Target { get; set; }

        public override void PrepareForIteration(float timestep)
        {
            var delta = Target - Body1.Position;
            var length = delta.Length();
            if (length > 1f*Velocity)
            {
                if (Velocity > targetVelocity.Length())
                    targetVelocity = JVector.Normalize(delta)*(targetVelocity.Length() + Velocity*timestep);
                else
                    targetVelocity = JVector.Normalize(delta)*Velocity;
            }
            else
                targetVelocity = JVector.Normalize(delta) * (float) Math.Sqrt(length);
        }

        public override void Iterate()
        {
            var deltaVelocity = targetVelocity - Body1.LinearVelocity;
            if (deltaVelocity.LengthSquared() > 0.000001f)
                Body1.ApplyImpulse(deltaVelocity*Body1.Mass);
        }
    }
}
