using System.Linq;
using Calcifer.Engine.Graphics.Primitives;

namespace Calcifer.Engine.Graphics
{
    public class Geometry
    {
        public Geometry()
        {
            Vertices = new SkinnedVertex[0];
            Triangles = new Vector3i[0];
        }

        public Material Material;
        public SkinnedVertex[] Vertices;
        public Vector3i[] Triangles;

        public bool Validate()
        {
            return Triangles.All(triangle => (triangle.X < Vertices.Length) && (triangle.Y < Vertices.Length) && (triangle.Z < Vertices.Length));
        }
    }
}
