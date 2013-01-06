using System.Collections.Generic;
using ComponentKit.Model;

namespace Calcifer.Engine.Scripting
{
    public class LuaStorageComponent : Component
    {
        public bool CanPush
        {
            get;
            set;
        }
        public bool CanHitWall
        {
            get;
            set;
        }
        public bool CanClimb
        {
            get;
            set;
        }
        public bool CanGetOver
        {
            get;
            set;
        }
        public List<string> Nodes
        {
            get;
            private set;
        }
        public int CurrentNode
        {
            get;
            set;
        }
        public LuaStorageComponent(IEnumerable<string> nodes)
        {
            this.Nodes = new List<string>(nodes);
        }
    }
}