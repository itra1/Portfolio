using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
namespace it.UI.Elements
{
	[RequireComponent(typeof(RectTransform))]
	public class ProgressBarSlider : MonoBehaviour
	{

		[SerializeField] private RectTransform fillRect;
		[SerializeField] private TextMeshProUGUI valueText;
		[SerializeField] private float animTime = 0.5F;
		[SerializeField] private float maxValue = 100;

		private RectTransform mainRect;
		private float _value;
		private float value
		{
			get
			{
				return _value;
			}
			set
			{
				if (value < 0) value = 0;
				_value = value;

				UpdateFields();
			}

		}
		public void SetColor(Color targetColor){
			fillRect.GetComponent<Image>().color = targetColor;
		}
		private void Start()
		{
			mainRect = GetComponent<RectTransform>();
		}
		public void Awake()
		{
			value = 55;
		}
		public void SetValue(float value)
		{
			if (value > maxValue) value = maxValue;
			this.value = value;
		}
		private void UpdateFields()
		{
			if (mainRect == null)
			{
				mainRect = GetComponent<RectTransform>();
			}
			valueText.text = $"{value}%";
			fillRect.DOSizeDelta(new Vector2(mainRect.rect.width * value / maxValue, 0), animTime);
		}

	}
}
