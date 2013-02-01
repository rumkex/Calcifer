using System.Collections.Generic;
using System.IO;
using Calcifer.Engine.Components;
using Calcifer.Engine.Scenery;
using Calcifer.Utilities;
using ComponentKit.Model;
using OpenTK;
using System;

namespace Calcifer.Engine
{
	public class TransformComponent : Component, ISaveable, IConstructable
	{
		private ScalableTransform transform;
		private Func<ScalableTransform> transformCallback;
	    private Action<ScalableTransform> transformFeedback;

	    public ScalableTransform Transform
		{
			get
			{
                if (transformCallback != null) transform = transformCallback();
				return transform;
			}
		}
		public Vector3 Translation
		{
			get
			{
				return Transform.Translation;
			}
			set
			{
			    transform = Transform;
				transform.Translation = value;
			    if (transformFeedback != null) transformFeedback(transform);
			}
		}
		public Quaternion Rotation
		{
			get
			{
				return Transform.Rotation;
			}
			set
            {
                transform = Transform;
                transform.Rotation = value;
                if (transformFeedback != null) transformFeedback(transform);
			}
		}
		public Vector3 Scale
		{
			get
			{
				return Transform.Scale;
			}
			set
            {
                transform = Transform;
                transform.Scale = value;
                if (transformFeedback != null) transformFeedback(transform);
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

		public void Bind(Func<ScalableTransform> callback, Action<ScalableTransform> feedback)
		{
			transformCallback = callback;
		    transformFeedback = feedback;
		}
		public void Unbind()
		{
            transformCallback = null;
            transformFeedback = null;
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

	    void IConstructable.Construct(IDictionary<string, string> param)
	    {
            var r = param["rotation"].ConvertToVector();
	        Translation = param["translation"].ConvertToVector();
	        Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, r.X)*
	                   Quaternion.FromAxisAngle(Vector3.UnitY, r.Y)*
	                   Quaternion.FromAxisAngle(Vector3.UnitZ, r.Z);
	        Scale = param["scale"].ConvertToVector();
	    }
	}
}
