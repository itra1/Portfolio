using UI.Base.Presenter;
using UnityEngine;

namespace UI.SplashScreens.Screensaver.Presenter.Components
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class ScreensaverBase : HiddenPresenterBase
    {
        public void ForcedShow() => gameObject.SetActive(true);
    }
}