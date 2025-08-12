using Base;
using Elements.ScreenModes.Controller;

namespace UI.SplashScreens.Background.Presenter
{
	public interface IBackgroundPresenter : IUnloadable
	{
		void Initialize(IScreenMode screenMode);
	}
}