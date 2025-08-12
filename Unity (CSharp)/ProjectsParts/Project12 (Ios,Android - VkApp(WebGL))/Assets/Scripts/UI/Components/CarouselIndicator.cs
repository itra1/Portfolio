using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Components
{
	public class CarouselIndicator : MonoBehaviour
	{
		[SerializeField] private Image _lightImage;

		public RectTransform RectTransform => transform as RectTransform;

		public void SetLight(float alpha)
		{
			_lightImage.color = new(1, 1, 1, 1 * (1 - alpha));
		}

		public void SetSize(float value)
		{
			var rt = RectTransform;
			rt.sizeDelta = new(32 + ((1 - value) * 38), rt.sizeDelta.y);
		}

		public void SetSizeOwerPosition(float value)
		{
			SetLight(value);
			//SetSize(value);
		}
	}
}
