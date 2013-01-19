using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcifer.Engine.Scenery
{
    public class MapBuilder
    {
        private readonly Stack<EntityDefinition> entityStack;
        private readonly Map map;
        private string currentComponent;

        public MapBuilder()
        {
            map = new Map();
            entityStack = new Stack<EntityDefinition>();
        }

        public void BeginEntity(string name)
        {
            entityStack.Push(new EntityDefinition(name));
        }

        public void EndEntity()
        {
            if (entityStack.Count == 0)
            {
                throw new InvalidOperationException("EndEntity called before BeginEntity");
            }
            var def = entityStack.Pop();
            // entity names are usually "<classname>.NNN", so we extract the class name
            var pos = def.Name.LastIndexOf('.');
            var entityName = def.Name;
            var entityClass = (entityName.Contains(".node")) ? "node" :
                (pos < 0) ? entityName : entityName.Substring(0, pos);
            if (!map.Definitions.Contains(entityClass))
            {
                def.Name = entityClass;
                map.Definitions.Add(def);
            }
            var instance = CalculateDelta(entityName, map.Definitions[entityClass], def);
            map.Instances.Add(instance);
        }

        private EntityInstance CalculateDelta(string name, EntityDefinition def, EntityDefinition instance)
        {
            var result = new EntityInstance();
            foreach (var param in instance.Parameters)
                if (!def.Parameters.Contains(param.Component + ":" + param.Name) || (def.Parameters[param.Component + ":" + param.Name].Value != param.Value))
                    result.Deltas.Add(new DeltaEntry(DeltaType.Add, param.Component, param.Name, param.Value));
            foreach (var param in def.Parameters)
                if (!instance.Parameters.Contains(param.Component + ":" + param.Name)) 
                    result.Deltas.Add(new DeltaEntry(DeltaType.Remove, param.Component, param.Name, param.Value));
            result.Name = name;
            result.ClassName = def.Name;
            return result;
        }
        
        public void BeginComponent(string type)
        {
            if (currentComponent != null) throw new InvalidOperationException("Component stack not empty.");
            currentComponent = type;
        }

        public void EndComponent()
        {
            if (!entityStack.Peek().Parameters.GetComponents().Contains(currentComponent))
            {
                entityStack.Peek().Parameters.Add(new Parameter(currentComponent, null, null));
            }
            currentComponent = null;
        }

        public void AddParameter(string name, string value)
        {
            entityStack.Peek().Parameters.Add(new Parameter(currentComponent, name, value));
        }

        public Map GetMap()
        {
            if (entityStack.Count != 0)
            {
                throw new InvalidOperationException("Entity stack not empty.");
            }
            return map;
        }

        public void AddAsset(string alias, bool composite, string source)
        {
            if (!map.Assets.Contains(alias))
                map.Assets.Add(new AssetReference {Name = alias, Source = source, Composite = composite});
        }
    }
}