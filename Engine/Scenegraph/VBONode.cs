using System;
using Calcifer.Engine.Graphics;
using Calcifer.Engine.Graphics.Buffers;
using Calcifer.Engine.Graphics.Primitives;
using Calcifer.Utilities.Logging;
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


        public override void VisitChildren(RenderPass pass)
        {
            if (VBO.Disposed || IBO.Disposed) return;
            base.VisitChildren(pass);
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
        private int count;
        private int offset;

        public SubmeshNode(SceneNode parent, Geometry g) : base(parent)
        {
            count = g.Count;
            offset = g.Offset;
        }

        public override void RenderNode()
        {
            // Draw submesh
            GL.DrawElements(BeginMode.Triangles, count * 3, DrawElementsType.UnsignedShort, offset);
        }
    }
}
