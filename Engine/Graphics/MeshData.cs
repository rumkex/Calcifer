﻿using System.Collections.Generic;
using System.Linq;
using Calcifer.Engine.Content;
using ComponentKit.Model;

namespace Calcifer.Engine.Graphics
{
    // TODO: Make MeshComponent (RenderComponent) a separate entity
    public class MeshData : Component, IResource
    {
        public List<Geometry> Submeshes { get; private set; }

        public MeshData(IEnumerable<Geometry> geometry)
        {
            Submeshes = geometry.ToList();
        }

	    private MeshData(MeshData geometry)
	    {
		    Submeshes = geometry.Submeshes;
	    }

	    public object Clone()
	    {
		    return new MeshData(this);
	    }
    }
}