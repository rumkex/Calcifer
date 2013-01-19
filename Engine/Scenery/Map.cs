using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Calcifer.Engine.Scenery
{
    [XmlRoot("map")]
    public class Map
    {
        [XmlArray("assets"), XmlArrayItem("asset")]
        public AssetCollection Assets { get; private set; }
        [XmlArray("definitions"), XmlArrayItem("definition")]
        public DefinitionCollection Definitions { get; private set; }
        [XmlArray("instances"), XmlArrayItem("instance")]
        public InstanceCollection Instances { get; private set; }

        public Map()
        {
            Assets = new AssetCollection();
            Definitions = new DefinitionCollection();
            Instances = new InstanceCollection();
        }
    }

    public class AssetCollection: KeyedCollection<string, AssetReference>
    {
        protected override string GetKeyForItem(AssetReference item)
        {
            return item.Name;
        }
    }

    public class DefinitionCollection: KeyedCollection<string, EntityDefinition>
    {
        protected override string GetKeyForItem(EntityDefinition item)
        {
            return item.Name;
        }
    }

    public class InstanceCollection : KeyedCollection<string, EntityInstance>
    {
        protected override string GetKeyForItem(EntityInstance item)
        {
            return item.Name;
        }
    }
}