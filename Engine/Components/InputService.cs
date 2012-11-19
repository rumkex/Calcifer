using OpenTK;
using OpenTK.Input;

namespace Calcifer.Engine.Components
{
    class InputService//: Service
    {
        public KeyboardDevice Keyboard { get; private set; }
        public MouseDevice Mouse { get; private set; }

        private GameWindow host;

        public InputService(GameWindow window)
        {
            host = window;
            Keyboard = host.Keyboard;
            Mouse = host.Mouse;
        }
    }
}
