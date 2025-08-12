using Game.Scripts.UI.Presenters.Base;

namespace Game.Scripts.UI.Controllers.Base
{
	public interface IWindowPresenterController : IWindowPresenterControllerBase
	{
		IWindowPresenter WindowPresenter { get; }
		bool AddInNavigationStack { get; }
	}
}
