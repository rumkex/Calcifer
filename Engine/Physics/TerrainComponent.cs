using System;
using System.Collections.Generic;
using System.Linq;
using Calcifer.Engine.Content;
using Calcifer.Engine.Scenery;
using ComponentKit.Model;
using Jitter.Collision;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
	public class TerrainComponent: DependencyComponent, IConstructable
	{
		private Octree octree;
		private List<Tuple<int, int, string>> materials;

		public string GetMaterial(JVector start, JVector delta)
		{
			var result = new List<int>();
			var count = octree.GetTrianglesIntersectingRay(result, start, delta);
		    if (count == 0) return "";
		    var match = materials.FirstOrDefault(t => result[0] >= t.Item1 && result[0] < t.Item1 + t.Item2);
		    return match != null ? match.Item3 : "";
		}

	    void IConstructable.Construct(IDictionary<string, string> param)
        {
            var physData = ResourceFactory.LoadAsset<PhysicsData>(param["physData"]);
            materials = physData.Shapes.Select(g => new Tuple<int, int, string>(g.Offset, g.Count, g.Material.Name)).ToList();
	        octree = physData.Octree;
        }
	}
}
