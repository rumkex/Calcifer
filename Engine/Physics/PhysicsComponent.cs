using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Calcifer.Engine.Components;
using Calcifer.Engine.Content;
using Calcifer.Engine.Scenery;
using Calcifer.Utilities;
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
        Projectile = 2,
	}

	public class PhysicsComponent : DependencyComponent, ISaveable, IConstructable
	{
        [RequireComponent] private TransformComponent transform = null;

		private Transform baseTransform = Transform.Identity;
        private Vector3 storedScale;

		public event EventHandler<ComponentStateEventArgs> Synchronized;
        
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
            if (Record.HasComponent<SensorComponent>())
                return Record.GetComponent<SensorComponent>().CollidingBodies.Contains(other);

            //if (Body.CollisionIsland.Bodies.Contains(other)) return true;
            return Body.Arbiters.FirstOrDefault(a => a.Body1 == other || a.Body2 == other) != null;
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

        void IConstructable.Construct(IDictionary<string, string> param)
        {
            var type = param["type"];
            switch (type)
            {
                case "trimesh":
                    var physData = ResourceFactory.LoadAsset<PhysicsData>(param["physData"]);
                    Shape shape = new TriangleMeshShape(physData.Octree);
                    Body = new RigidBody(shape) { Material = { Restitution = 0f, KineticFriction = 0f } };
                    break;
                case "hull":
                    physData = ResourceFactory.LoadAsset<PhysicsData>(param["physData"]);
                    shape = new ConvexHullShape(physData.Vertices);
                    Body = new RigidBody(shape);
                    break;
                case "sphere":
                    shape = new SphereShape(float.Parse(param["radius"], CultureInfo.InvariantCulture));
                    Body = new RigidBody(shape);
                    break;
                case "box":
                    var d = param["size"].ConvertToVector();
                    var offset = param.Get("offset", "0;0;0").ConvertToVector();
                    shape = new BoxShape(2.0f * d.ToJVector());
                    Body = new RigidBody(shape) { Position = offset.ToJVector() };
                    break;
                case "capsule":
                    var height = float.Parse(param["height"], CultureInfo.InvariantCulture);
                    var radius = float.Parse(param["radius"], CultureInfo.InvariantCulture);
                    shape = new CapsuleShape(height, radius);
                    Body = new RigidBody(shape)
                    {
                        Position = JVector.Backward * (0.5f * height + radius),
                        Orientation = JMatrix.CreateRotationX(MathHelper.PiOver2)
                    };
                    break;
                default:
                    throw new Exception("Unknown shape: " + type);
            }
            Body.IsStatic = Convert.ToBoolean(param.Get("static", "false"));
            Body.Material.KineticFriction = 0.5f;
            Body.Material.StaticFriction = 0.5f;
            Body.Material.Restitution = 0.5f;
        }
    }
}