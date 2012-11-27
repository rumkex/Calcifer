using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Calcifer.Engine.Scenery
{
	[XmlRoot("component", Namespace = "urn:Map")]
	public class ComponentDefinition : List<Parameter>, IXmlSerializable
	{
		public string Type
		{
			get;
			set;
		}
		public string this[string key]
		{
			get
			{
			    var result = Find(p => p.Name == key);
			    return result != null ? result.Value: null;
			}
		}
		public ComponentDefinition()
		{
		}
		public ComponentDefinition(string type, IEnumerable<Parameter> parameters) : base(parameters)
		{
			this.Type = type;
		}
		public ComponentDefinition(string type, params Parameter[] parameters) : this(type, (IEnumerable<Parameter>)parameters)
		{
		}

		public bool TryGetValue(string key, out string value)
		{
			value = "";
			var parameter = Find(p => p.Name == key);
			if (parameter == null)
			{
				return false;
			}
			value = parameter.Value;
			return true;
		}
		public XmlSchema GetSchema()
		{
			return null;
		}
		public void ReadXml(XmlReader reader)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Parameter));
			if (reader.MoveToContent() != XmlNodeType.Element || reader.LocalName != "component")
			{
				return;
			}
			this.Type = reader.GetAttribute("type");
			if (reader.ReadToDescendant("param"))
			{
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "param")
				{
					Parameter parameter = xmlSerializer.Deserialize(reader) as Parameter;
					if (parameter != null)
					{
						base.Add(parameter);
					}
				}
			}
			reader.ReadEndElement();
		}
		public void WriteXml(XmlWriter writer)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Parameter));
			writer.WriteStartAttribute("type");
			writer.WriteString(this.Type);
			writer.WriteEndAttribute();
			foreach (Parameter current in this)
			{
				xmlSerializer.Serialize(writer, current, Map.Namespaces);
			}
		}
		public override string ToString()
		{
			return this.Type;
		}
	}
}
