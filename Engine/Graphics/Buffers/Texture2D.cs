using Calcifer.Engine.Content;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Graphics.Buffers
{
	public class Texture2D : Texture, IResource
    {
        public Texture2D(string name, int width, int height) : base(name)
        {
            Width = width;
            Height = height;
        }

        public Texture2D()
        {}

        public int Height { get; private set; }

        public int Width { get; private set; }

	    public override void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }

        public override void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

	    public object Clone()
	    {
		    RefCount++;
		    return this;
	    }
    }
}
