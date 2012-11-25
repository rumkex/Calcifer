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
				var xmlSerializerNamespaces = new XmlSerializerNamespaces();
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
			var assetSerializer = new XmlSerializer(typeof(AssetReference));
			var entitySerializer = new XmlSerializer(typeof(EntityDefinition));
			if (reader.MoveToContent() != XmlNodeType.Element || reader.LocalName != "map")
			{
				return;
			}
			if (reader.ReadToDescendant("asset"))
			{
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "asset")
				{
				    var assetReference = assetSerializer.Deserialize(reader) as AssetReference;
				    if (assetReference != null) Assets.Add(assetReference.Name, assetReference);
				}
			    while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "entity")
			    {
			        var entityDefinition = entitySerializer.Deserialize(reader) as EntityDefinition;
			        if (entityDefinition != null) Entities.Add(entityDefinition.Name, entityDefinition);
			    }
			}
			reader.ReadEndElement();
		}
		public void WriteXml(XmlWriter writer)
		{
			var assetSerializer = new XmlSerializer(typeof(AssetReference));
			var entitySerializer = new XmlSerializer(typeof(EntityDefinition));
			foreach (var assetEntry in Assets)
			{
				assetSerializer.Serialize(writer, assetEntry.Value, Map.Namespaces);
			}
			foreach (var entityEntry in Entities)
			{
				entitySerializer.Serialize(writer, entityEntry.Value, Map.Namespaces);
			}
		}
	}
}
