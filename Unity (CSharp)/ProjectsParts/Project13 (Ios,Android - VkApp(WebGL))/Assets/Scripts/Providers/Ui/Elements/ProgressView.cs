using UnityEngine;

namespace Game.Providers.Ui.Elements
{
	public class ProgressView : MonoBehaviour
	{
		[SerializeField] private RectTransform _progressBackRect;
		[SerializeField] private RectTransform _progressRect;

		private float? _width;

		public void SetValue(float value, string label)
		{
			if (_width == null)
				_width = _progressRect.sizeDelta.x;
			_progressRect.sizeDelta = new(value * _width.Value, _progressRect.sizeDelta.y);
		}
	}
}
