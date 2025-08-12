using UnityEngine;
using TMPro;
using it.UI.Elements;
using System.Text.RegularExpressions;
using it.Network.Rest;
 

namespace it.Popups
{
	public class PhoneEdit : PopupBase
	{
		[SerializeField] private TMP_InputField _phoneInput;
		[SerializeField] private TextMeshProUGUI _errorLabel;
		[SerializeField] private GraphicButtonUI _applyButton;

		protected override void EnableInit()
		{
			base.EnableInit();

			_phoneInput.text = Validator.PhoneCorrect(UserController.User.phone);
			_applyButton.interactable = false;
			SetError("");

			_phoneInput.onValueChanged.RemoveAllListeners();
			_phoneInput.onValueChanged.AddListener((val) =>
			{
				_phoneInput.text = Validator.PhoneCorrect(val);
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
			int err = Validator.PhoneNumber(phone);

			switch (err)
			{
				case 0:
					SetError("errors.forms.empty".Localized());
					return false;
				case 1:
					SetError(string.Format("errors.forms.lenght_x_x".Localized(),8,15));
					return false;
				case 2:
					SetError("errors.forms.noCorrect".Localized());
					return false;
			}
			return true;
		}

		public void SaveButtonTouch()
		{
			string phone = Validator.PhoneCorrect(_phoneInput.text);

			if (!Validate(phone)) return;

			phone = "+" + phone;

			SetError("");
			if (phone == UserController.User.phone) return;

			Lock(true);
			it.Api.UserApi.PhoneUpdate(phone, (result) =>
			{
				_applyButton.interactable = false;
				Lock(false);
				if (result.IsSuccess)
				{
					UserController.Instance.SetPhone(phone);
					Hide();
					return;
				}

				//var error = (ErrorRest)it.Helpers.ParserHelper.Parse(typeof(ErrorRest), result.ErrorMessage);
				var error = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(result.ErrorMessage);

				if (error.errors[0].id == "phone" && error.errors[0].code == "validation_error")
				{
					SetError("errors.forms.noCorrect".Localized());
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