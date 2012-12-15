using Calcifer.Engine.Graphics.Primitives;
using ComponentKit.Model;
using OpenTK;
using System;

namespace Calcifer.Engine.Components
{
	public class TransformComponent : Component
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
	}
}
