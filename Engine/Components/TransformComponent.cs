using System.IO;
using Calcifer.Engine.Graphics.Primitives;
using ComponentKit.Model;
using OpenTK;
using System;

namespace Calcifer.Engine.Components
{
	public class TransformComponent : Component, ISaveable
	{
		private ScalableTransform transform;
		private Func<ScalableTransform> transformCallback;

		public ScalableTransform Transform
		{
			get
			{
				return (transformCallback != null) ? transformCallback() : transform;
			}
		}
		public Vector3 Translation
		{
			get
			{
				return this.Transform.Translation;
			}
			set
			{
				this.transform.Translation = value;
			}
		}
		public Quaternion Rotation
		{
			get
			{
				return this.Transform.Rotation;
			}
			set
			{
				this.transform.Rotation = value;
			}
		}
		public Vector3 Scale
		{
			get
			{
				return transform.Scale;
			}
			set
			{
				this.transform.Scale = value;
			}
		}
		public Matrix4 Matrix
		{
			get
			{
				return Matrix4.Scale(Scale) * Matrix4.Rotate(Rotation) * Matrix4.CreateTranslation(Translation);
			}
		}
		public TransformComponent()
		{
			this.transform = new ScalableTransform(Quaternion.Identity, Vector3.Zero);
		}

		public void Bind(Func<ScalableTransform> callback)
		{
			this.transformCallback = callback;
		}
		public void Unbind()
		{
			this.transformCallback = null;
		}

	    public void SaveState(BinaryWriter writer)
	    {
            writer.Write(transform.Translation.X);
            writer.Write(transform.Translation.Y);
            writer.Write(transform.Translation.Z);
            writer.Write(transform.Rotation.X);
            writer.Write(transform.Rotation.Y);
            writer.Write(transform.Rotation.Z);
            writer.Write(transform.Rotation.W);
            writer.Write(transform.Scale.X);
            writer.Write(transform.Scale.Y);
            writer.Write(transform.Scale.Z);
	    }

	    public void RestoreState(BinaryReader reader)
	    {
            transform.Translation = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            transform.Rotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            transform.Scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
	    }
	}
}
