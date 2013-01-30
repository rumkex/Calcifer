using System.Drawing;

namespace Calcifer.UI.Controls
{
    public class Bar: UIElement
    {
        public int Value { get; set; }
        public int MaxValue { get; set; }

        public Bar(UIElement parent) : base(parent)
        {
            FrontColor = Color.GreenYellow;
            BackColor = Color.Silver;
        }

        protected override void Render(IRenderer renderer)
        {
            renderer.SetProperty("Value", Value);
            renderer.SetProperty("MaxValue", MaxValue);
            base.Render(renderer);
        }
    }
}
