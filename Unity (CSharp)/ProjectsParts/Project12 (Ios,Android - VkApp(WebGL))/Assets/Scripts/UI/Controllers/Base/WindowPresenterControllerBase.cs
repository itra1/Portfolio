using Cysharp.Threading.Tasks;
using Game.Scripts.UI.Presenters.Factorys;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.UI.Controllers.Base
{
	public abstract class WindowPresenterControllerBase : IWindowPresenterControllerBase
	{
		public UnityEvent<IWindowPresenterController> OnPresenterVisibleChange { get; set; } = new();

		private IWindowPresenterFactory _presenterFactory;
		//protected IWindowPresenterController _sourceOpen;

		public bool IsOpen { get; protected set; }

		[Inject]
		private void Constructor(IWindowPresenterFactory presenterFactory)
		{
			_presenterFactory = presenterFactory;
		}

		public virtual async UniTask<bool> Open()
		{
			return await UniTask.FromResult(true);
		}

		public virtual async UniTask<bool> Close()
		{
			if (!IsOpen)
				return await UniTask.FromResult(false);
			return await UniTask.FromResult(true);
		}

		//public async UniTask<bool> BackStackNavigation()
		//{
		//	if (_sourceOpen != null)
		//		return await _sourceOpen.Open(null);
		//	return await UniTask.FromResult(false);
		//}

		protected void EmitOnPresenterVisibleChange()
		{
			OnPresenterVisibleChange?.Invoke(this as IWindowPresenterController);
		}
	}
}
