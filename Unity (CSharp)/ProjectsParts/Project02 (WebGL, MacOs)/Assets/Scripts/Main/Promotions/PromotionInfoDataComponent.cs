using DG.Tweening;

using I2.Loc;

using TMPro;
using it.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace it.UI.Promotions
{
	public class PromotionInfoDataComponent : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _progressText;
		[SerializeField] private TextMeshProUGUI _valueText;
		[SerializeField] private Slider _progressSlider;
		[SerializeField, Range(0.1f, 4f)] private float _progressAnimationDuration = 1f;

		private InfoData _data;

		public string typeData => _data.type;

		public void OnEnable()
		{
			UpdateProgress(_data.progress);
		}

		public void SetData(InfoData data)
		{
			_data = data;
			_title.text = $"singlePage.promotions.navigations.{data.type}".Localized().ToUpper();
			if (_data.isRake)
				_progressText.text = $"{_data.progress.x.CurrencyString()} / {_data.progress.y.CurrencyString()} {"singlePage.promotions.rakeIsAwarded".Localized()}";
			else
				_progressText.text = $"{_data.progress.x} / {_data.progress.y}";
			_valueText.text = Helpers.Currency.String(_data.value);
			UpdateProgress(_data.progress);
		}

		public void UpdateProgress(Vector2 progress)
		{
			_data.progress = progress;
			_progressSlider.value = 0;
			_progressSlider.minValue = 0;
			_progressSlider.maxValue = _data.progress.y;
			_progressSlider.DOValue(_data.progress.x, _progressAnimationDuration);
		}

		public struct InfoData
		{
			public string type;
			public Vector2 progress;
			public int value;
			public bool isRake;

			public InfoData(string type, Vector2 progress, int value, bool isRake = false)
			{
				this.type = type;
				this.progress = progress;
				this.value = value;
				this.isRake = isRake;
			}
		}
	}
}