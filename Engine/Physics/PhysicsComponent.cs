using System;
using System.IO;
using Calcifer.Engine.Components;
using Calcifer.Engine.Graphics.Primitives;
using Calcifer.Utilities;
using ComponentKit;
using ComponentKit.Model;
using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;

namespace Calcifer.Engine.Physics
{
	[Flags]
	public enum BodyTags
	{
		None = 0,
		Ghost = 1,
	}

	public class PhysicsComponent : DependencyComponent, ISaveable
    {
        [RequireComponent] private TransformComponent transform = null;

		private Transform baseTransform = Transform.Identity;
        private Vector3 storedScale;

		public event EventHandler<ComponentStateEventArgs> Synchronized;

		public PhysicsComponent()
		{
			Body = new RigidBody(new SphereShape(0.0f))
			{
				IsStatic = true
			};
		}

        public PhysicsComponent(RigidBody body)
        {
            Body = body;
            Body.Material.KineticFriction = 0.5f;
            Body.Material.StaticFriction = 0.5f;
            Body.Material.Restitution = 0.5f;
        }

        public RigidBody Body { get; private set; }
        public Vector3 Offset { get; private set; }
        public World World { get; set; }

	    protected override void OnAdded(ComponentStateEventArgs registrationArgs)
        {
            base.OnAdded(registrationArgs);
			Body.Tag = Record.Name;
            var offset = Body.Position;
            var rot = JMatrix.CreateFromQuaternion(transform.Rotation.ToQuaternion());
            JVector.Transform(ref offset, ref rot, out offset);
            Offset = offset.ToVector3();
            Body.Orientation = Body.Orientation * rot;
            Body.Position = offset + transform.Translation.ToJVector();
			baseTransform = new Transform(JQuaternion.CreateFromMatrix(Body.Orientation).ToQuaternion(),
										 Body.Position.ToVector3()).Invert() * new Transform(transform.Rotation, transform.Translation);
            storedScale = transform.Scale;
            transform.Bind(GetTransform, SetTransform);
        }

        private void SetTransform(ScalableTransform t)
        {
            storedScale = t.Scale;
            var tr = new Transform(t.Rotation, t.Translation);
            var current = tr*baseTransform.Invert();
            Body.Position = current.Translation.ToJVector();
            Body.Orientation = JMatrix.CreateFromQuaternion(current.Rotation.ToQuaternion());
        }

        private ScalableTransform GetTransform()
        {
			var current = new Transform(JQuaternion.CreateFromMatrix(Body.Orientation).ToQuaternion(),
										 Body.Position.ToVector3());
	        var t = current*baseTransform;
            return new ScalableTransform(t.Rotation, t.Translation, storedScale);
        }

        public bool CollidesWith(RigidBody other)
        {
            return Record.HasComponent<SensorComponent>() ? 
                Record.GetComponent<SensorComponent>().CollidingBodies.Contains(other) : 
                Body.CollisionIsland.Bodies.Contains(other);
        }

		public void Synchronize()
		{
			if (Synchronized != null) Synchronized(this, new ComponentStateEventArgs(Record));
		}

	    public void SaveState(BinaryWriter writer)
	    {
            writer.Write(Body.Position.X); writer.Write(Body.Position.Y); writer.Write(Body.Position.Z);
            var q = JQuaternion.CreateFromMatrix(Body.Orientation);
            writer.Write(q.X); writer.Write(q.Y); writer.Write(q.Z); writer.Write(q.W);
	        if (Body.IsStatic) return;
            writer.Write(Body.LinearVelocity.X); writer.Write(Body.LinearVelocity.Y); writer.Write(Body.LinearVelocity.Z);
            writer.Write(Body.AngularVelocity.X); writer.Write(Body.AngularVelocity.Y); writer.Write(Body.AngularVelocity.Z);
	    }

	    public void RestoreState(BinaryReader reader)
	    {
            Body.Position = new JVector(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            var q = new JQuaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Body.Orientation = JMatrix.CreateFromQuaternion(q);
            if (Body.IsStatic) return;
            Body.LinearVelocity = new JVector(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Body.AngularVelocity = new JVector(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
	    }
    }
}