namespace Calcifer.UI
{
    public class Padding
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Width { get { return Left + Right; } }
        public int Height { get { return Top + Bottom; } }

        public void Set(int value)
        {
            Top = Bottom = Left = Right = value;
        }
    }
}
