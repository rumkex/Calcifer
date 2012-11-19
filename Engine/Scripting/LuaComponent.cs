using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentKit.Model;

namespace Sunflower.Engine.Scripting
{

    //public class LuaComponent: DependencyComponent
    //{
    //    public string Source { get; private set; }
    //    public bool IsWaiting { get { return wait < 0.0f; } }

    //    private LuaService service;
    //    private float wait;

    //    public LuaComponent(string code)
    //    {
    //        Source = code;
    //    }

    //    protected override void OnAdded(ComponentStateEventArgs e)
    //    {
    //        service = Owner.Manager.GetService<LuaService>();
    //         base.OnAdded(e);
    //    }

    //    public override void Update(double dt)
    //    { 
    //        if (IsWaiting)
    //            wait -= (float)dt;
    //        service.ExecuteScript(this);
    //    }

    //    public void Wait(float seconds)
    //    {
    //        wait = seconds;
    //    }
    //}

    //// Miscellaneous properties from LSA
    //public class LuaStorageComponent: Component
    //{
    //    public bool CanWalk { get; set; }
    //    public bool CanPush { get; set; }
    //    public bool CanHitWall { get; set; }
    //    public bool CanClimb { get; set; }
    //    public bool CanGetOver { get; set; }

    //    public List<string> Nodes { get; private set; }
    //    public int CurrentNode { get; set; }

    //    public LuaStorageComponent(IEnumerable<string> nodes)
    //    {
    //        Nodes = new List<string>(nodes);
    //    }
    //}
}
