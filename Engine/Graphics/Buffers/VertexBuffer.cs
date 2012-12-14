using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Graphics.Buffers
{
    /// <summary>
    /// Provides managed handler for vertex buffer
    /// </summary>
    public class VertexBuffer: Buffer
    {
        public BufferTarget Target { get; private set; }
        public BufferUsageHint Usage { get; private set; }

        public int Size { get; private set; }

        public VertexBuffer(int size, BufferTarget target, BufferUsageHint usage)
        {
            Target = target;
            Usage = usage;
            Size = size;
            if (!GL.IsEnabled(EnableCap.VertexArray))
                throw new EngineException("Cannot create buffer object: VertexArray state is not enabled");
            int id;
            GL.GenBuffers(1, out id);
            GL.BindBuffer(Target, id);
            ID = id;
            // Allocate space
            GL.BufferData(Target, (IntPtr)size, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            Size = size;
        }

        public override void Bind()
        {
            GL.BindBuffer(Target, ID);
        }

        public override void Unbind()
        {
            GL.BindBuffer(Target, 0);
        }

        public void Write<T>(int offset, T[] data) where T : struct
        {
            Bind();
            int datasize = data.Length*Marshal.SizeOf(typeof (T));
            if (offset + datasize > Size) throw new EngineException("Vertex buffer: out of bounds");
            GL.BufferSubData(Target, (IntPtr)offset, (IntPtr)datasize, data);
        }

        protected override void Dispose(bool manual)
        {
            int id = ID;
            if (manual) GL.DeleteBuffers(1, ref id);
        }

        public override string ToString()
        {
            return "Buffer #" + ID;
        }
    }
}
