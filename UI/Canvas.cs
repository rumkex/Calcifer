using System;
using System.Collections.Generic;

namespace Calcifer.UI
{
    public class Canvas : UIElement
    {
        private readonly IRenderer renderer;
        private readonly Stack<LinkedListNode<UIElement>> selectStack; 

        public Canvas(IRenderer renderer)
        {
            this.renderer = renderer;
            selectStack = new Stack<LinkedListNode<UIElement>>();
            Style = "None";
        }

        public void Render()
        {
            renderer.Begin();
            DoRender(renderer);
            renderer.End();
        }

        private enum Direction
        {
            Next = 0,
            Previous = 1
        }

        private UIElement Select(Direction d)
        {
            var first = new Func<UIElement, LinkedListNode<UIElement>>(c => d == Direction.Next ? c.Children.First : c.Children.Last);
            var next = new Func<LinkedListNode<UIElement>, LinkedListNode<UIElement>>(c => d == Direction.Next ? c.Next: c.Previous);
            LinkedListNode<UIElement> current;
            if (selectStack.Count == 0)
                current = first(this);
            else
            {
                current = selectStack.Pop();
                if (current.Value.Children.Count > 0)
                {
                    selectStack.Push(current);
                    current = first(current.Value);
                }
                else
                {
                    while (next(current) == null && selectStack.Count > 0) current = selectStack.Pop();
                    if (next(current) != null) current = next(current);
                }
            }
            selectStack.Push(current);
            return current.Value.Selectable ? current.Value: Select(d);
        }


        public void AcceptInput(InputKey key)
        {
            var focus = (selectStack.Count > 0) ? selectStack.Peek().Value: null;
            if (key == InputKey.Up || key == InputKey.Down)
            {
                if (focus != null) focus.Unfocus();
                var elem = Select(key == InputKey.Up ? Direction.Next: Direction.Previous);
                elem.Focus();
                return;
            }
            if (focus != null) focus.Input(key);
        }
    }
}
