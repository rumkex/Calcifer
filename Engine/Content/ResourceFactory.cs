using System.Collections.Generic;
using System.Linq;
using Calcifer.Engine.Content.Pipeline;
using Calcifer.Engine.Scenery;

namespace Calcifer.Engine.Content
{
    public static class ResourceFactory
    {
        private static Dictionary<string, AssetReference> assets = new Dictionary<string, AssetReference>();
        private static ContentManager content;
        public static void SetManager(ContentManager c)
        {
            content = c;
        }

        public static void AddAssetRange(IEnumerable<AssetReference> list)
        {
            foreach (var asset in list)
            {
                AddAsset(asset);
            }
        }

        public static void AddAsset(AssetReference asset)
        {
            assets.Add(asset.Name, asset);
        }

        public static T LoadAsset<T>(string alias) where T: class, IResource
        {
            var r = assets[alias];
            return r.Composite
                       ? content.Load<CompositeResource>(r.Source).OfType<T>().FirstOrDefault()
                       : content.Load<T>(r.Source);
        }

        public static T LoadFile<T>(string name) where T : class, IResource
        {
            return content.Load<T>(name);
        }
    }
}
