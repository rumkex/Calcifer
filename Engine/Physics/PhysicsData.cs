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