using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace it.UI.Elements
{
	public class PasswordInputField : MonoBehaviour
	{
		[SerializeField] private GameObject _eye;
		[SerializeField] private GameObject _eyeLock;
		[SerializeField] private TMP_InputField _inputField;

		public void EyePointer(bool isDown)
		{
			_eye.SetActive(isDown);
			_eyeLock.SetActive(!isDown);
			_inputField.contentType = isDown ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
			_inputField.ForceLabelUpdate();
		}

	}
}