using Cysharp.Threading.Tasks;

using DG.Tweening;

using System;
using System.Threading;

using TMPro;

using UnityEngine;

namespace Providers.SystemMessage.Components
{
	public class SystemMessagePanel : MonoBehaviour
	{
		[SerializeField] private TMP_Text _infoLabel;

		private const float _labelWidth = 200;
		private const int _timePanel = 2500; // milliseconds

		private string _message;
		private bool _isVisible;
		private Vector2 _startSize;
		private RectTransform _rt;
		private CancellationTokenSource _visibleCancellationTS;

		private RectTransform RT => _rt != null ? _rt : _rt = GetComponent<RectTransform>();

		public void SetMessage(string message)
		{
			if (_startSize == Vector2.zero)
				_startSize = RT.sizeDelta;

			_message = message;

			_infoLabel.text = _message;

			if (!_isVisible)
			{
				StartVisible();
			}
			else
				UpdateTimerVisible().Forget();

		}

		private void StartVisible()
		{
			RT.anchoredPosition = new Vector2(300, RT.anchoredPosition.y);
			_isVisible = true;
			gameObject.SetActive(true);


			UpdateTimerVisible().Forget();
		}

		private async UniTaskVoid UpdateTimerVisible()
		{
			UpdateRect();
			RT.DOAnchorPos(new Vector2(0, RT.anchoredPosition.y), 0.2f);

			_visibleCancellationTS?.Cancel();
			_visibleCancellationTS?.Dispose();
			_visibleCancellationTS = new();

			try
			{
				await UniTask.Delay(_timePanel, cancellationToken: _visibleCancellationTS.Token);

				RT.DOAnchorPos(new Vector2(300, RT.anchoredPosition.y), 0.2f).OnComplete(()=> {
					_isVisible = false;
					gameObject.SetActive(false);
				});
			}
			catch(OperationCanceledException){

			}
		}

		private void UpdateRect()
		{
			if (_infoLabel.TryGetComponent<RectTransform>(out var labelrect))
			{
				labelrect.sizeDelta = new Vector2(Mathf.Min(_labelWidth, _infoLabel.preferredWidth), _infoLabel.preferredHeight);
				RT.sizeDelta = new Vector2(Mathf.Min(_labelWidth+70, _infoLabel.preferredWidth + 70), _infoLabel.preferredHeight + 40);
			}
		}

	}
}
