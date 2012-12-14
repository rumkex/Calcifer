using System.Collections.Generic;
using System.Linq;
using Calcifer.Engine.Graphics.Primitives;
using OpenTK;

namespace Calcifer.Engine.Graphics
{
    public class GeometryBuilder
    {
        private readonly List<Geometry> submeshes;
        private readonly List<SkinnedVertex> vertices;
        private readonly Dictionary<Vector3, LinkedList<int>> vertindex;
        private readonly List<Vector3i> triangles;
        private readonly int[] indices;
        
        public GeometryBuilder()
        {
            submeshes = new List<Geometry> {new Geometry()};
            vertindex = new Dictionary<Vector3, LinkedList<int>>();
            indices = new int[3];
            vertices = new List<SkinnedVertex>();
            triangles = new List<Vector3i>();
        }
        
        public void Add(IList<SkinnedVertex> v, bool generateNormals)
        {
            if (v.Count < 3) throw new EngineException("Trying to build a face with less than 3 vertices.");
            if (v.Count >= 4)
            {
                Add(v.Take(3).ToList(), generateNormals);
                Add(v.Skip(2).Concat(new[] {v[0]}).ToList(), generateNormals);
                return;
            }
            for (var i = 0; i < v.Count; i++ )
            {
                var vert = v[i];
                if (generateNormals)
                {
                    var temp = (i + v.Count - 1) % v.Count;
                    var left = v[temp].Position - v[i].Position;
                    var right = v[(i + 1) % v.Count].Position - v[i].Position;
                    vert.Normal = Vector3.Normalize(Vector3.Cross(left, right));
                }
                if (!vertindex.ContainsKey(vert.Position)) vertindex.Add(vert.Position, new LinkedList<int>());
                var found = false;
                foreach (var index in vertindex[vert.Position])
                {
                    if (Equals(vertices[index], vert))
                    {
                        found = true;
                        indices[i] = index;
                        break;
                    }
                }
                if (found) continue;
                indices[i] = vertices.Count;
                vertindex[vert.Position].AddLast(vertices.Count);
                vertices.Add(vert);
            }
            submeshes.Last().Count += v.Count;
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
            var tri = triangles.ToArray();
            var vert = vertices.ToArray();
            foreach (var g in submeshes)
            {
                g.Triangles = tri;
                g.Vertices = vert;
                if (g.Count > 0) yield return g;
            }
        }

        public void NextGeometry()
        {
            var current = submeshes.Last();
            if (current.Count == 0) return;
            var g = new Geometry
                        {
                            Material = current.Material,
                            Offset = triangles.Count * Vector3i.Size,
                            Count = 0
                        };
            submeshes.Add(g);
        }

        public Material Material
        {
            get { return submeshes.Last().Material; }
            set { submeshes.Last().Material = value; }
        }
    }
}
