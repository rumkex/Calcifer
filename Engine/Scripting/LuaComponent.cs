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
        private LuaService service;
        private float wait;
        public string Source
        {
            get;
            private set;
        }
        public bool IsWaiting
        {
            get
            {
                return this.wait < 0f;
            }
        }
        public LuaComponent(string code)
        {
            this.Source = code;
        }

        public override void Update(double dt)
        {
            if (this.IsWaiting)
            {
                this.wait -= (float)dt;
            }
            this.service.ExecuteScript(this);
        }
        public void Wait(float seconds)
        {
            this.wait = seconds;
        }
    }
}