using System.Linq;

namespace Calcifer.Engine.Graphics
{
    public class Geometry
    {
        public Material Material;
        public SkinnedVertex[] Vertices;
        public Vector3i[] Triangles;

        public int Offset, Count;
    }
}
