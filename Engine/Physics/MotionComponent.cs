using System;
using Calcifer.Engine.Components;
using Calcifer.Engine.Graphics.Animation;
using Calcifer.Utilities;
using ComponentKit;
using ComponentKit.Model;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using OpenTK;

namespace Calcifer.Engine.Physics
{
    public class MotionComponent : DependencyComponent, IUpdateable
    {
        [RequireComponent(AllowDerivedTypes = true)] private AnimationComponent anim;
        [RequireComponent] private PhysicsComponent phys;
        private CharacterController controller;

        protected override void OnAdded(ComponentStateEventArgs e)
        {
			base.OnAdded(e);
			phys.Body.Material.KineticFriction = 0f;
	        phys.Body.Material.StaticFriction = 0f;
            phys.Body.Material.Restitution = 0f;
	        var invInertia = phys.Body.InverseInertia;
			invInertia.M11 = 0;
			invInertia.M22 = 0;
	        invInertia.M33 = 0;
			phys.Body.SetMassProperties(invInertia, 1f, true);
			phys.Synchronized += OnSynchronized;
        }

        protected override void OnRemoved(ComponentStateEventArgs e)
        {
            phys.World.RemoveConstraint(controller);
            base.OnRemoved(e);
        }

	    private void OnSynchronized(object sender, ComponentStateEventArgs e)
	    {
		    if (!IsOutOfSync)
			{
				controller = new CharacterController(phys.World, phys.Body);
			    phys.World.AddConstraint(controller);
		    }
	    }

	    public bool IsOnGround
        {
            get { return controller.BodyWalkingOn != null; }
        }

	    public string GetFloorMaterial()
	    {
		    if (!IsOnGround) return "";
			var floor = Entity.Find(controller.BodyWalkingOn.Tag.ToString());
			var terrainComponent = floor.GetComponent<TerrainComponent>();
		    if (terrainComponent == null) return "";
			var material = terrainComponent.GetMaterial(phys.Body.Position + JVector.Forward * (controller.FeetPosition - 0.1f), JVector.Forward);
		    return material;
	    }

	    public void SetTargetVelocity(Vector3 speed)
        {
	        phys.Body.IsActive = true;
            controller.TargetVelocity = speed.ToJVector();
        }

	    private double cooldown;

        public void Jump()
		{
			phys.Body.IsActive = true;
	        if (cooldown > 0) return;
			cooldown = 1.0;
			controller.TryJump = true;
        }

	    public void Update(double t)
	    {
			if (cooldown > 0) cooldown -= t;
		}

	    public void SetAngularVelocity(Vector3 w)
	    {
		    phys.Body.AngularVelocity = w.ToJVector();
	    }
    }
}