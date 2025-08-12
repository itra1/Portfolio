using Base.Presenter;
using Zenject;

namespace Elements.Presentations.Presenter
{
	public class PresentationsPresenter : PresenterBase, IPresentationsPresenter
	{
		[Inject]
		private void Initialize()
		{
			SetName("Presentations");
		}
	}
}