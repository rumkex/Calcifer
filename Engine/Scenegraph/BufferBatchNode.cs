using System;
using Calcifer.Engine.Graphics;
using Calcifer.Engine.Graphics.Buffers;

namespace Calcifer.Engine.Scenegraph
{
    public class BufferBatchNode: SceneNode, IDisposable
    {
        private BufferManager manager;

        public BufferBatchNode(SceneNode parent) : base(parent)
        {
            manager = new BufferManager(RenderHints<int>.GetHint("VBOSize"), RenderHints<int>.GetHint("VBOChunkSize"));
        }

        public override void AcceptPass(RenderPass pass)
        {
            pass.Visit(this);
        }

        protected bool Disposed;
        public void Dispose()
        {
            if (!Disposed) return;
            manager.Dispose();
            Disposed = true;
        }
    }
}
