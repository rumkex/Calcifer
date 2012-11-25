using Calcifer.Utilities.Logging;
using Jitter;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
    public class CharacterController : Constraint
    {
        private readonly float feetPosition;
        private bool canJump;
        private JVector deltaVelocity = JVector.Zero;

        public CharacterController(World world, RigidBody body) : base(body, null)
        {
            World = world;
            var down = JVector.Down;
            JVector ray;
            body.Shape.SupportMapping(ref down, out ray);
            feetPosition = ray*JVector.Down;
            JumpVelocity = 2f;
        }

        public float JumpVelocity { get; set; }
        public JVector TargetVelocity { get; set; }
        public World World { get; private set; }
        public bool TryJump { get; set; }
        public RigidBody BodyWalkingOn { get; set; }

        public override void PrepareForIteration(float timestep)
        {
            RigidBody body;
            JVector normal;
            float depth;
            bool flag = World.CollisionSystem.Raycast(Body1.Position + JVector.Down*(feetPosition - 0.1f),
                                                      JVector.Down, (b, n, f) => b != Body1,
                                                      out body, out normal, out depth);
            BodyWalkingOn = ((!flag || depth > 0.2f) ? null : body);
            canJump = (flag && depth <= 0.2f && Body1.LinearVelocity.Y < JumpVelocity && TryJump);
            TryJump = false;
        }

        public override void Iterate()
        {
            deltaVelocity = TargetVelocity - Body1.LinearVelocity;
            deltaVelocity.Y = 0f;
            deltaVelocity *= ((!canJump) ? 0.01f : 0.5f);
            if (deltaVelocity.LengthSquared() > 0.000001f)
            {
                Body1.IsActive = true;
                Body1.ApplyImpulse(deltaVelocity*Body1.Mass);
            }
            if (canJump)
            {
                Body1.IsActive = true;
                Body1.ApplyImpulse(JumpVelocity*JVector.Up*base.Body1.Mass);
                Log.WriteLine(LogLevel.Debug, "JUMP!");
                if (!BodyWalkingOn.IsStatic)
                {
                    BodyWalkingOn.IsActive = true;
                    BodyWalkingOn.ApplyImpulse(-1f*JumpVelocity*JVector.Up*base.Body1.Mass);
                }
            }
        }
    }
}