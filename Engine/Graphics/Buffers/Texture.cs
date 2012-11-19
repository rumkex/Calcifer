using Calcifer.Engine.Content;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Graphics.Buffers
{
    public abstract class Texture: Buffer, IResource
    {
        public string Name { get; protected set; }

        protected Texture(string name)
        {
            ID = GL.GenTexture();
            Name = name;
        }

        protected Texture()
        {
            ID = GL.GenTexture();
            Name = "unnamedtexture" + ID;
        }

        protected override void Dispose(bool manual)
        {
            if (!manual) return;
            GL.DeleteTexture(ID);
        }

		public override string ToString ()
		{
			return string.Format ("[Texture: {0}", Name);
		}
    }
}
