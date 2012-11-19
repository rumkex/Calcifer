namespace Calcifer.Engine.Graphics
{
    class State
    {
        public static State Current { get; private set; }

        public TexturingState Texturing { get; private set; }
    }

    internal class TexturingState
    {
        public int ID { get; set; }
        public OpenTK.Graphics.OpenGL.TextureTarget Target { get; set; }
    }
}
