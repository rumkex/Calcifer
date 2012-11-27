using Calcifer.Engine.Graphics.Animation;
using Calcifer.Utilities;
using ComponentKit.Model;
using Jitter.LinearMath;
using OpenTK;

namespace Calcifer.Engine.Physics
{
    public class MotionComponent : DependencyComponent
    {
        [RequireComponent(AllowDerivedTypes = true)] private AnimationComponent anim;
        [RequireComponent] private PhysicsComponent phys;
        private CharacterController controller;

        protected override void OnAdded(ComponentStateEventArgs e)
        {
 	        base.OnAdded(e);
            phys.Body.Material.KineticFriction = 0f;
            phys.Body.Material.Restitution = 0f;
            phys.Body.SetMassProperties(JMatrix.Zero, 1f/phys.Body.Mass, true);
            controller = new CharacterController(phys.World, phys.Body);
            phys.World.AddConstraint(controller);
        }

        public bool IsOnGround
        {
            get { return controller.BodyWalkingOn != null; }
        }

        public void SetTargetVelocity(Vector3 speed)
        {
            controller.TargetVelocity = speed.ToJVector();
        }

        public void Jump()
        {
            controller.TryJump = true;
        }
    }
}