using UnityEngine;
using TMPro;
using it.UI.Elements;
using System.Text.RegularExpressions;
using it.Network.Rest;
 
using DG.Tweening;
using it.Main;

namespace it.Popups
{
	public class PasswordEdit : PopupBase
	{
		[SerializeField] private TMP_InputField _currentInput;
		[SerializeField] private TMP_InputField _newInput;
		[SerializeField] private TextMeshProUGUI _errorLabel;
		[SerializeField] private GraphicButtonUI _applyButton;

		protected override void EnableInit()
		{
			base.EnableInit();
			ClearForm();
			_currentInput.onValueChanged.RemoveAllListeners();
			_newInput.onValueChanged.RemoveAllListeners();
			_applyButton.interactable = false;
			_currentInput.onValueChanged.AddListener((val) =>
			{
				CheckChande();
			});
			_newInput.onValueChanged.AddListener((val) =>
			{
				CheckChande();
			});
		}

		private void ClearForm()
		{
			_currentInput.text = "";
			_currentInput.caretPosition = 0;
			_newInput.text = "";
			_newInput.caretPosition = 0;
			_applyButton.interactable = false;
			SetError("");
		}
		public void SetError(string error)
		{
			if (string.IsNullOrEmpty(error))
			{
				_errorLabel.transform.parent.gameObject.SetActive(false);
				return;
			}
			_errorLabel.text = error;
			_errorLabel.transform.parent.gameObject.SetActive(true);
		}

		private void CheckChande()
		{
			_applyButton.interactable = _currentInput.text.Length > 0 && _newInput.text.Length > 0;
		}

		public void ConfirmButtonTouch()
		{
			int errOld = Validator.Password(_currentInput.text);
			int errNew = Validator.Password(_newInput.text);
			if (errNew != -1 || errNew != -1)
			{
				SetError("errors.forms.noCorrectDataForm".Localized());
				return;
			}

			Lock(true);

			it.Api.UserApi.ChangePassword(_currentInput.text, _newInput.text, (result) =>
			{
				Lock(false);
				if (result.IsSuccess)
				{
					ClearForm();
					Hide();
					PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info)
					.SetDescriptionString("popup.passwordRecovery.success".Localized());
				}
				else
				{
					_errorLabel.text = "errors.forms.noCorrectDataForm".Localized();
					_errorLabel.transform.parent.gameObject.SetActive(true);
				}

			});
		}

	}
}