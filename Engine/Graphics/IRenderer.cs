using Calcifer.Engine.Scenegraph;

namespace Calcifer.Engine.Graphics
{
    public interface IRenderer
    {
		void Render(SceneNode rootNode, Camera camera);
		void AddRenderPass(RenderPass pass);
    }

    public class BasicRenderer : IRenderer
    {
        private RenderPass pass;
        public void Render(SceneNode rootNode, Camera camera)
        {
            rootNode.AcceptPass(this.pass);
        }
        public void AddRenderPass(RenderPass p)
        {
            this.pass = p;
        }
    }
}