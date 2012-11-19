using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Calcifer.Engine.Scenery
{
	[XmlRoot("map", Namespace = "urn:Map")]
	public class Map : IXmlSerializable
	{
		public Dictionary<string, AssetReference> Assets
		{
			get;
			set;
		}
		public Dictionary<string, EntityDefinition> Entities
		{
			get;
			set;
		}
		public static XmlSerializerNamespaces Namespaces
		{
			get
			{
				XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
				xmlSerializerNamespaces.Add("", "urn:Map");
				return xmlSerializerNamespaces;
			}
		}
		public Map()
		{
			this.Assets = new Dictionary<string, AssetReference>();
			this.Entities = new Dictionary<string, EntityDefinition>();
		}
		public XmlSchema GetSchema()
		{
			return null;
		}
		public void ReadXml(XmlReader reader)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AssetReference));
			XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(EntityDefinition));
			if (reader.MoveToContent() != XmlNodeType.Element || reader.LocalName != "map")
			{
				return;
			}
			if (reader.ReadToDescendant("asset"))
			{
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "asset")
				{
					AssetReference assetReference = xmlSerializer.Deserialize(reader) as AssetReference;
					this.Assets.Add(assetReference.Name, assetReference);
				}
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "entity")
				{
					EntityDefinition entityDefinition = xmlSerializer2.Deserialize(reader) as EntityDefinition;
					this.Entities.Add(entityDefinition.Name, entityDefinition);
				}
			}
			reader.ReadEndElement();
		}
		public void WriteXml(XmlWriter writer)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AssetReference));
			XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(EntityDefinition));
			foreach (KeyValuePair<string, AssetReference> current in this.Assets)
			{
				xmlSerializer.Serialize(writer, current.Value, Map.Namespaces);
			}
			foreach (KeyValuePair<string, EntityDefinition> current2 in this.Entities)
			{
				xmlSerializer2.Serialize(writer, current2.Value, Map.Namespaces);
			}
		}
	}
}
