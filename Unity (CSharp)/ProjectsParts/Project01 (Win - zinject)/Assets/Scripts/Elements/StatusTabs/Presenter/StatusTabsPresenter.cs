using Base.Presenter;
using Zenject;

namespace Elements.StatusTabs.Presenter
{
	public class StatusTabsPresenter : PresenterBase, IStatusTabsPresenter
	{
		[Inject]
		private void Initialize()
		{
			SetName("Tabs");
		}
	}
}