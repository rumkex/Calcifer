using System;
using System.Collections.Generic;

namespace Calcifer.Engine.Scenery
{
	public class MapBuilder
	{
		private Map map;
		private Stack<EntityDefinition> entityStack;
		private Stack<ComponentDefinition> componentStack;
		public MapBuilder()
		{
			this.map = new Map();
			this.entityStack = new Stack<EntityDefinition>();
			this.componentStack = new Stack<ComponentDefinition>();
		}
		public void BeginEntity(string name)
		{
			this.entityStack.Push(new EntityDefinition(name, new ComponentDefinition[0]));
		}
		public void EndEntity()
		{
			if (this.entityStack.Count == 0)
			{
				throw new InvalidOperationException("EndEntity called before BeginEntity");
			}
			EntityDefinition entityDefinition = this.entityStack.Pop();
			this.map.Entities.Add(entityDefinition.Name, entityDefinition);
		}
		public void BeginComponent(string type)
		{
			this.componentStack.Push(new ComponentDefinition(type, new Parameter[0]));
		}
		public void EndComponent()
		{
			if (this.componentStack.Count == 0)
			{
				throw new InvalidOperationException("EndComponent called before BeginComponent");
			}
			ComponentDefinition componentDefinition = this.componentStack.Pop();
			this.entityStack.Peek().Add(componentDefinition.Type, componentDefinition);
		}
		public void AddParameter(string name, string value)
		{
			if (this.componentStack.Count == 0)
			{
				throw new Exception("AddParameter has no component to add parameter to");
			}
			this.componentStack.Peek().Add(new Parameter(name, value));
		}
		public void AddAsset(string alias, string type, bool composite, string source)
		{
			AssetReference value = new AssetReference
			{
				Name = alias,
				Type = type,
				Composite = composite,
				Source = source
			};
			if (!this.map.Assets.ContainsKey(alias))
			{
				this.map.Assets.Add(alias, value);
			}
		}
		public Map GetMap()
		{
			if (this.componentStack.Count != 0)
			{
				throw new InvalidOperationException("Component stack not empty.");
			}
			if (this.entityStack.Count != 0)
			{
				throw new InvalidOperationException("Entity stack not empty.");
			}
			return this.map;
		}
	}
}
