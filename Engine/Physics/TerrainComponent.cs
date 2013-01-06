using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentKit.Model;
using Jitter.Collision;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
	public class TerrainComponent: DependencyComponent
	{
		private readonly Octree octree;
		private List<Tuple<int, int, string>> materials;

		public TerrainComponent(IEnumerable<Tuple<int, int, string>> materials, Octree octree)
		{
			this.octree = octree;
			this.materials = new List<Tuple<int, int, string>>(materials);
		}

		public string GetMaterial(JVector start, JVector delta)
		{
			var result = new List<int>();
			var count = octree.GetTrianglesIntersectingRay(result, start, delta);
			return materials.First(t => result[0] >= t.Item1 && result[0] < t.Item1 + t.Item2).Item3;
		}
	}
}
