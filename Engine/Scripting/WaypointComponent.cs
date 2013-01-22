using System.Collections.Generic;
using System.IO;
using System.Linq;
using Calcifer.Engine.Components;
using Calcifer.Engine.Scenery;
using ComponentKit;
using ComponentKit.Model;

namespace Calcifer.Engine.Scripting
{
    public class WaypointComponent : Component, ISaveable, IConstructable
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

        public void SaveState(BinaryWriter writer)
        {
            writer.Write(CurrentNode);
        }

        public void RestoreState(BinaryReader reader)
        {
            CurrentNode = reader.ReadInt32();
        }

        void IConstructable.Construct(IDictionary<string, string> param)
        {
            Nodes = param["nodes"].Split(';').Select(Entity.Find).ToList();
        }
    }
}