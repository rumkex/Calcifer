using System.Collections.Generic;
using Calcifer.Engine.Graphics.Primitives;
using OpenTK;

namespace Calcifer.Engine.Graphics
{
    public class GeometryBuilder
    {
        private List<Geometry> submeshes; 
        private readonly Dictionary<Vector3, LinkedList<int>> vertindex;
        private readonly List<SkinnedVertex> vertices;
        private readonly List<Vector3i> triangles;
        private readonly int[] indices;

        public GeometryBuilder()
        {
            submeshes = new List<Geometry>();
            vertindex = new Dictionary<Vector3, LinkedList<int>>();
            indices = new int[4];
            vertices = new List<SkinnedVertex>();
            triangles = new List<Vector3i>();
        }

        public Material Material { get; set; }

        public void Add(IList<SkinnedVertex> v, bool generateNormals)
        {
            if (v.Count > 4) throw new EngineException("Trying to build a face with more than 4 vertices.");
            if (v.Count == 4)
            {
                Add(new[] { v[0], v[1], v[2] }, generateNormals);
                Add(new[] { v[2], v[3], v[0] }, generateNormals);
                return;
            }
            for (int i = 0; i < v.Count; i++ )
            {
                var vert = v[i];
                if (generateNormals)
                {
                    var temp = (i + v.Count - 1) % v.Count;
                    var left = v[temp].Position - v[i].Position;
                    var right = v[(i + 1) % v.Count].Position - v[i].Position;
                    vert.Normal = Vector3.Normalize(Vector3.Cross(left, right));
                }
                if (!vertindex.ContainsKey(vert.Position))
                {
                    vertindex.Add(vert.Position, new LinkedList<int>());
                    vertindex[vert.Position].AddLast(vertices.Count);
                    indices[i] = vertices.Count;
                    vertices.Add(vert);
                }
                else
                {
                    var found = false;
                    foreach (var index in vertindex[vert.Position])
                        if (Equals(vertices[index], vert))
                        {
                            found = true;
                            indices[i] = index;
                        }
                    if (!found)
                    {
                        indices[i] = vertices.Count;
                        vertindex[vert.Position].AddLast(vertices.Count);
                        vertices.Add(vert);
                    }
                }
            }
            triangles.Add(new Vector3i(indices));
        }

        public void Add(SkinnedVertex v1, SkinnedVertex v2, SkinnedVertex v3)
        {
            Add(new[] {v1, v2, v3}, false);
        }

        public void Add(SkinnedVertex v1, SkinnedVertex v2, SkinnedVertex v3, SkinnedVertex v4)
        {
            Add(new[] { v1, v2, v3, v4 }, false);
        }

        public IEnumerable<Geometry> GetGeometry()
        {
            return submeshes;
        }

        public void NextGeometry()
        {
            var g = new Geometry
                        {
                            Triangles = triangles.ToArray(),
                            Vertices = vertices.ToArray(),
                            Material = Material
                        };
            submeshes.Add(g);
        }
    }
}
