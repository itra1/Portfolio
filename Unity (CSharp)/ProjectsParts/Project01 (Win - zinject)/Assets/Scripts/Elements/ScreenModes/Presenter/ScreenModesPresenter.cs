using Base.Presenter;
using Zenject;

namespace Elements.ScreenModes.Presenter
{
	public class ScreenModesPresenter : PresenterBase, IScreenModesPresenter
	{
		[Inject]
		private void Initialize()
		{
			SetName("ScreenModes");
		}
	}
}