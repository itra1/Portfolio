using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace it.UI.Elements
{
	public class SmartHudCircleBar : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _valueLabel;
		[SerializeField] private Image _diagremImage;

		[SerializeField] private Color _color25;
		[SerializeField] private Color _color50;
		[SerializeField] private Color _color75;
		[SerializeField] private Color _color100;

		private float _startValue;

		private void OnEnable()
		{
			PrintValue(_startValue);
		}

		public void SetData(float value){

			float currentValue = _startValue;

			DOTween.To(() => currentValue, (x) =>
			{
				currentValue = x;
				PrintValue(currentValue);
			}, value, 0.5f);

		}
		public void SetData(float startValue, float value)
		{
			_startValue = startValue;
			PrintValue(_startValue);
			SetData(value);
		}

		private void PrintValue(float value){
			_valueLabel.text = $"{(int)(value*100)}<size=80%>%";
			_diagremImage.fillAmount = value;
			if (value <= 1f)
				_diagremImage.color = _color100;
			if (value <= 0.75f)
				_diagremImage.color = _color75;
			if (value <= 0.50f)
				_diagremImage.color = _color50;
			if (value <= 0.25f)
				_diagremImage.color = _color25;
		}

	}
}