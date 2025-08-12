using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace it.Animations
{
	public class ScaleX : MonoBehaviour, IVisibleAnimation
	{
		[SerializeField] private RectTransform _rt;
		[SerializeField] private bool _confirmOnEnable = true;
		[SerializeField] private float _startValue = 0;
		[SerializeField] private float _endValue = 1;
		[SerializeField] private float _time = 0.5f;
		[SerializeField] private float _delay = 0.1f;

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
				Rt.localScale = new Vector2(_startValue, 1);
			}
		}

		public RectTransform Rt
		{
			get
			{
				if (_rt == null)
					_rt = GetComponent<RectTransform>();
				return _rt;
			}
		}

		private void OnEnable()
		{
			if (!_confirmOnEnable) return;
			StartValue = _startValue;
		}

		public void PlayAnimation()
		{
			Rt.DOScale(new Vector2(_endValue, 1), _time).SetDelay(_delay);
		}
	}
}