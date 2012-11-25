using System.ComponentModel.Design;
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
	    public event EventHandler<ComponentEventArgs> TransformChanged;

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
				return this.Transform.Scale;
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
				return this.Transform.Matrix;
			}
		}
		public TransformComponent()
		{
			this.transform = new ScalableTransform(Quaternion.Identity, Vector3.Zero);
		}
		protected virtual void OnTransformChanged(ComponentEventArgs e)
		{
			EventHandler<ComponentEventArgs> transformChanged = this.TransformChanged;
			if (transformChanged != null)
			{
				transformChanged(this, e);
			}
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
