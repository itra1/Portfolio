using Cysharp.Threading.Tasks;
using Game.Scripts.UI.Presenters.Animations;
using Game.Scripts.UI.Presenters.Interfaces;
using UnityEngine;

namespace Game.Scripts.UI.Presenters.Base
{
	public abstract class WindowPresenter : MonoBehaviour, IWindowPresenter
	{
		[SerializeField] protected RectTransform _positionTransform;

		protected CanvasGroup _canvasGroup;
		private IUiVisibleHandler[] _visibleElements;
		protected IShowPresenterAnimation _showPresenterAnimation;
		protected IHidePresenterAnimation _hidePresenterAnimation;

		public bool IsVisible { get; protected set; }

		public virtual async UniTask<bool> Initialize()
		{
			if (_positionTransform != null)
				PositionContent();

			_visibleElements = GetComponentsInChildren<IUiVisibleHandler>(true);

			_showPresenterAnimation = GetComponent<IShowPresenterAnimation>();
			_hidePresenterAnimation = GetComponent<IHidePresenterAnimation>();

			return await UniTask.FromResult(true);
		}

		public virtual async UniTask Show()
		{
			IsVisible = true;
			for (int i = 0; i < _visibleElements.Length; i++)
				_visibleElements[i].Show();

			if (_showPresenterAnimation != null)
				await _showPresenterAnimation.ShowAnimation();
			else
				gameObject.SetActive(true);
		}

		public virtual async UniTask Hide()
		{
			IsVisible = false;
			for (int i = 0; i < _visibleElements.Length; i++)
				_visibleElements[i].Hide();

			if (_hidePresenterAnimation != null)
				await _hidePresenterAnimation.HideAnimation();
			else
				gameObject.SetActive(false);
		}

		protected virtual void PositionContent()
		{
			_positionTransform.anchoredPosition = new(_positionTransform.anchoredPosition.x, 0);
		}

		protected CanvasGroup GetMainCanvasGroup()
		{
			if (_canvasGroup != null)
				return _canvasGroup;

			_canvasGroup = GetComponent<CanvasGroup>();

			if (_canvasGroup == null)
				_canvasGroup = gameObject.AddComponent<CanvasGroup>();
			return _canvasGroup;
		}
	}
}
