using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calcifer.Engine.Graphics;
using Jitter;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Scenegraph
{
    public class DebugDrawNode : SceneNode
    {
        private class GLDrawer : IDebugDrawer
        {
            public void DrawLine(JVector start, JVector end)
            {
            }
            public void DrawPoint(JVector pos)
            {
            }
            public void DrawTriangle(JVector pos1, JVector pos2, JVector pos3)
            {
                GL.Vertex3(pos1.X, pos1.Y, pos1.Z);
                GL.Vertex3(pos2.X, pos2.Y, pos2.Z);
                GL.Vertex3(pos3.X, pos3.Y, pos3.Z);
            }
        }

        private static GLDrawer drawer = new GLDrawer();

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
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Begin(BeginMode.Triangles);
            GL.Color4(Color4.White);
            body.DebugDraw(drawer);
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);
            Shader.Current.Enable();
        }
    }
}
