using Cysharp.Threading.Tasks;
using Game.Scripts.UI.Presenters.Base;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Presenters
{
	public class LoaderPresenter : WindowPresenter
	{
		[SerializeField] private RectTransform _loaderrPanelRectTransform;
		[SerializeField] private RectTransform _maskProgressRectTransform;
		[SerializeField] private RectTransform _fillProgressRectTransform;
		[SerializeField] private TMP_Text _loadingLabel;

		private float _progressValue = 0;
		private Vector2 _startMaskAnchor;
		private Vector2 _endMaskAnchor;
		private Vector2 _currentMaskAnchor;
		private float _progressLenght;
		private Vector2 _fillPosition;

		public override async UniTask<bool> Initialize()
		{
			_ = await base.Initialize();
			await UniTask.Yield();
			_fillPosition = _fillProgressRectTransform.anchoredPosition;

			_startMaskAnchor = _maskProgressRectTransform.anchoredPosition;
			_startMaskAnchor.x = -10;

			_endMaskAnchor = _maskProgressRectTransform.anchoredPosition;
			_endMaskAnchor.x = _loaderrPanelRectTransform.rect.width + 10;

			_progressLenght = _endMaskAnchor.x - _startMaskAnchor.x;

			_fillProgressRectTransform.SetParent(_maskProgressRectTransform);
			_fillPosition = _fillProgressRectTransform.transform.position;

			return true;
		}

		private void OnEnable()
		{
			LoadingProgress(0);
		}

		/// <summary>
		/// Установка прогресса
		/// </summary>
		/// <param name="value">Прогресс [0-1]</param>
		public void LoadingProgress(float value)
		{
			_progressValue = value;
			_loadingLabel.text = $"loading {(int) (_progressValue * 100)}%";

			_maskProgressRectTransform.anchoredPosition = new(_startMaskAnchor.x + (_progressValue * _progressLenght), _maskProgressRectTransform.anchoredPosition.y);
			_fillProgressRectTransform.SetParent(_maskProgressRectTransform.parent);
			_fillProgressRectTransform.anchoredPosition = _fillPosition;
			_fillProgressRectTransform.SetParent(_maskProgressRectTransform);
		}
	}
}
