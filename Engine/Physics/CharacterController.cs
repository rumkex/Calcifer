using Calcifer.Utilities.Logging;
using Jitter;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
    public class CharacterController : Constraint
    {
        private bool canJump;
        private JVector deltaVelocity = JVector.Zero;

        public CharacterController(World world, RigidBody body) : base(body, null)
        {
            World = world;
			JMatrix orient = body.Orientation;
            JVector down = JVector.Forward, ray;
			JVector.TransposedTransform(ref down, ref orient, out down);
            body.Shape.SupportMapping(ref down, out ray);
			FeetPosition = ray * down;
            JumpVelocity = 5f;
        }

		public float FeetPosition { get; private set; }
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
            bool flag = World.CollisionSystem.Raycast(Body1.Position + JVector.Forward*(FeetPosition - 0.1f),
                                                      JVector.Forward, (b, n, f) => b != Body1,
                                                      out body, out normal, out depth);
            BodyWalkingOn = ((!flag || depth > 0.2f) ? null : body);
            canJump = (BodyWalkingOn != null) && Body1.LinearVelocity.Z < JumpVelocity && TryJump;
        }

        public override void Iterate()
        {
            deltaVelocity = TargetVelocity - Body1.LinearVelocity;
            deltaVelocity.Z = 0f;
            deltaVelocity *= (BodyWalkingOn == null) ? 0.01f : 0.2f;
            if (deltaVelocity.LengthSquared() > 0.000001f)
            {
                Body1.ApplyImpulse(deltaVelocity*Body1.Mass);
            }
            if (canJump)
            {
                Body1.ApplyImpulse(JumpVelocity*JVector.Backward*base.Body1.Mass);
                Log.WriteLine(LogLevel.Debug, "JUMP!");
				//if (BodyWalkingOn != null && !BodyWalkingOn.IsStatic)
				//{
				//	BodyWalkingOn.IsActive = true;
				//	BodyWalkingOn.ApplyImpulse(-1f*JumpVelocity*JVector.Backward*base.Body1.Mass);
				//}
				canJump = false;
	            TryJump = false;
            }
        }
    }
}