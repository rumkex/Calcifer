using System;
using System.Collections.Generic;
using System.IO;

namespace Calcifer.Engine.Content.Data
{
    public class ProviderCollection
    {
        private readonly DirectoryNode root;

        public ProviderCollection()
        {
            root = new DirectoryNode(null, "");
        }

        public void Add(IContentProvider provider)
        {
            var separator = new[]{'\\', '/'};
            foreach (AssetEntry current in provider.GetAssets())
            {
                string[] array = current.FullName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                DirectoryNode directory = root;
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

        public Stream LoadAsset(string assetName)
        {
            var separator = new[] { '\\', '/' };
            string[] array = assetName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            DirectoryNode directory = root;
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

        private sealed class DirectoryNode
        {
            private readonly Dictionary<string, DirectoryNode> children;
            private readonly DirectoryNode parent;

            public DirectoryNode(DirectoryNode parent, string name)
            {
                this.parent = parent;
                Name = name;
                Files = new Dictionary<string, AssetEntry>();
                children = new Dictionary<string, DirectoryNode>();
            }

            public string Name { get; set; }
            public Dictionary<string, AssetEntry> Files { get; private set; }

            public string FullName
            {
                get
                {
                    if (parent == null)
                    {
                        return "";
                    }
                    return (parent.parent != null) ? (parent.FullName + "/" + Name) : Name;
                }
            }

            public void Add(string directory)
            {
                children.Add(directory, new DirectoryNode(this, directory));
            }

            public DirectoryNode GetDirectory(string directory)
            {
                return children[directory];
            }

            public bool HasDirectory(string directory)
            {
                return children.ContainsKey(directory);
            }
        }
    }
}