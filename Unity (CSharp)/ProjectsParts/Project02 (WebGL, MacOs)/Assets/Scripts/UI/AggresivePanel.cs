using System.Collections;
using UnityEngine;


namespace Garilla.Games
{
	public class AggresivePanel : MonoBehaviour
	{
		[SerializeField] private TMPro.TextMeshProUGUI _valueLabel;
		[SerializeField] private RectTransform _stamdartBack;
		[SerializeField] private RectTransform _iceBack;
		[SerializeField] private RectTransform _blackBack;
		[SerializeField] private RectTransform _hotBack;
		[SerializeField] private RectTransform _fireBack;

		float? _value = float.MinValue;

		private void OnEnable()
		{
			SerAggresiceValue(null);
		}


		public void SerAggresiceValue(float? value)
		{

			if (_value == value) return;
			_value = value;
			if (_value == null)
			{
				_stamdartBack.gameObject.SetActive(true);
				_valueLabel.text = "";
				return;
			}
			_valueLabel.text = ((int)_value).ToString();

			_stamdartBack.gameObject.SetActive(false);
			_iceBack.gameObject.SetActive(0 <= _value.Value && _value.Value < 25);
			_blackBack.gameObject.SetActive(25 <= _value.Value && _value.Value < 50);
			_hotBack.gameObject.SetActive(50 <= _value.Value && _value.Value < 75);
			_fireBack.gameObject.SetActive(75 <= _value.Value && _value.Value <= 100);


		}

	}
}