using System;
using System.IO;

namespace Calcifer.Engine.Content.Data
{
    public class AssetEntry
    {
        private readonly Func<Stream> loadFunc;
        public string FullName
        {
            get;
            set;
        }
        public string Name
        {
            get
            {
                return Path.GetFileName(this.FullName);
            }
        }
        public AssetEntry(string name, Func<Stream> loadFunc)
        {
            this.loadFunc = loadFunc;
            this.FullName = name.Replace('\\', '/');
        }
        public Stream Load()
        {
            return this.loadFunc();
        }
    }
}