using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentKit.Model;

namespace Calcifer.Engine.Scripting
{
    public class PlayerStateComponent: Component
    {
        public bool CanHitWall { get; set; }

        public bool CanGetOver { get; set; }

        public bool CanPush { get; set; }

        public bool CanClimb { get; set; }
    }
}
