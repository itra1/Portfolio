using Cysharp.Threading.Tasks;
using Game.Providers.Ui.Presenters.Interfaces;
using UnityEngine;

namespace Game.Providers.Ui.Presenters.Base
{
	public class WindowPresenter : MonoBehaviour, IWindowPresenter
	{
		private IUiVisibleHandler[] _visibleElements;

		public virtual async UniTask<bool> Initialize()
		{
			_visibleElements = GetComponentsInChildren<IUiVisibleHandler>(true);
			return await UniTask.FromResult(true);
		}

		public virtual void Show()
		{
			for (int i = 0; i < _visibleElements.Length; i++)
				_visibleElements[i].Show();
		}

		public virtual void Hide()
		{
			for (int i = 0; i < _visibleElements.Length; i++)
				_visibleElements[i].Hide();
		}
	}
}
