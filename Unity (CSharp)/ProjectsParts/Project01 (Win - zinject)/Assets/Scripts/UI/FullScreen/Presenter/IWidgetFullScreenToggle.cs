using Core.Materials.Data;

namespace UI.FullScreen.Presenter
{
    public interface IWidgetFullScreenToggle : IWidgetFullScreenEventDispatcher
    {
        void Toggle(MaterialData material);
    }
}