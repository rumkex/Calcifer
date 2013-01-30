using System.Drawing;

namespace Calcifer.UI.Controls
{
    public class Frame: UIElement
    {
        public Frame(UIElement parent) : base(parent)
        {
            BackColor = Color.Gray;
        }
    }
}
