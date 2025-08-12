 
using it.Network.Rest;
using it.Popups;
using it.UI.Elements;
using System.Collections;
using TMPro;
using UnityEngine;

namespace it.Popups
{
	public class NicknameEdit : PopupBase
	{
		[SerializeField] private TMP_InputField _nicknameInput;
		[SerializeField] private TextMeshProUGUI _errorLabel;
		[SerializeField] private GraphicButtonUI _applyButton;
		protected override void EnableInit()
		{
			base.EnableInit();
			_nicknameInput.text = UserController.User.nickname;

			_applyButton.interactable = false;
			SetError("");

			_nicknameInput.onValueChanged.RemoveAllListeners();
			_nicknameInput.onValueChanged.AddListener((val) =>
			{
				_nicknameInput.text = Validator.PhoneCorrect(val);
				_applyButton.interactable = true;
			});

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

		private bool Validate(string phone)
		{
			int err = Validator.Nickname(phone);

			switch (err)
			{
				case 0:
					SetError("errors.forms.empty".Localized());
					return false;
				case 1:
					SetError(string.Format("errors.forms.lenght_x_x".Localized(), 8, 15));
					return false;
				case 2:
					SetError("errors.forms.noCorrect".Localized());
					return false;
			}
			return true;
		}

		public void SaveButtonTouch()
		{
			string nickname = _nicknameInput.text;

			if (!Validate(nickname)) return;

			SetError("");

			Lock(true);

			UserController.NicknameChange(nickname, (result, error) =>
			{
				_applyButton.interactable = false;
				Lock(false);
				if (result)
				{
					//UserController.Instance.SetPhone(phone);
					Hide();
					return;
				}
			});

		}

		public void CancelButtonTouch()
		{
			Hide();
		}
	}
}