using System.Collections.Generic;
using System.IO;
using Calcifer.Engine.Components;
using ComponentKit;
using ComponentKit.Model;

namespace Calcifer.Engine.Scripting
{
    public class WaypointComponent : Component, ISaveable
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
    }
}