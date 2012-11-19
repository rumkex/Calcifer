using System;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Graphics.Buffers
{
    internal class Framebuffer : Buffer
    {
        private bool ext;

        public Framebuffer()
        {
            int id;
            GL.GenFramebuffers(1, out id);
            ID = id;
        }

        public override void Bind()
        {
            throw new NotImplementedException();
        }

        public override void Unbind()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool manual)
        {
            if (!manual) return;
            int id = ID;
            GL.DeleteFramebuffers(1, ref id);
        }
    }
}
