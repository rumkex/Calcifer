using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Calcifer.Engine.Scenery
{
	[XmlRoot("entity", Namespace = "urn:Map")]
	public class EntityDefinition : Dictionary<string, ComponentDefinition>, IXmlSerializable
	{
		public string Name
		{
			get;
			set;
		}
		public EntityDefinition()
		{
		}
		public EntityDefinition(string name, params ComponentDefinition[] components)
		{
			this.Name = name;
			for (int i = 0; i < components.Length; i++)
			{
				ComponentDefinition componentDefinition = components[i];
				base.Add(componentDefinition.Type, componentDefinition);
			}
		}
		public XmlSchema GetSchema()
		{
			return null;
		}
		public void ReadXml(XmlReader reader)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ComponentDefinition));
			if (reader.MoveToContent() != XmlNodeType.Element || reader.LocalName != "entity")
			{
				return;
			}
			this.Name = reader.GetAttribute("name");
			if (reader.ReadToDescendant("component"))
			{
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "component")
				{
					ComponentDefinition componentDefinition = xmlSerializer.Deserialize(reader) as ComponentDefinition;
					base.Add(componentDefinition.Type, componentDefinition);
				}
			}
			reader.ReadEndElement();
		}
		public void WriteXml(XmlWriter writer)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ComponentDefinition));
			writer.WriteStartAttribute("name");
			writer.WriteString(this.Name);
			writer.WriteEndAttribute();
			foreach (KeyValuePair<string, ComponentDefinition> current in this)
			{
				xmlSerializer.Serialize(writer, current.Value, Map.Namespaces);
			}
		}
		public override string ToString()
		{
			return this.Name;
		}
	}
}
