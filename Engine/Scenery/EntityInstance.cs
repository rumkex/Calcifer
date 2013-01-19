using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Calcifer.Engine.Scenery
{
    [XmlRoot("instance")]
    public class EntityInstance
    {
		[XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("class")]
        public string ClassName { get; set; }

        [XmlArray("state"), XmlArrayItem("delta")]
        public List<DeltaEntry> Deltas { get; set; }

        public EntityInstance()
        {
            Deltas = new List<DeltaEntry>();
        }
    }

    public enum DeltaType
    {
        Add = 0,
        Remove = 1
    }

    public class DeltaEntry
    {
        [XmlAttribute("type")]
        public DeltaType Type;

        [XmlAttribute("component")]
        public string Component;

        [XmlAttribute("param")]
        public string Parameter;

        [XmlText]
        public string Value;

        public DeltaEntry()
        {}

		public DeltaEntry(DeltaType type, string component, string param = "*", string value = "")
		{
		    Type = type;
            Component = component;
		    Parameter = param;
			Value = value;
		}
    }
}