using System.Xml.Serialization;

namespace Calcifer.Engine.Scenery
{
	[XmlRoot("param", Namespace = "urn:Map")]
	public class Parameter
	{
		[XmlAttribute("name")]
		public string Name;
		[XmlText]
		public string Value;
		public Parameter()
		{
		}
		public Parameter(string name, string value)
		{
			this.Name = name;
			this.Value = value;
		}
	}
}
