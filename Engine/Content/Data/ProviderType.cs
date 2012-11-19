using System;

namespace Calcifer.Engine.Content.Data
{
    public class ProviderType : Attribute
    {
        public string Name
        {
            get;
            private set;
        }
        public ProviderType(string name)
        {
            this.Name = name;
        }
    }
}