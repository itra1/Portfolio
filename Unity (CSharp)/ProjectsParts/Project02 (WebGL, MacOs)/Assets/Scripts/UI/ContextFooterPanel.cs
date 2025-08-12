using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.UI
{
  public class ContextFooterPanel : MonoBehaviour
	{
		[SerializeField] private Canvas _buttonCanvas;
		[SerializeField] private RectTransform _contextPanel;
		[SerializeField] private RectTransform _closeButton;

		protected bool _isShow;

		protected virtual void OnEnable()
		{
			_isShow = false;
			_contextPanel.gameObject.SetActive(false);
			_closeButton.gameObject.SetActive(false);
			//_buttonCanvas.overrideSorting = false;
		}

		public void OpenButton()
		{
			_isShow = !_isShow;

			if (_isShow)
			{
				_closeButton.gameObject.SetActive(true);
				_contextPanel.anchoredPosition = new Vector2(_contextPanel.anchoredPosition.x, -_contextPanel.rect.height);
				_contextPanel.gameObject.SetActive(true);
				//_buttonCanvas.overrideSorting = true;
				_contextPanel.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
				{

				});
			}
			else
			{
				_closeButton.gameObject.SetActive(false);
				//_buttonCanvas.overrideSorting = false;
				_contextPanel.DOAnchorPos(new Vector2(_contextPanel.anchoredPosition.x, -_contextPanel.rect.height), 0.2f).OnComplete(() =>
				{
					_contextPanel.gameObject.SetActive(false);
				});
			}

		}

	}
}