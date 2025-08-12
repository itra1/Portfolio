using Game.Base;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class HitIndicationsPanel : MonoBehaviour, IInjection
	{
		[SerializeField] private TMP_Text _valueLabel;
		[SerializeField] private RectTransform _progressRect;

		private int _currentValue;
		private int _maxHitValue;
		private float _maxWidthProgress = 0;

		[Inject]
		public void Initialize()
		{
			_maxWidthProgress = _progressRect.rect.width;
			_progressRect.sizeDelta = new(0, _progressRect.sizeDelta.y);
		}

		public void ChangeCount(int count, int maxHit)
		{
			_currentValue = count;
			_maxHitValue = maxHit;

			_valueLabel.text = _currentValue.ToString();

			float progressVall = _currentValue == 0
				? 0
				: (float) count / (float) _maxHitValue;
			_progressRect.sizeDelta = new(_maxWidthProgress * progressVall, _progressRect.sizeDelta.y);
		}
	}
}
