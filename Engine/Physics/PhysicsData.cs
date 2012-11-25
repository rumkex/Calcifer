using System.Collections.Generic;
using Calcifer.Engine.Content;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
    public class PhysicsData : IResource
    {
        public List<JVector> Positions
        {
            get;
            private set;
        }
        public List<TriangleVertexIndices> Triangles
        {
            get;
            private set;
        }
        public List<Material> Materials
        {
            get;
            private set;
        }
        public PhysicsData(List<JVector> positions, List<TriangleVertexIndices> triangles, List<Material> materials)
        {
            this.Materials = materials;
            this.Positions = positions;
            this.Triangles = triangles;
        }
    }
}