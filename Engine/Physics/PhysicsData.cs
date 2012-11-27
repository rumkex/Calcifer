using System.Collections.Generic;
using System.Linq;
using Calcifer.Engine.Content;
using Calcifer.Engine.Graphics;
using Calcifer.Utilities;
using Jitter.Collision;
using Jitter.LinearMath;
using Material = Jitter.Dynamics.Material;

namespace Calcifer.Engine.Physics
{
    public class PhysicsData : IResource
    {
        public struct SubShape
        {
            public Material Material;
            public List<JVector> Positions;
            public List<TriangleVertexIndices> Triangles;

            public SubShape(IEnumerable<JVector> pos, IEnumerable<TriangleVertexIndices> tri, Material material)
            {
                Material = material;
                Positions = new List<JVector>(pos);
                Triangles = new List<TriangleVertexIndices>(tri);
            }
        }

        public List<SubShape> Shapes { get; private set; }

        public PhysicsData(IEnumerable<Geometry> mesh)
        {
            Shapes = mesh.Select(g => 
                                 new SubShape(
                                     g.Vertices.Select(v => v.Position.ToJVector()),
                                     g.Triangles.Select(t => new TriangleVertexIndices(t.X, t.Y, t.Z)),
                                     new Material()
                                     )).ToList();
        }
    }
}