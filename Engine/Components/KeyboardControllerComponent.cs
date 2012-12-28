using Calcifer.Utilities;
using ComponentKit.Model;
using OpenTK;
using OpenTK.Input;

namespace Calcifer.Engine.Components
{
    public class KeyboardControllerComponent : DependencyComponent, IUpdateable
    {
        [RequireComponent] private TransformComponent transform;
        private MouseState mouseState;
	    private float yaw, pitch;
		
	    public void Update(double dt)
        {
            var current = Mouse.GetState();
            if (mouseState != current)
            {
                // Mouse state has changed
                int xdelta = current.X - mouseState.X;
                int ydelta = current.Y - mouseState.Y;
                int zdelta = current.Wheel - mouseState.Wheel;

				yaw += (float)dt * 0.02f * -xdelta;
				pitch += (float)dt * 0.08f * ydelta;
				pitch = MathUtils.Clamp(pitch, -MathHelper.PiOver2 + .01f, MathHelper.PiOver2 - .01f);

	            transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, yaw)*
	                                 Quaternion.FromAxisAngle(Vector3.UnitY, pitch);
            }
            mouseState = current;

	        var forward = Vector3.Transform(Vector3.UnitX, transform.Rotation);
	        var right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitZ));

            var targetVelocity = Vector3.Zero;
	        if (Keyboard.GetState()[Key.W])
		        targetVelocity += forward;
            if (Keyboard.GetState()[Key.S])
                targetVelocity -= forward;
            if (Keyboard.GetState()[Key.A])
                targetVelocity -= right;
            if (Keyboard.GetState()[Key.D])
				targetVelocity += right;
            if (targetVelocity != Vector3.Zero)
			{
				targetVelocity.Normalize();
				var delta = (float) dt*2.0f*targetVelocity;
				transform.Translation += delta;
            }
        }
    }
}