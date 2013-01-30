namespace Calcifer.UI
{
    public interface IRenderer
    {
        void Begin();
        void End();
        void Render(UIElement element);
        void SetProperty<T>(string name, T value);
    }
}
