using System.Collections.Generic;
using System.Linq;
using Calcifer.Engine.Components;
using Calcifer.Engine.Scenegraph;
using ComponentKit;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Graphics
{
    public class RenderService : IService
    {
        private readonly SceneNode root;

        public RenderService()
        {
            root = new SceneNode(null);
        }

        public void Synchronize(IEnumerable<IComponent> components)
        {
            foreach (var c in components.OfType<RenderComponent>())
            {
                if (!c.IsOutOfSync) c.Attach(root);
            }
        }

        public void AddLight(Vector3 pos, Vector4 ambient, Vector4 diffuse, Vector4 specular)
        {
            GL.Enable(EnableCap.Light0);
            new LightNode(pos, ambient, diffuse, specular, this.root);
        }

        public void Render(RenderPass pass, ICamera camera)
        {
            pass.BeginRender(camera);
            root.AcceptPass(pass);
            pass.EndRender();
        }
    }
}
