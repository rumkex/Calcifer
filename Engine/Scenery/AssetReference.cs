using System.ComponentModel;
using System.Xml.Serialization;

namespace Calcifer.Engine.Scenery
{
	[XmlRoot("asset", Namespace = "urn:Map")]
	public class AssetReference
	{
		[XmlAttribute("alias")]
		public string Name;
		[XmlAttribute("type")]
		public string Type;
		[DefaultValue(false), XmlAttribute("composite")]
		public bool Composite;
		[XmlAttribute("source")]
		public string Source;
		public override string ToString()
		{
			return this.Type + " from " + this.Source;
		}
	}
}
