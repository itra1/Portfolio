using Base.Presenter;
using Zenject;

namespace Elements.Desktops.Presenter
{
	public class DesktopsPresenter : PresenterBase, IDesktopsPresenter
	{
		[Inject]
		private void Initialize()
		{
			SetName("Desktops");
		}
	}
}