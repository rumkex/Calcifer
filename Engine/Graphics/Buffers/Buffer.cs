using System;

namespace Calcifer.Engine.Graphics.Buffers
{
    public abstract class Buffer : IDisposable
    {
        public int ID { get; protected set; }
        public abstract void Bind();
        public abstract void Unbind();

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool manual);

        ~Buffer()
        {
            Dispose(false);
        }
    }
}