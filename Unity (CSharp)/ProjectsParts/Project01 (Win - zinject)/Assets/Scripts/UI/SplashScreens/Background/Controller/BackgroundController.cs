using System;
using Elements.ScreenModes.Controller;
using UI.Canvas.Presenter;
using UI.SplashScreens.Background.Presenter;

namespace UI.SplashScreens.Background.Controller
{
	public class BackgroundController : IBackgroundController, IDisposable
	{
		private IBackgroundPresenter _presenter;
		
		public BackgroundController(ICanvasPresenter root, IScreenMode screenMode)
		{
			_presenter = root.Background;
			_presenter.Initialize(screenMode);
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