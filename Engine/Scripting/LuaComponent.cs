using System.Collections.Generic;
using System.IO;
using Calcifer.Engine.Components;
using Calcifer.Engine.Scenery;
using Calcifer.Utilities;
using ComponentKit.Model;

namespace Calcifer.Engine.Scripting
{
    public class LuaComponent : Component, IUpdateable, ISaveable, IConstructable
    {
        private float wait;
        
        public LuaService Service { get; set; }

        public string Source { get; private set; }

        public bool IsWaiting
        {
            get { return wait > 0f; }
        }

        public void Update(double dt)
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

        public void SaveState(BinaryWriter writer)
        {
            writer.Write(wait);
        }

        public void RestoreState(BinaryReader reader)
        {
            wait = reader.ReadSingle();
        }

        void IConstructable.Construct(IDictionary<string, string> param)
        {
            Source = param.Get("source", null) ?? File.ReadAllText(param["sourceRef"]);
        }
    }
}