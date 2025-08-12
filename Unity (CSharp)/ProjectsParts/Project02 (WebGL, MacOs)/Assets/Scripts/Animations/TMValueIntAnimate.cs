using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace it.Animations
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class TMValueIntAnimate : MonoBehaviour, IVisibleAnimation
	{
		[SerializeField] private TextMeshProUGUI _label;
		[SerializeField] private string _patern = "{0}";
		[SerializeField] private bool _separateAmount = true;
		[SerializeField] private bool _confirmOnEnable = true;
		[SerializeField] private int _startValue = 0;
		[SerializeField] private int _endValue = 1;
		[SerializeField] private float _time = 0.5f;
		[SerializeField] private float _delay = 0.1f;

		public TextMeshProUGUI Label
		{
			get
			{
				try
				{
					if (_label == null)
						_label = GetComponent<TextMeshProUGUI>();
					return _label;
				}
				catch
				{
					return _label;
				}
			}
		}
		public int EndValue
		{
			get => _endValue; set
			{
				_endValue = value;
				if (gameObject.activeInHierarchy)
					PlayAnimation();
			}
		}
		public int StartValue
		{
			get => _startValue; set
			{
				_startValue = value;
				if (Label != null)
					Label.text = string.Format(_patern, _startValue);
			}
		}

		public string Patern { get => _patern; set => _patern = value; }

		private void OnEnable()
		{
			if (!_confirmOnEnable) return;
			StartValue = _startValue;
		}

		public void PlayAnimation()
		{

			try
			{
				int val = _startValue;
				_startValue = _endValue;
				DOTween.To(() => val, (x) =>
				{
					try
					{
						val = x;
						//Label.text =  string.Format(_patern, val);
						if (_separateAmount)
						{
							if (Label != null)
								Label.text = string.Format(_patern, it.Helpers.Currency.String(val, false));
						}
						else
						{
							if (Label != null)
								Label.text = string.Format(_patern, val);
						}
					}
					catch { }
				}, _endValue, _time);
			}
			catch { }
		}

	}
}