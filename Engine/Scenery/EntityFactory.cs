using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ComponentKit;
using ComponentKit.Model;

namespace Calcifer.Engine.Scenery
{
    public interface IConstructable: IComponent
    {
        void Construct(IDictionary<string, string> param);
    }


    public static class EntityFactory
    {
        private delegate IComponent ComponentActivator();
        private static DefinitionCollection definitions;
        private static Dictionary<string, ComponentActivator> constructors;
        
        static EntityFactory()
        {
            definitions = new DefinitionCollection();
            constructors = new Dictionary<string, ComponentActivator>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.GetInterfaces().Contains(typeof (IComponent))) continue;
                    if (type.GetConstructor(Type.EmptyTypes) == null) continue;
                    var ctr = Expression.New(type);
                    var lambda = Expression.Lambda<ComponentActivator>(ctr).Compile();
                    constructors.Add(type.Name, lambda);
                }
            }
        }

        public static void Define(EntityDefinition definition)
        {
            if (!definitions.Contains(definition.Name))
                definitions.Add(definition);
        }

        public static IEntityRecord Create(string name, string className)
        {
            var def = definitions[className];
            return Create(name, def);
        }

        public static IEntityRecord Create(EntityInstance instance)
        {
            var def = definitions[instance.ClassName];
            var newdef = new EntityDefinition(instance.Name);
            foreach (var p in def.Parameters)
                newdef.Parameters.Add(p);
            foreach (var delta in instance.Deltas)
            {
                var key = delta.Component + ":" + delta.Parameter;
                if (delta.Type == DeltaType.Add)
                {
                    if (newdef.Parameters.Contains(key)) newdef.Parameters.Remove(key);
                    newdef.Parameters.Add(new Parameter(delta.Component, delta.Parameter, delta.Value));
                }
                else if (newdef.Parameters.Contains(key)) newdef.Parameters.Remove(key);
            }
            return Create(instance.Name, newdef);
        }
        
        private static IEntityRecord Create(string name, EntityDefinition definition)
        {
            var e = Entity.Create(name);
            foreach (var c in definition.Parameters.GetComponents())
                e.Add(BuildComponent(c, definition.Parameters));
            return e;
        }

        private static IComponent BuildComponent(string type, ParameterCollection param)
        {
            if (!constructors.ContainsKey(type))
                throw new EngineException(string.Format("Type {0} has no parameterless constructor", type));
            var c = constructors[type]();
            var constructable = c as IConstructable;
            if (constructable != null) constructable.Construct(param.GetComponent(type));
            return c;
        }
    }
}
