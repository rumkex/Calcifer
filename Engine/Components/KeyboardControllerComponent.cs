using ComponentKit.Model;
using OpenTK;
using OpenTK.Input;

namespace Calcifer.Engine.Components
{
    public class KeyboardControllerComponent : DependencyComponent, IUpdateable
    {
        [RequireComponent] private TransformComponent transform;
        private MouseState mouseState;

        public void Update(double dt)
        {
            var current = Mouse.GetState();
            if (mouseState != current)
            {
                // Mouse state has changed
                int xdelta = current.X - mouseState.X;
                int ydelta = current.Y - mouseState.Y;
                int zdelta = current.Wheel - mouseState.Wheel;
                var currentY = Vector3.Transform(Vector3.UnitY, transform.Rotation);
                var currentZ = Vector3.Transform(Vector3.UnitZ, transform.Rotation);
                //var rotY = Quaternion.FromAxisAngle(currentY, (float)dt * -0.1f * xdelta);
                //var rotZ = Quaternion.FromAxisAngle(currentZ, (float)dt * -0.1f * ydelta);
                var rotY = Quaternion.FromAxisAngle(Vector3.UnitY, (float)dt * -0.1f * xdelta);
                var rotZ = Quaternion.FromAxisAngle(Vector3.UnitZ, (float)dt * -0.1f * ydelta);
                transform.Rotation = transform.Rotation*rotY*rotZ;
            }
            mouseState = current;

            var targetVelocity = Vector3.Zero;
            if (Keyboard.GetState()[Key.W])
                targetVelocity += Vector3.UnitX;
            if (Keyboard.GetState()[Key.S])
                targetVelocity -= Vector3.UnitX;
            if (Keyboard.GetState()[Key.A])
                targetVelocity -= Vector3.UnitZ;
            if (Keyboard.GetState()[Key.D])
                targetVelocity += Vector3.UnitZ;
            if (targetVelocity != Vector3.Zero) targetVelocity.Normalize();
            var delta = (float)dt * 2.0f * Vector3.Transform(targetVelocity, transform.Rotation);
            transform.Translation += delta;
        }
    }
}