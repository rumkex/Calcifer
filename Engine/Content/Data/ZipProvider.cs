using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace Calcifer.Engine.Content.Data
{
    [ProviderType("zip")]
    public class ZipProvider : IContentProvider
    {
        private ZipFile root;
        public void Bind(string path)
        {
            this.root = new ZipFile(path);
        }
        public IEnumerable<AssetEntry> GetAssets()
        {
            return 
                from ZipEntry entry in this.root
                where entry.IsFile
                select new AssetEntry(entry.Name, () => this.root.GetInputStream(entry));
        }
    }
}