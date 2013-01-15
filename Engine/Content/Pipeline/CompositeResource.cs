using System.Collections.Generic;
using System.Linq;

namespace Calcifer.Engine.Content.Pipeline
{
    /// <summary>
    /// This datatype is actually a list of different subtypes
    /// </summary>
    public class CompositeResource: List<IResource>, IResource
    {
        public CompositeResource(params IResource[] data): base(data)
        {}

	    public object Clone()
	    {
			var clone = new CompositeResource();
	        clone.AddRange(this.Select(resource => resource.Clone()).Cast<IResource>());
	        return clone;
	    }
    }
}
