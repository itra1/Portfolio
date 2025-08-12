using System;
using UI.Canvas.Presenter;
using UI.SplashScreens.Intro.Presenter;

namespace UI.SplashScreens.Intro.Controller
{
    public class IntroController : IIntroController, IDisposable
    {
        private IIntroPresenter _presenter;
        
        public IntroController(ICanvasPresenter root)
        {
            _presenter = root.Intro;
        }

        public void Dispose()
        {
            if (_presenter == null)
                return;
			
            _presenter.Unload();
            _presenter = null;
        }
    }
}