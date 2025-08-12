using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace it.UI.Elements
{
	public class SoundToggleUI : MonoBehaviour
	{
		public bool IsOn => _isOn;

		[SerializeField] private it.UI.Elements.GraphicButtonUI _onButton;
		[SerializeField] private it.UI.Elements.GraphicButtonUI _offButton;

		private bool _isOn;

		private void Start()
		{
			SetValue(true);
		}

		public void OnButton()
		{
			if (_isOn) return;
			SetValue(true);
		}

		public void OffButton()
		{
			if (!_isOn) return;
			SetValue(false);
		}

		private void SetValue(bool isOn)
		{
			_isOn = isOn;
			_onButton.GetComponent<Image>().enabled = !_isOn;
			_offButton.GetComponent<Image>().enabled = _isOn;
		}

	}
}