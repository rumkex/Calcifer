using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Calcifer.Engine.Content.Data;
using Calcifer.Engine.Content.Pipeline;
using Calcifer.Utilities.Logging;

namespace Calcifer.Engine.Content
{
    public interface IResource: ICloneable
    {
    }

    public class ContentManager : IDisposable
    {
        private readonly Dictionary<string, LinkedList<IResource>> resourceCache;
        private bool disposed;

        public ContentManager()
        {
            Providers = new ProviderCollection();
            resourceCache = new Dictionary<string, LinkedList<IResource>>();
            Loaders = new List<IResourceLoader>();
            Types = (
                        from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetExportedTypes()
                        where type.GetInterface("IResource") != null
                        select type).ToDictionary(t => t.Name);
        }

        public List<IResourceLoader> Loaders { get; private set; }
        public ProviderCollection Providers { get; private set; }
        public Dictionary<string, Type> Types { get; private set; }

        public void Dispose()
        {
            if (!disposed)
            {
                GC.SuppressFinalize(this);
                disposed = true;
            }
        }

        public object Load(string assetName, Type type)
        {
            var methodInfo = typeof (ContentManager).GetMethod("Load", new[] {typeof(string)}).MakeGenericMethod(new[]{type});
            return methodInfo.Invoke(this, new object[]{assetName});
        }

        public T Load<T>(string assetName) where T : class, IResource
        {
            if (resourceCache.ContainsKey(assetName))
            {
                T cachedResult = resourceCache[assetName].OfType<T>().FirstOrDefault();
                if (cachedResult != null)
                {
                    Log.WriteLine(LogLevel.Debug, "instantiated '{0}'", assetName);
	                var clone = cachedResult.Clone() as T;
	                return clone;
                }
            }
            Stream stream = Providers.LoadAsset(assetName);
            if (stream == null)
            {
                Log.WriteLine(LogLevel.Error, "'{0}' was not found", assetName);
                return default(T);
            }
            foreach (IResourceLoader loader in Loaders)
            {
                if (loader == null || loader.Type != typeof (T)) continue;
                bool supports = loader.Supports(assetName, stream);
                if (stream.CanSeek)
                    stream.Seek(0L, SeekOrigin.Begin);
                else
                {
                    stream.Close();
                    stream = Providers.LoadAsset(assetName);
                }
                if (!supports) continue;
                var resourceLoader = loader as ResourceLoader<T>;
                if (resourceLoader != null)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    T resource = resourceLoader.Load(assetName, stream);
                    stopwatch.Stop();
                    if (resource != null)
                    {
                        if (!resourceCache.ContainsKey(assetName))
                            resourceCache.Add(assetName, new LinkedList<IResource>());
                        resourceCache[assetName].AddLast(resource);
                        Log.WriteLine(LogLevel.Debug, "loaded '{0}': {1} ms", assetName, stopwatch.ElapsedMilliseconds);
                        stream.Close();
                        return resource;
                    }
                }
            }
            stream.Close();
            Log.WriteLine(LogLevel.Error, "loader not found: '{0}'", assetName);
            return default(T);
        }

        public void Unload(string assetName)
        {
            if (resourceCache.ContainsKey(assetName))
                resourceCache.Remove(assetName);
        }

        private void Dispose(bool manual)
        {
        }

        ~ContentManager()
        {
            Dispose(false);
        }
    }
}