using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace it.UI.Elements
{
	public class SliderScrollHandler : MonoBehaviour, IScrollHandler,IDragHandler, IPointerClickHandler, IPointerDownHandler
	{
		[SerializeField] float _speed = 0.1f;
		private Slider _slider;

		public float Speed { get => _speed; set => _speed = value; }

    public void OnDrag(PointerEventData eventData)
		{
			Change();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			Change();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			Change();
		}

		private void Change(){

			if (_slider == null)
				_slider = GetComponent<Slider>();
			if (_slider != null)
			{
				if(_slider.value < _slider.maxValue && _slider.value > _slider.minValue)
				{
					var countInt = _slider.value < _slider.maxValue ? Mathf.Round(_slider.value / _speed) * _speed : _slider.maxValue;
					_slider.value = Mathf.Clamp(countInt, _slider.minValue, _slider.maxValue);
				}
			}
		}

		public void OnScroll(PointerEventData eventData)
		{
			if (_slider == null)
				_slider = GetComponent<Slider>();

			if (_slider != null)
			{
				_slider.value += Mathf.Sign(eventData.scrollDelta.y) * _speed;
			}

		}
	}
}