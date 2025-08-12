using System.Collections;
using UnityEngine;
using TMPro;

namespace it.Popups
{
	public class PhoneForm : MonoBehaviour
	{
		[SerializeField] private TMP_InputField _codeInput;
		[SerializeField] private TMP_InputField _numberInput;

		public string Phone { get{ return _codeInput.text + _numberInput.text; } }

		private void OnEnable()
		{
			ClearForm();
		}
		private void ClearForm()
		{
			if (_codeInput != null)
				_codeInput.text = "";
			if (_numberInput != null)
				_numberInput.text = "";
		}
	}
}