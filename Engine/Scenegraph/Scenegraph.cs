using Calcifer.Engine.Graphics;

namespace Calcifer.Engine.Scenegraph
{
    public class Scenegraph
    {
        private readonly SceneNode root;
        public ScenegraphBuilder Builder { get; private set; }

        public Scenegraph()
        {
            root = new SceneNode(null);
            Builder = new ScenegraphBuilder(root);
        }

        public void Render(RenderPass pass)
        {
            pass.BeginRender();
            root.AcceptPass(pass);
            pass.EndRender();
        }
    }

    public class ScenegraphBuilder
    {
        public ScenegraphBuilder(SceneNode root)
        {
            throw new System.NotImplementedException();
        }
    }
}
