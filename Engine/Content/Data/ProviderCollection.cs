using System;
using System.Collections.Generic;
using System.IO;

namespace Calcifer.Engine.Content.Data
{
    public class ProviderCollection
    {
        private sealed class DirectoryNode
        {
            private readonly ProviderCollection.DirectoryNode parent;
            private Dictionary<string, ProviderCollection.DirectoryNode> children;
            public string Name
            {
                get;
                set;
            }
            public Dictionary<string, AssetEntry> Files
            {
                get;
                private set;
            }
            public string FullName
            {
                get
                {
                    if (this.parent == null)
                    {
                        return "";
                    }
                    return (this.parent.parent != null) ? (this.parent.FullName + "/" + this.Name) : this.Name;
                }
            }
            public DirectoryNode(ProviderCollection.DirectoryNode parent, string name)
            {
                this.parent = parent;
                this.Name = name;
                this.Files = new Dictionary<string, AssetEntry>();
                this.children = new Dictionary<string, ProviderCollection.DirectoryNode>();
            }
            public void Add(string directory)
            {
                this.children.Add(directory, new ProviderCollection.DirectoryNode(this, directory));
            }
            public ProviderCollection.DirectoryNode GetDirectory(string directory)
            {
                return this.children[directory];
            }
            public bool HasDirectory(string directory)
            {
                return this.children.ContainsKey(directory);
            }
        }
        private readonly ProviderCollection.DirectoryNode root;
        public ProviderCollection()
        {
            this.root = new ProviderCollection.DirectoryNode(null, "");
        }
        public void Add(IContentProvider provider)
        {
            char[] separator = new char[]
                                   {
                                       '\\',
                                       '/'
                                   };
            foreach (AssetEntry current in provider.GetAssets())
            {
                string[] array = current.FullName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                ProviderCollection.DirectoryNode directory = this.root;
                for (int i = 0; i < array.Length - 1; i++)
                {
                    if (!directory.HasDirectory(array[i]))
                    {
                        directory.Add(array[i]);
                    }
                    directory = directory.GetDirectory(array[i]);
                }
                if (directory.Files.ContainsKey(current.Name))
                {
                    directory.Files[current.Name] = current;
                }
                else
                {
                    directory.Files.Add(current.Name, current);
                }
            }
        }
        public IEnumerable<string> ListAssets(string directory)
        {
            return new string[0];
        }
        public Stream LoadAsset(string assetName)
        {
            char[] separator = new char[]
                                   {
                                       '\\',
                                       '/'
                                   };
            string[] array = assetName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            ProviderCollection.DirectoryNode directory = this.root;
            string key = array[array.Length - 1];
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (!directory.HasDirectory(array[i]))
                {
                    throw new DirectoryNotFoundException(string.Format("Directory not found: {0}", directory.FullName));
                }
                directory = directory.GetDirectory(array[i]);
            }
            if (!directory.Files.ContainsKey(key))
            {
                throw new FileNotFoundException(string.Format("File not found: {0}", assetName));
            }
            return directory.Files[key].Load();
        }
    }
}