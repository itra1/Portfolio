using Game.Providers.Ui.Presenters.Base;

namespace Game.Providers.Ui.Controllers.Base
{
	public interface IWindowPresenterController : IWindowPresenterControllerBase
	{
		IWindowPresenter WindowPresenter { get; }
	}
}