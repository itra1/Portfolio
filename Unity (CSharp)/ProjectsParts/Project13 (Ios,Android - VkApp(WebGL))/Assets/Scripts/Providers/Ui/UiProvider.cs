using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Providers.Ui.Controllers.Base;
using Game.Providers.Ui.Controllers.Factorys;
using Game.Providers.Ui.Presenters.Base;

namespace Game.Providers.Ui
{
	public class UiProvider : IUiProvider
	{
		private readonly IWindowPresenterControllerFactory _presentersFactory;

		private readonly List<WindowPresenterControllerBase> _presenterBaseList = new();

		public UiProvider(IWindowPresenterControllerFactory presentersFactory)
		{
			_presentersFactory = presentersFactory;
		}

		public void AddOrder(WindowPresenterControllerBase controller)
		{
			if (_presenterBaseList.Contains(controller))
				return;
			_presenterBaseList.Add(controller);
		}

		public void RemoveOrder(WindowPresenterControllerBase controller)
		{
			_ = _presenterBaseList.Remove(controller);
		}

		public async UniTask<IWindowPresenter> OpenWindow(string windowName)
		{
			var instance = _presentersFactory.GetInstance(windowName);

			return await instance.Show(null) ? instance.WindowPresenter : null;
		}

		public T GetController<T>()
		{
			return _presentersFactory.GetInstance<T>();
		}

		public async UniTask CloseAll()
		{
			while (_presenterBaseList.Count > 0)
				_ = await _presenterBaseList[0].Hide();
		}
	}
}
