using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UGui.Elements
{
	public class PasswordVisibleButton : MonoBehaviour
	{
		[SerializeField] private RectTransform _visibleIcone;
		[SerializeField] private RectTransform _invisibleIcone;

		private TMP_InputField _inputField;
		private bool _isVisible;

		private void Awake()
		{
			SubscribeButton();
		}

		private void OnEnable()
		{
			FindComponents();
			SetVisibleText(false);
		}

		private void SubscribeButton()
		{
			if (TryGetComponent<Button>(out var btn))
			{
				btn.onClick.RemoveAllListeners();
				btn.onClick.AddListener(ButtonTouch);
			}
		}

		private void ButtonTouch()
		{
			SetVisibleText(!_isVisible);
		}

		private void FindComponents()
		{
			_inputField = _inputField == null ? GetComponentInParent<TMP_InputField>() : _inputField;
		}

		private void SetActiveImage(bool isVisible)
		{
			_visibleIcone.gameObject.SetActive(!isVisible);
			_invisibleIcone.gameObject.SetActive(isVisible);
		}

		private void SetVisibleText(bool isVisible)
		{
			var txt = _inputField.text;
			_inputField.contentType = isVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
			_inputField.text = txt;
			_inputField.ForceLabelUpdate();
			SetActiveImage(isVisible);
			_isVisible = !_isVisible;
		}

	}
}
