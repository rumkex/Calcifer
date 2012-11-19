using System;
using System.IO;

namespace Calcifer.Engine.Content.Pipeline
{
    public interface IResourceLoader
    {
        Type Type { get; }
        bool Supports(string name, Stream stream);
    }

    public abstract class ResourceLoader<T>: IResourceLoader where T: IResource
    {
        protected ContentManager Parent { get; private set; }

        protected ResourceLoader(ContentManager parent)
        {
            Parent = parent;
        }

        public abstract T Load(string name, Stream stream);

        public Type Type { get { return typeof (T); } }

        public abstract bool Supports(string name, Stream stream);
    }
}