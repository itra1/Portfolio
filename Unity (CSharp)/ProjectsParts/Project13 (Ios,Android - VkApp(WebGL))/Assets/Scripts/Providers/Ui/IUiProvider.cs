using Cysharp.Threading.Tasks;
using Game.Providers.Ui.Controllers.Base;
using Game.Providers.Ui.Presenters.Base;

namespace Game.Providers.Ui
{
	public interface IUiProvider
	{
		UniTask<IWindowPresenter> OpenWindow(string windowName);
		T GetController<T>();
		UniTask CloseAll();
		void AddOrder(WindowPresenterControllerBase controller);
		void RemoveOrder(WindowPresenterControllerBase controller);
	}
}