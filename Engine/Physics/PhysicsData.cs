using System.Collections.Generic;
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
        public List<JVector> Positions
        {
            get;
            private set;
        }
        public List<TriangleVertexIndices> Triangles
        {
            get;
            private set;
        }
        public List<Material> Materials
        {
            get;
            private set;
        }
        public PhysicsData(List<JVector> positions, List<TriangleVertexIndices> triangles, List<Material> materials)
        {
            Materials = materials;
            Positions = positions;
            Triangles = triangles;
        }

        public PhysicsData(IEnumerable<Geometry> mesh)
        {
            Positions = new List<JVector>();
            Triangles = new List<TriangleVertexIndices>();
            Materials = new List<Material>();
            foreach (var g in mesh)
            {
                var vmap = new Dictionary<int, int>();
                for (var i = 0; i < g.Vertices.Length; i++)
                {
                    var v = g.Vertices[i];
                    vmap.Add(i, Positions.Count);
                    Positions.Add(v.Position.ToJVector());
                    Materials.Add(new Material());
                }
                foreach (var t in g.Triangles)
                    Triangles.Add(new TriangleVertexIndices(vmap[t.X], vmap[t.Y], vmap[t.Z]));
            }
        }
    }
}