using Cysharp.Threading.Tasks;
using Game.Providers.Ui.Presenters.Factorys;
using Zenject;

namespace Game.Providers.Ui.Controllers.Base
{
	public class WindowPresenterControllerBase : IWindowPresenterControllerBase
	{
		protected IWindowPresenterController _sourceOpen;
		protected IUiProvider _uiProvider;

		private IWindowPresenterFactory _presenterFactory;
		public bool IsOpen { get; protected set; }

		[Inject]
		private void Constructor(IWindowPresenterFactory presenterFactory, IUiProvider uiProvider)
		{
			_presenterFactory = presenterFactory;
			_uiProvider = uiProvider;
		}

		public virtual async UniTask<bool> Show(IWindowPresenterController source)
		{
			if (IsOpen)
				return await UniTask.FromResult(false);

			IsOpen = true;
			_sourceOpen = source;
			_uiProvider.AddOrder(this);

			return true;
		}

		public virtual async UniTask<bool> Hide()
		{
			if (!IsOpen)
				return await UniTask.FromResult(false);
			IsOpen = false;
			_uiProvider.RemoveOrder(this);
			return await UniTask.FromResult(true);
		}

		public async UniTask<bool> ParentOpen()
		{
			if (_sourceOpen != null)
				return await _sourceOpen.Show(null);
			return await UniTask.FromResult(false);
		}
	}
}
