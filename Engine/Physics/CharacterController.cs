using Calcifer.Utilities.Logging;
using Jitter;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
    public class CharacterController : Constraint
    {
        enum State
        {
            Grounded = 0,
            Jumping = 1,
            Falling = 2
        }

        private JVector normal = JVector.Backward;
        private State state;

        public CharacterController(World world, RigidBody body) : base(body, null)
        {
            World = world;
			JMatrix orient = body.Orientation;
            JVector down = JVector.Forward, ray;
			JVector.TransposedTransform(ref down, ref orient, out down);
            body.Shape.SupportMapping(ref down, out ray);
			FeetPosition = ray * down;
            JumpVelocity = 5f;
            FallVelocity = 5f;
        }

		public float FeetPosition { get; private set; }
        public float JumpVelocity { get; set; }
        public float FallVelocity { get; set; }
        public JVector TargetVelocity { get; set; }
        public World World { get; private set; }
        public bool TryJump { get; set; }
        public RigidBody BodyWalkingOn { get; set; }

        public override void PrepareForIteration(float timestep)
        {
            RigidBody body;
            float depth;
        
            bool flag = World.CollisionSystem.Raycast(Body1.Position + JVector.Forward*(FeetPosition - 0.1f),
                                                      JVector.Forward, (b, n, f) => b != Body1 && (b.BroadphaseTag & (int)BodyTags.Ghost) == 0,
                                                      out body, out normal, out depth);
            BodyWalkingOn = ((!flag || depth > 0.2f) ? null : body);
            var canJump = (BodyWalkingOn != null) && Body1.LinearVelocity.Z < JumpVelocity;

            var oldState = state;
            if ((BodyWalkingOn == null && state != State.Jumping) || (-Body1.LinearVelocity.Z > FallVelocity)) state = State.Falling;
            if (state != State.Grounded && depth < 0.1f && Body1.LinearVelocity.Z < 0.0f) state = State.Grounded;
            if (canJump && TryJump) state = State.Jumping;
            if (state != oldState) Log.WriteLine(LogLevel.Debug, "switched from {0} to {1}", oldState, state);
        }

        public override void Iterate()
        {
            // Controlled movement happens in every state
            var deltaVelocity = TargetVelocity - Body1.LinearVelocity;
            deltaVelocity -= JVector.Dot(deltaVelocity, normal) * normal;
            // However while in the air, control is greatly reduced
            deltaVelocity *= (state == State.Grounded) ? 0.2f: 0.01f;
            if (deltaVelocity.LengthSquared() > 0.000001f)
            {
                Body1.ApplyImpulse(deltaVelocity * Body1.Mass);
            }
            switch (state)
            {
                case State.Grounded:
                    // If player just stands on the ground,
                    // this reduces overall jumpiness
                    var nvel = Body1.LinearVelocity * normal;
                    Body1.LinearVelocity -= 0.7f*nvel*normal;
                    break;
                case State.Falling:
                    // First let the gravity do its job
                    if (-Body1.LinearVelocity.Z < FallVelocity) break;
                    // If it's moving too fast, constrain the falling velocity
                    var dv = 0.5f * (FallVelocity + Body1.LinearVelocity.Z);
                    Body1.LinearVelocity += dv*JVector.Forward;
                    break;
                case State.Jumping:
                    // There are multiple iterations per step, so apply jump impulse only once
                    if (!TryJump) break;
                    Body1.ApplyImpulse(JumpVelocity*JVector.Backward*Body1.Mass);
                    Log.WriteLine(LogLevel.Debug, "JUMP!");
	                TryJump = false;
                    break;
            }
        }
    }
}