using Calcifer.Engine.Components;
using ComponentKit.Model;

namespace Calcifer.Engine.Scripting
{
    public abstract class ScriptingComponent : Component, IUpdateable
    {
        public virtual void Update(double dt)
        {
        }
    }

    public class LuaComponent : ScriptingComponent
    {
        private float wait;

        public LuaComponent(string code)
        {
            Source = code;
        }

        public LuaService Service { get; set; }

        public string Source { get; private set; }

        public bool IsWaiting
        {
            get { return wait < 0f; }
        }

        public override void Update(double dt)
        {
            if (IsWaiting)
            {
                wait -= (float) dt;
            }
            Service.ExecuteScript(this);
        }

        public void Wait(float seconds)
        {
            wait = seconds;
        }
    }
}