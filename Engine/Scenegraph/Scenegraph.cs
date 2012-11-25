using System;
using System.Collections.Generic;
using System.Linq;
using Calcifer.Engine.Components;
using Calcifer.Engine.Graphics;
using Calcifer.Engine.Graphics.Animation;
using Calcifer.Engine.Graphics.Buffers;
using Calcifer.Engine.Graphics.Primitives;
using Calcifer.Engine.Physics;
using ComponentKit;
using ComponentKit.Model;
using Jitter;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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
        private class DebugDrawNode : SceneNode
        {
            private RigidBody body;
            public DebugDrawNode(SceneNode parent, RigidBody b)
                : base(parent)
            {
                this.body = b;
            }
            public override void RenderNode()
            {
            }
        }
        internal class GLDrawer : IDebugDrawer
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
        private readonly SceneNode root;
        public ScenegraphBuilder(SceneNode root)
        {
            this.root = root;
        }
        public void AddLight(Vector3 pos, Vector4 ambient, Vector4 diffuse, Vector4 specular)
        {
            GL.Enable(EnableCap.Light0);
            new LightNode(pos, ambient, diffuse, specular, this.root);
        }
        public void AddModelFromEntity(IEntityRecord entity)
        {
            this.AddModelFromEntity(this.root, entity);
        }
        public void AddModelFromEntity(SceneNode parent, IEntityRecord entity)
        {
            var physicsComponent = entity.GetComponent<PhysicsComponent>();
            var transformComponent = entity.GetComponent<TransformComponent>();
            var meshData = entity.GetComponent<MeshData>();
            if (meshData == null)
            {
                return;
            }
            SceneNode head = new TransformNode(parent, transformComponent ?? new TransformComponent());
            var animationComponent = entity.GetComponent<AnimationComponent>();
            if (animationComponent != null) head = new AnimationNode(head, animationComponent);
            var icount = 0;
            var vcount = 0;
            var writeQueue = new Queue<Action<VBONode>>();
            foreach (var matGroup in
                from g in meshData.Submeshes
                group g by g.Material)
            {
                foreach (var geometry in matGroup)
                {
                    for (var i = 0; i < geometry.Triangles.Length; i++)
                    {
                        geometry.Triangles[i].X = (ushort) (geometry.Triangles[i].X + vcount);
                        geometry.Triangles[i].Y = (ushort) (geometry.Triangles[i].Y + vcount);
                        geometry.Triangles[i].Z = (ushort) (geometry.Triangles[i].Z + vcount);
                    }
                    var geom = geometry;
                    var vc = vcount;
                    var ic = icount;
                    var mat = matGroup.Key;
                    writeQueue.Enqueue(v => new SubmeshNode(new MaterialNode(v, mat), v, geom, vc * SkinnedVertex.Size, ic * 4));
                    vcount += geometry.Vertices.Length;
                    icount += geometry.Triangles.Length * 3;
                }
            }
            var vbo = new VertexBuffer(vcount * SkinnedVertex.Size, BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw)
            { ElementCount = vcount };
            var ibo = new VertexBuffer(icount * 4, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw)
            { ElementCount = icount };
            var obj = new VBONode(head, vbo, ibo);
            foreach (var action in writeQueue) action(obj);
        }
    }
}
