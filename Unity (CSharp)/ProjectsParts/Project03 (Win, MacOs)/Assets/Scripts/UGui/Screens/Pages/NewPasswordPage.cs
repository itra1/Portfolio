using Common;
using Cysharp.Threading.Tasks;
using Providers.Network.Common;
using Providers.SystemMessage.Common;
using System.Collections.Generic;
using TMPro;
using UGui.Screens.Common;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace UGui.Screens.Pages
{
	public class NewPasswordPage : MonoBehaviour, IZInjection
	{
		public UnityEngine.Events.UnityAction OnConfirm;

		[SerializeField] private TMP_InputField _newPasswordInput;
		[SerializeField] private TMP_InputField _newPasswordConfirmInput;
		[SerializeField] private Button _applyButton;

		private Dictionary<string, object> _data;
		private INetworkApi _api;
		private ISystemMessageVisible _meassage;

		[Inject]
		private void Initialize(INetworkApi api,
		ISystemMessageVisible message)
		{
			_api = api;
			_meassage = message;
		}

		private void OnEnable()
		{
			ClearForm();
		}

		private void ClearForm()
		{
			_applyButton.interactable = true;
			_newPasswordInput.text = "";
			_newPasswordConfirmInput.text = "";
			_applyButton.interactable = false;
		}

		private bool ValidateForm()
		{
			string newPassword = _newPasswordInput.text;
			string newPasswordRepeat = _newPasswordConfirmInput.text;

			if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(newPasswordRepeat))
			{
				_meassage.SetMessage(ErrorMessages.PasswordEmpty);
				return false;
			}
			if (newPassword == newPasswordRepeat)
			{
				_meassage.SetMessage(ErrorMessages.PasswordsMatch);
				return false;
			}
			var passwordError = InputValidate.Password(newPassword);

			switch (passwordError)
			{
				case InputValidate.ErrorFormat:
					{
						_meassage.SetMessage(ErrorMessages.PasswordErrorFormat);
						return false;
					};
				case InputValidate.ErrorLenght:
					{
						_meassage.SetMessage(ErrorMessages.PasswordLenght);
						return false;
					};
			};

			return true;
		}

		private async UniTaskVoid Process()
		{
			if (!ValidateForm()) return;

			string newPassword = _newPasswordInput.text;
			string newPasswordRepeat = _newPasswordConfirmInput.text;

			_applyButton.interactable = false;
			(bool result1, object response1) = await _api.RequestUpdatePassword();
			_applyButton.interactable = true;

			_data.Add("password", newPassword);
			_data.Add("password_confirmation", newPasswordRepeat);

			(bool result, object response) = await _api.PasswordRestore(_data);

			if (!result)
			{
				_meassage.SetMessage(ErrorMessages.ServerData);
				return;
			}

			OnConfirm?.Invoke();
		}

		public void SetValue(Dictionary<string, object> data)
		{
			_data = data;
		}

		public void ConfirmButtonTouch()
		{
			Process().Forget();
		}
	}
}
