using UI.FullScreen.Presenter.Targets.Base;

namespace UI.FullScreen.Presenter
{
    public interface IWindowFullScreenState
    {
        bool IsInFullScreenMode(IFullScreenCapable target);
    }
}