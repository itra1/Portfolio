using Base;

namespace UI.SplashScreens.Screensaver.Presenter.Components
{
    public interface IScreensaver : IVisualAsync, IUnloadable
    {
        void TogglePlayPause();
        void StopIfPlaying();
    }
}