using Calcifer.Engine.Content.Data;
using Calcifer.Engine.Content.Pipeline;
using Calcifer.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
namespace Calcifer.Engine.Content
{
    public interface IResource
    {
    }

	public class ContentManager : IDisposable
	{
		private readonly Dictionary<string, LinkedList<IResource>> resourceCache;
		private bool disposed;

		public List<IResourceLoader> Loaders
		{
			get;
			private set;
		}
		public ProviderCollection Providers
		{
			get;
			private set;
		}
		public Dictionary<string, Type> Types
		{
			get;
			private set;
		}

		public ContentManager()
		{
			this.Providers = new ProviderCollection();
			this.resourceCache = new Dictionary<string, LinkedList<IResource>>();
			this.Loaders = new List<IResourceLoader>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
		    this.Types = (
		                     from assembly in assemblies
		                     from type in assembly.GetExportedTypes()
		                     where type.GetInterface("IResource") != null
                             select type).ToDictionary((Type t) => t.Name);
		}

		public object Load(string assetName, Type type)
		{
			MethodInfo methodInfo = typeof(ContentManager).GetMethod("Load", new Type[]
			{
				typeof(string)
			}).MakeGenericMethod(new Type[]
			{
				type
			});
			return methodInfo.Invoke(this, new object[]
			{
				assetName
			});
		}

		public T Load<T>(string assetName) where T : class, IResource
		{
			if (this.resourceCache.ContainsKey(assetName))
			{
			    var cachedResult = this.resourceCache[assetName].OfType<T>().FirstOrDefault();
                if (cachedResult != null)
                {
					Log.WriteLine(LogLevel.Debug, "instantiated '{0}'", assetName);
                    return cachedResult;
				}
			}
			var stream = this.Providers.LoadAsset(assetName);
			if (stream == null)
			{
				Log.WriteLine(LogLevel.Error, "'{0}' was not found", assetName);
				return default(T);
			}
			foreach (var loader in Loaders)
			{
			    if (loader == null || loader.Type != typeof (T)) continue;
			    var supports = loader.Supports(assetName, stream);
			    if (stream.CanSeek)
			        stream.Seek(0L, SeekOrigin.Begin);
			    else
			    {
			        stream.Close();
			        stream = this.Providers.LoadAsset(assetName);
			    }
			    if (!supports) continue;
			    var resourceLoader = loader as ResourceLoader<T>;
			    if (resourceLoader != null)
			    {
			        var stopwatch = new Stopwatch();
			        stopwatch.Start();
			        var resource = resourceLoader.Load(assetName, stream);
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
		        this.resourceCache.Remove(assetName);
		}

	    public void Dispose()
		{
	        if (!disposed)
	        {
	            GC.SuppressFinalize(this);
	            this.disposed = true;
	        }
		}

		private void Dispose(bool manual)
		{
		}

		~ContentManager()
		{
			this.Dispose(false);
		}
	}
}
