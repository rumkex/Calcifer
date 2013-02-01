using System;
using System.Collections.Generic;

namespace Calcifer.UI
{
    public class Canvas : UIElement
    {
        private readonly IRenderer renderer;
        private readonly Stack<LinkedListNode<UIElement>> selectStack;

        public float KeyRepeatInterval { get; set; }

        public Canvas(IRenderer renderer)
        {
            this.renderer = renderer;
            KeyRepeatInterval = 0.1f;
            selectStack = new Stack<LinkedListNode<UIElement>>();
            cooldowns = new Dictionary<InputKey, float>();
            Style = "None";
        }

        public void Render()
        {
            renderer.Begin();
            DoRender(renderer);
            renderer.End();
        }

        public void Tick(float time)
        {
            DoUpdate(time);
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

        protected override void Update(float time)
        {
            foreach (var key in cooldowns.Keys)
                if (cooldowns[key] > 0)
                    cooldowns[key] -= time;
        }

        private Dictionary<InputKey, float> cooldowns; 

        public void AcceptInput(InputKey key)
        {
            if (!cooldowns.ContainsKey(key)) cooldowns.Add(key, 0f);
            if (cooldowns[key] > 0f) return;
            cooldowns[key] = KeyRepeatInterval;
            var focus = (selectStack.Count > 0) ? selectStack.Peek().Value: null;
            if (key == InputKey.Up || key == InputKey.Down)
            {
                if (focus != null) focus.Unfocus();
                var elem = Select(key == InputKey.Down ? Direction.Next: Direction.Previous);
                elem.Focus();
                return;
            }
            if (focus != null) focus.Input(key);
        }
    }
}
