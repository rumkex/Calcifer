using System.Collections.Generic;
using System.Linq;
using Calcifer.Engine.Components;
using Calcifer.Engine.Content;
using Calcifer.Engine.Graphics.Animation;
using Calcifer.Engine.Graphics.Buffers;
using Calcifer.Engine.Physics;
using Calcifer.Engine.Scenegraph;
using Calcifer.Engine.Scenery;
using ComponentKit.Model;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Graphics
{
    public class RenderComponent: DependencyComponent, IConstructable
    {
        private MeshData meshData;
        
        private SceneNode localRoot, localParent;

        public void Attach(SceneNode parent)
        {
            localParent = parent;
            localRoot = parent = new SceneNode(parent);
            var physicsComponent = Record.GetComponent<PhysicsComponent>();
            if (physicsComponent != null) new DebugDrawNode(parent, physicsComponent.Body);

            var transformComponent = Record.GetComponent<TransformComponent>();
            if (meshData == null) return;
            parent = new TransformNode(parent, transformComponent ?? new TransformComponent());
            var animationComponent = Record.GetComponent(default(AnimationComponent), true);
            if (animationComponent != null) parent = new AnimationNode(parent, animationComponent);
            var tri = meshData.Submeshes[0].Triangles;
            var vert = meshData.Submeshes[0].Vertices;
            var vbo = new VertexBuffer(vert.Length * SkinnedVertex.Size, BufferTarget.ArrayBuffer,
                                       BufferUsageHint.StaticDraw);
            var ibo = new VertexBuffer(tri.Length * Vector3i.Size, BufferTarget.ElementArrayBuffer,
                                       BufferUsageHint.StaticDraw);
            vbo.Write(0, vert);
            ibo.Write(0, tri);
            var vboNode = new VBONode(parent, vbo, ibo);
            foreach (var matGroup in from g in meshData.Submeshes group g by g.Material)
            {
                foreach (var geometry in matGroup)
                {
                    var mat = matGroup.Key;
                    new SubmeshNode(new MaterialNode(vboNode, mat), geometry);
                }
            }
        }

        protected override void OnAdded(ComponentStateEventArgs e)
        {
            base.OnAdded(e);
        }

        protected override void OnRemoved(ComponentStateEventArgs e)
        {
            base.OnRemoved(e);
            localParent.RemoveChild(localRoot);
        }

        void IConstructable.Construct(IDictionary<string, string> param)
        {
            meshData = ResourceFactory.LoadAsset<MeshData>(param["meshData"]);
        }
    }
}
