﻿using System.Collections.Generic;

namespace Calcifer.Engine.Content.Pipeline
{
    /// <summary>
    /// This datatype is actually a list of different subtypes
    /// </summary>
    public class CompositeResource: List<IResource>, IResource
    {
        public CompositeResource(params IResource[] data): base(data)
        {}
    }
}
