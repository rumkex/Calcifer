using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Calcifer.Engine.Content.Data
{
    [ProviderType("fs")]
    public class FilesystemProvider : IContentProvider
    {
        private string root = ".";
        public FilesystemProvider(string root)
        {
            this.root = root;
        }
        public IEnumerable<AssetEntry> GetAssets()
        {
            return 
                from file in Directory.GetFiles(this.root, "*", SearchOption.AllDirectories)
                select new AssetEntry(file.Remove(0, this.root.Length + 1), () => File.OpenRead(file));
        }
    }
}