using System.Collections.Generic;
using ComponentKit;
using ComponentKit.Model;

namespace Calcifer.Engine.Scripting
{
    public class WaypointComponent : Component
    {
		public List<IEntityRecord> Nodes
        {
            get;
            private set;
        }
        public int CurrentNode
        {
            get;
            set;
        }

        public WaypointComponent()
        {
            Nodes = new List<IEntityRecord>();
        }

        public WaypointComponent(IEnumerable<IEntityRecord> nodes): this()
        {
			Nodes.AddRange(nodes);
        }
    }
}