using UI.FullScreen.Presenter.Targets.Base;

namespace UI.FullScreen.Presenter
{
    public interface IWindowFullScreenToggle : IWindowFullScreenState
    {
        void Toggle(IFullScreenCapable target);
    }
}