using Calcifer.Engine.Graphics;
using Calcifer.Engine.Graphics.Animation;
using Calcifer.Engine.Graphics.Primitives;
using Calcifer.Engine.Scenegraph;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.IO;
namespace Calcifer.Engine.Graphics
{
    public abstract class RenderPass
    {
        public virtual void Visit(SceneNode node)
        {
            node.BeginRender();
            node.RenderNode();
            node.VisitChildren(this);
            node.EndRender();
        }
        public virtual void Visit(LightNode node)
        {
            this.Visit((SceneNode)node);
        }
        public virtual void Visit(MaterialNode node)
        {
            this.Visit((SceneNode)node);
        }
        public virtual void Visit(TransformNode node)
        {
            this.Visit((SceneNode)node);
        }
        public virtual void Visit(AnimationNode node)
        {
            this.Visit((SceneNode)node);
        }
        public virtual void Visit(VBONode node)
        {
            this.Visit((SceneNode)node);
        }
        public virtual void Visit(SubmeshNode node)
        {
            this.Visit((SceneNode)node);
        }
        public virtual void BeginRender(ICamera camera)
        {
        }
        public virtual void EndRender()
        {
        }
    }

	public class BaseRenderPass : RenderPass
	{
	    private const int MaxBones = 60;

		private readonly Shader shader;
		private float[] boneCache = new float[16 * MaxBones];
		public BaseRenderPass()
		{
            for (int i = 0; i < boneCache.Length; i++)
		    {
		        if ((i%16)%5 == 0) boneCache[i] = 1;
		    }
			shader = ShaderFactory.Create(File.OpenRead("../FX/skin.vert"), File.OpenRead("../FX/skin.frag"));
		}
		public override void BeginRender(ICamera camera)
		{
			var mat = camera.Matrix;
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref mat);
			foreach (var light in LightNode.Inventory) light.Set();
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			this.shader.Enable();
		}
		public override void EndRender()
		{
			this.shader.Disable();
		}
		public override void Visit(MaterialNode node)
		{
			node.BeginRender();
			this.shader.SetUniform(this.shader.GetUniformLocation("diffuseMap"), 0);
			node.VisitChildren(this);
			node.EndRender();
		}
		public override void Visit(AnimationNode node)
		{
            for (int i = 0; i < node.Pose.BoneCount; i++)
            {
                node.Pose[i].GetMatrix(boneCache, i * 16);
            }
			this.shader.SetUniform("animated", 1f);
            GL.UniformMatrix4(this.shader.GetUniformLocation("Bones"), node.Pose.BoneCount, false, this.boneCache);
			node.VisitChildren(this);
			this.shader.SetUniform("animated", 0f);
		}

		public override void Visit(VBONode node)
		{
			int vPos = this.shader.GetAttribLocation("inVertex");
			int nPos = this.shader.GetAttribLocation("inNormal");
			int tcPos = this.shader.GetAttribLocation("inTexCoord");
			int weightPos = this.shader.GetAttribLocation("inWeights");
			int bonesPos = this.shader.GetAttribLocation("inBones");
			GL.EnableVertexAttribArray(vPos);
			GL.EnableVertexAttribArray(nPos);
			GL.EnableVertexAttribArray(tcPos);
			GL.EnableVertexAttribArray(weightPos);
			GL.EnableVertexAttribArray(bonesPos);
			node.BeginRender();
			GL.VertexAttribPointer(vPos, 3, VertexAttribPointerType.Float, false, SkinnedVertex.Size, 0);
			GL.VertexAttribPointer(nPos, 3, VertexAttribPointerType.Float, true, SkinnedVertex.Size, (IntPtr)12);
			GL.VertexAttribPointer(tcPos, 2, VertexAttribPointerType.Float, false, SkinnedVertex.Size, (IntPtr)24);
			GL.VertexAttribPointer(weightPos, 4, VertexAttribPointerType.Float, false, SkinnedVertex.Size, (IntPtr)32);
			GL.VertexAttribPointer(bonesPos, 4, VertexAttribPointerType.Float, false, SkinnedVertex.Size, (IntPtr)48);
			node.VisitChildren(this);
			node.EndRender();
			GL.DisableVertexAttribArray(vPos);
			GL.DisableVertexAttribArray(nPos);
			GL.DisableVertexAttribArray(tcPos);
			GL.DisableVertexAttribArray(weightPos);
			GL.DisableVertexAttribArray(bonesPos);
		}
	}

}
