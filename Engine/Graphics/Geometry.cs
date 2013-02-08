using System.Linq;

namespace Calcifer.Engine.Graphics
{
    public class Geometry
    {
        /// <summary>
        /// Geometry material.
        /// </summary>
        public Material Material;

        /// <summary>
        /// Vertex array. Shared between many geometries.
        /// </summary>
        public SkinnedVertex[] Vertices;

        /// <summary>
        /// Triangle array. Shared between many geometries.
        /// </summary>
        public Vector3i[] Triangles;

        /// <summary>
        /// Offset in the triangle array (in bytes).
        /// </summary>
        public int Offset;
        /// <summary>
        /// Number of triangles in the geometry.
        /// </summary>
        public int Count;
    }
}
