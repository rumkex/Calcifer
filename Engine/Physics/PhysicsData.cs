using System.Collections.Generic;
using System.Linq;
using Calcifer.Engine.Content;
using Jitter.Collision;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
    public class PhysicsData : IResource
    {
        private Octree octree;

        public Octree Octree
        { 
            get
            {
                if (octree == null) octree = new Octree(Vertices, Triangles);
                return octree;
            } 
        }

        public List<JVector> Vertices { get; private set; }
        public List<TriangleVertexIndices> Triangles { get; private set; }

        public PhysicsData(IEnumerable<JVector> vertices, IEnumerable<TriangleVertexIndices> triangles) 
        {
            Vertices = vertices.ToList();
            Triangles = triangles.ToList();
        }

	    private PhysicsData(PhysicsData source)
	    {
		    Vertices = source.Vertices;
	        Triangles = source.Triangles;
	    }

	    public object Clone()
	    {
		    return new PhysicsData(this);
	    }
    }
}