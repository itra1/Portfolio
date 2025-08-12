using Base.Presenter;
using Zenject;

namespace Elements.Statuses.Presenter
{
	public class StatusesPresenter : PresenterBase, IStatusesPresenter
	{
		[Inject]
		private void Initialize()
		{
			SetName("Statuses");
		}
	}
}