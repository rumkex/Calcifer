using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace Calcifer.Engine.Scenery
{
    [XmlRoot("definition")]
    public class EntityDefinition
    {
        [XmlAttribute("class")]
        public string Name { get; set; }

        [XmlArray("params"), XmlArrayItem("param")]
        public ParameterCollection Parameters;

        public EntityDefinition(string name)
        {
            Parameters = new ParameterCollection();
            Name = name;
        }

        public EntityDefinition() : this("")
        {}

        public string this[string key]
        {
            get { return Parameters.Contains(key) ? Parameters[key].Value: null; }
        }
    }

    public class ParameterCollection: KeyedCollection<string, Parameter>
    {
        private readonly Dictionary<string, int> components = new Dictionary<string, int>(); 

        public IEnumerable<string> GetComponents()
        {
            return components.Keys;
        }

        protected override void RemoveItem(int index)
        {
            components[base[index].Component] -= 1;
            if (components[base[index].Component] <= 0) components.Remove(base[index].Component);
            base.RemoveItem(index);
        }

        protected override void InsertItem(int index, Parameter item)
        {
            if (!components.ContainsKey(item.Component)) components.Add(item.Component, 0);
            components[item.Component] += 1;
            base.InsertItem(index, item);
        }

        protected override string GetKeyForItem(Parameter item)
        {
            return item.Component + ":" + item.Name;
        }

        public IDictionary<string, string> GetComponent(string type)
        {
            return this.Where(p => p.Component == type).ToDictionary(p => p.Name, p => p.Value);
        }
    }

    [XmlRoot("param")]
    public struct Parameter
    {
        [XmlAttribute("component")]
        public string Component;
        [XmlAttribute("param")]
        public string Name;
        [XmlText]
        public string Value;
        
        public Parameter(string component, string name, string value)
        {
            Component = component;
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return Component + ":" + Name + " = " + Value;
        }
    }
}