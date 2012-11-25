using Calcifer.Engine.Components;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Scenegraph
{
    public class TransformNode : SceneNode
    {
        private TransformComponent transform;

        public TransformNode(SceneNode parent, TransformComponent transform) : base(parent)
        {
            this.transform = transform;
        }

        public override void BeginRender()
        {
            GL.PushMatrix();
            var m = transform.Matrix;
            GL.MultMatrix(ref m);
        }

        public override void EndRender()
        {
            GL.PopMatrix();
        }
    }
}
