using System.Collections;
using System.Collections.Generic;
using Calcifer.Engine.Graphics;

namespace Calcifer.Engine.Scenegraph
{
    public class SceneNode: IEnumerable<SceneNode>
    {
        protected LinkedList<SceneNode> Children { get; private set; }
        protected SceneNode Parent { get; private set; }

        public SceneNode(SceneNode parent): this(parent, new LinkedList<SceneNode>())
        {}

        public SceneNode(SceneNode parent, LinkedList<SceneNode> children)
        {
            Parent = parent;
            if (Parent != null) Parent.AddChild(this);
            Children = children;
        }

        public virtual void AcceptPass(RenderPass pass)
        {
            pass.Visit(this);
        }

        public virtual void VisitChildren(RenderPass pass)
        {
            foreach (var node in Children) node.AcceptPass(pass);
        }

        public virtual void BeginRender()
        {
        }

        public virtual void EndRender()
        {
        }

        public virtual void RenderNode()
        {
        }

        protected virtual void Clear()
        {
            var current = Children.First;
            while (current != null)
            {
                current.Value.Clear();
                Children.Remove(current);
                current = current.Next;
            }
        }
        
        public void RemoveChild(SceneNode child)
        {
            child.Clear();
            Children.Remove(child);
        }

        public void AddChild(SceneNode child)
        {
            Children.AddLast(child);
        }

        public IEnumerator<SceneNode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
