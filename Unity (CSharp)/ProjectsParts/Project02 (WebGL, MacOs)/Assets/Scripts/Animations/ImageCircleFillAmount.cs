using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace it.Animations
{
	public class ImageCircleFillAmount : MonoBehaviour, IVisibleAnimation
	{
		[SerializeField] private Image _img;
		[SerializeField] private bool _confirmOnEnable = true;
		[SerializeField] private float _startValue = 0;
		[SerializeField] private float _endValue = 1;
		[SerializeField] private float _time = 0.5f;
		[SerializeField] private float _delay = 0.1f;

		public float CurrentValue => _endValue;

		public float EndValue { get => _endValue; set
			{
				_endValue = value;
				if (gameObject.activeInHierarchy)
					PlayAnimation();
			}
		}
		public float StartValue
		{
			get => _startValue; set
			{
				_startValue = value;
				Img.fillAmount = _startValue;
			}
		}

		public Image Img
		{
			get
			{
				if (_img == null)
					_img = GetComponent<Image>();
				return _img;
			}
		}

		private void OnEnable()
		{
			if (!_confirmOnEnable) return;
			StartValue = _startValue;
		}

		public void PlayAnimation()
		{
			_startValue = _endValue;
			Img.DOFillAmount(_endValue,_time).SetDelay(_delay);
		}
	}
}