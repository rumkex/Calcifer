using System.Drawing;

namespace Calcifer.UI.Controls
{
    public class Button: UIElement
    {
        private enum ButtonState
        {
            Default = 0,
            Selected = 1
        }

        private ButtonState state;

        public Button(UIElement parent): base(parent)
        {
            FrontColor = Color.Silver;
            BackColor = Color.SlateGray;
            Selectable = true;
        }

        public override string State { get { return state.ToString(); } }

        protected override void OnFocus()
        {
            state = ButtonState.Selected;
        }

        protected override void OnLostFocus()
        {
            state = ButtonState.Default;
        }
    }
}