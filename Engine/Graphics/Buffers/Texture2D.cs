using Calcifer.Engine.Content;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Graphics.Buffers
{
    public class Texture2D: Texture, IResource
    {
        public Texture2D(string name): base(name)
        {}

        public Texture2D()
        {}

        public override void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }

        public override void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected override void Dispose(bool manual)
        {
            if (!manual) return;
            GL.DeleteTexture(ID);
        }
    }
}
