using System.Drawing;

namespace Calcifer.UI.Layouts
{
    public class GridLayout: UIElement
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public bool ForceSize { get; set; }
        public Size Margin { get; set; }

        public GridLayout(UIElement parent) : base(parent)
        {
            Bounds = parent.Bounds;
            Rows = 4;
            Columns = 4;
            Style = "None";
        }

        protected override void Layout()
        {
            var sizeX = (Width - (Columns + 1)*Margin.Width) / Columns;
            var sizeY = (Height - (Rows + 1)*Margin.Height) / Rows;
            var child = Children.First;
            for (var row = 0; row < Rows; row++)
                for (var col = 0; col < Columns; col++)
                {
                    if (child == null) break;
                    var x = Position.X + Margin.Width * (col + 1) + sizeX * col;
                    var y = Position.Y + Margin.Height * (row + 1) + sizeY * row;
                    child.Value.Position = new Point(x, y);
                    if (ForceSize) child.Value.Size = new Size(sizeX, sizeY);
                    child = child.Next;
                }
        }
    }
}
