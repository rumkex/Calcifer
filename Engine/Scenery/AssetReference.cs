using System.ComponentModel;
using System.Xml.Serialization;

namespace Calcifer.Engine.Scenery
{
	[XmlRoot("asset")]
	public class AssetReference
	{
		[XmlAttribute("alias")]
		public string Name;
		[DefaultValue(false), XmlAttribute("composite")]
		public bool Composite;
		[XmlAttribute("source")]
		public string Source;
		public override string ToString()
		{
			return Name;
		}
	}
}
