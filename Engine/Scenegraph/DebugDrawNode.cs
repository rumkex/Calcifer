using Calcifer.Engine.Graphics;
using Jitter;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Scenegraph
{
    public class DebugDrawNode : SceneNode, IDebugDrawer
    {
        private RigidBody body;
        public DebugDrawNode(SceneNode parent, RigidBody b)
            : base(parent)
        {
            body = b;
        }
        public override void RenderNode()
        {
            if (!RenderHints<bool>.GetHint("debugPhysics")) return;
            Shader.Current.Disable();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            GL.Color4(Color4.White);
            GL.Begin(BeginMode.Lines);
            body.DebugDraw(this);
            GL.End();
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);
            Shader.Current.Enable();
        }

        public void DrawLine(JVector pos1, JVector pos2)
        {
            GL.Vertex3(pos1.X, pos1.Y, pos1.Z);
            GL.Vertex3(pos2.X, pos2.Y, pos2.Z);
        }

        public void DrawPoint(JVector pos)
        {
            GL.End();
            GL.Begin(BeginMode.Points);
            GL.Vertex3(pos.X, pos.Y, pos.Z);
            GL.End();
            GL.Begin(BeginMode.Lines);
        }

        public void DrawTriangle(JVector pos1, JVector pos2, JVector pos3)
        {
            GL.Vertex3(pos1.X, pos1.Y, pos1.Z);
            GL.Vertex3(pos2.X, pos2.Y, pos2.Z);
            GL.Vertex3(pos2.X, pos2.Y, pos2.Z);
            GL.Vertex3(pos3.X, pos3.Y, pos3.Z);
            GL.Vertex3(pos3.X, pos3.Y, pos3.Z);
            GL.Vertex3(pos1.X, pos1.Y, pos1.Z);
        }
    }
}
