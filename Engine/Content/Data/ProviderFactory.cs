using System;
using System.Collections.Generic;
using System.Reflection;

namespace Calcifer.Engine.Content.Data
{
    internal sealed class ProviderFactory
    {
        private static Dictionary<string, Type> providers;
        private static ProviderFactory instance;
        public static ProviderFactory Instance
        {
            get
            {
                return instance ?? (instance = new ProviderFactory());
            }
        }

        private ProviderFactory()
        {
            providers = new Dictionary<string, Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    var customAttributes = type.GetCustomAttributes(false);
                    foreach (var attr in customAttributes)
                    {
                        if (attr.GetType() != typeof (ProviderType)) continue;
                        var providerType = attr as ProviderType;
                        if (providerType != null)
                            ProviderFactory.providers.Add(providerType.Name, type);
                        break;
                    }
                }
            }
        }
        public IContentProvider Create(string type)
        {
            return Activator.CreateInstance(ProviderFactory.providers[type]) as IContentProvider;
        }
    }
}