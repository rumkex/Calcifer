using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Calcifer.Utilities.Logging;
using ComponentKit;
using ComponentKit.Model;

namespace Calcifer.Engine.Components
{
    public class StateService: IService
    {
        private LinkedList<ISaveable> saveables;
        private MemoryStream stateStream;

        public StateService()
        {
            stateStream = new MemoryStream();
            saveables = new LinkedList<ISaveable>();
        }

        public void Synchronize(IEnumerable<IComponent> components)
        {
            foreach (var component in components)
                if (component.IsOutOfSync)
                    saveables.Remove((ISaveable) component);
                else
                    saveables.AddLast((ISaveable) component);
        }

        public void SaveState()
        {
            stateStream = new MemoryStream();
            var writer = new BinaryWriter(stateStream);
            var count = saveables.Count;
            Log.WriteLine(LogLevel.Info, "Tracking {0} component states", count);
            writer.Write(count);
            foreach (var component in saveables)
            {
                writer.Write(component.Record.Name);
                writer.Write(component.GetType().Name);
                component.SaveState(writer);
            }
            writer.Flush();
        }


        public void RestoreState()
        {
            if (stateStream.Length == 0) return;
            stateStream.Position = 0;
            var reader = new BinaryReader(stateStream);
            var count = reader.ReadInt32();
            Log.WriteLine(LogLevel.Info, "Restoring {0} component states", count);
            for (int i = 0; i < count; i++)
            {
                var name = reader.ReadString();
                var entity = Entity.Find(name);
                var ctype = reader.ReadString();
                var saveable = entity.GetComponents().FirstOrDefault(c => c.GetType().Name == ctype) as ISaveable;
                if (saveable != null)
                    saveable.RestoreState(reader);
                else 
                    throw new SerializationException(string.Format("Component type {0} isn't ISaveable", ctype));
            }
        }
    }
}
