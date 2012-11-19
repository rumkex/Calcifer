using System;
using Calcifer.Engine.Graphics;
using Calcifer.Engine.Graphics.Buffers;
using Calcifer.Engine.Graphics.Primitives;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Scenegraph
{
    public class VBONode: SceneNode, IDisposable
    {
        public VertexBuffer VBO { get; private set; }
        public VertexBuffer IBO { get; private set; }

        public VBONode(SceneNode parent, VertexBuffer vbo, VertexBuffer ibo) : base(parent)
        {
            VBO = vbo;
            IBO = ibo;
        }

        public override void AcceptPass(RenderPass pass)
        {
            pass.Visit(this);
        }

        public override void BeginRender()
        {
            VBO.Bind();
            IBO.Bind();
        }

        public override void EndRender()
        {
            VBO.Unbind();
            IBO.Unbind();
        }

        protected bool disposed;
        public void Dispose()
        {
            if (!disposed) return;
            VBO.Dispose();
            IBO.Dispose();
            disposed = true;
        }
    }

    public class SubmeshNode : SceneNode
    {
        private int ioffset;
        private int tricount;
        private VBONode vbo;

        public SubmeshNode(SceneNode parent, VBONode vboNode, Geometry g, int vboOffset, int iboOffset) : base(parent)
        {
            vbo = vboNode;
            ioffset = iboOffset;
            tricount = g.Triangles.Length;
            vbo.VBO.Write(vboOffset, g.Vertices);
            vbo.IBO.Write(ioffset, g.Triangles);
        }

        public override void RenderNode()
        {
            if (ioffset < 0) return;
            if (vbo.VBO.Disposed || vbo.IBO.Disposed) return;
            // Draw submesh
            GL.DrawElements(BeginMode.Triangles, tricount * Vector3i.Size, DrawElementsType.UnsignedShort, ioffset);
        }
    }
}
