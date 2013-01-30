using System.Collections.Generic;
using System.Drawing;

namespace Calcifer.UI
{
    public abstract class UIElement
    {
        private UIElement parent;
        private bool validated;
        private Rectangle bounds;

        public LinkedList<UIElement> Children { get; private set; }
        public Padding Padding { get; private set; }
        public bool Visible { get; set; }
        public bool Selectable { get; set; }
        public Color BackColor { get; set; }
        public Color FrontColor { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Style { get; set; }

        protected UIElement(UIElement parent = null)
        {
            Padding = new Padding();
            Padding.Left = Padding.Right = Padding.Top = Padding.Bottom = 3;
            Children = new LinkedList<UIElement>();
            Style = GetType().Name;
            Parent = parent;
            Text = "";
        }

        public virtual string State
        {
            get { return "Default"; }
        }

        public Rectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        public int Width { get { return Bounds.Width; } set { bounds.Width = value; } }
        public int Height { get { return Bounds.Height; } set { bounds.Height = value; } }

        public Size Size
        {
            get { return new Size(Width, Height); }
            set { bounds.Width = value.Width; bounds.Height = value.Height; }
        }

        public Point Position
        {
            get { return new Point(Bounds.X, Bounds.Y); }
            set { bounds.X = value.X; bounds.Y = value.Y; }
        }

        public int PaddedWidth { get { return Width - Padding.Width; } }
        public int PaddedHeight { get { return Height - Padding.Height; } }
        public Size PaddedSize { get { return new Size(PaddedWidth, PaddedHeight); } }
        public Point PaddedPosition { get { return Position + new Size(Padding.Left, Padding.Top); } }

        
        public UIElement Parent
        {
            get { return parent; }
            set
            {
                if (parent != value && parent != null)
                {
                    parent.RemoveChild(this);
                }
                parent = value;
                if (parent != null)
                {
                    parent.AddChild(this);
                }
            }
        }

        private void AddChild(UIElement element)
        {
            Children.AddLast(element);
            Invalidate();
        }

        private void RemoveChild(UIElement element)
        {
            Children.Remove(element);
            Invalidate();
        }

        public void Invalidate()
        {
            validated = false;
        }

        protected virtual void Render(IRenderer renderer)
        {
            renderer.Render(this);
        }

        /// <summary>
        /// This method is triggered during rendering by invalidation: for example, when children are added/removed
        /// </summary>
        protected virtual void Layout()
        {}

        internal virtual void DoRender(IRenderer renderer)
        {
            if (!validated)
            {
                Layout();
                validated = true;
            }
            Render(renderer);
            foreach (var element in Children)
            {
                element.DoRender(renderer);
            }
        }

        public virtual void Input(InputKey key)
        {}

        protected virtual void OnFocus()
        { }

        protected virtual void OnLostFocus()
        { }

        internal void Focus()
        {
            OnFocus();
        }

        internal void Unfocus()
        {
            OnLostFocus();
        }

    }
}