using System.Collections.Generic;
using Calcifer.Engine.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Scenegraph
{
    public class LightNode: SceneNode
    {
        static LightNode()
        {
            Inventory = new LinkedList<LightNode>();
        }

        public static LinkedList<LightNode> Inventory { get; private set; }

        private readonly LinkedListNode<LightNode> nodeHandler;

        public Vector4 Ambient { get; set; }
        public Vector4 Diffuse { get; set; }
        public Vector4 Specular { get; set; }
        public Vector4 Position { get; set; }

        public LightNode(Vector3 position, Vector4 ambient, Vector4 diffuse, Vector4 specular, SceneNode parent): base(parent)
        {
            Ambient = ambient;
            Specular = specular;
            Diffuse = diffuse;
            Position = new Vector4(position, 1.0f);
            nodeHandler = Inventory.AddLast(this);
        }

        public override void AcceptPass(RenderPass pass)
        {
            pass.Visit(this);
        }

        public void Set()
        {
            const LightName id = LightName.Light0;
            GL.Light(id, LightParameter.Ambient, Ambient);
            GL.Light(id, LightParameter.Diffuse, Diffuse);
            GL.Light(id, LightParameter.Specular, Specular);
            GL.Light(id, LightParameter.Position, Position);
        }

        ~LightNode()
        {
            Inventory.Remove(nodeHandler);
        }
    }
}
