using System.Collections.Generic;
using System.Linq;
using Calcifer.Engine.Content;
using Calcifer.Engine.Graphics;
using Calcifer.Utilities;
using Jitter.Collision;

namespace Calcifer.Engine.Physics
{
    public class PhysicsData : IResource
    {
        private Octree octree;

        public Octree Octree
        { 
            get
            {
                if (octree == null)
                {
                    var tris = Shapes[0].Triangles.Select(t => new TriangleVertexIndices(t.X, t.Y, t.Z)).ToList();
                    var verts = Shapes[0].Vertices.Select(v => v.Position.ToJVector()).ToList();
                    octree = new Octree(verts, tris);
                }
                return octree;
            } 
        }

        public List<Geometry> Shapes { get; private set; }

        public PhysicsData(IEnumerable<Geometry> mesh)
        {
	        Shapes = mesh.ToList();
        }

	    private PhysicsData(PhysicsData source)
	    {
		    Shapes = source.Shapes;
	    }

	    public object Clone()
	    {
		    return new PhysicsData(this);
	    }
    }
}