using Cysharp.Threading.Tasks;
using Providers.Network.Common;
using Providers.Network.Materials;
using System.Collections.Generic;
using TMPro;
using UGui.Screens.Common;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using Common;
using Providers.SystemMessage.Common;

namespace UGui.Screens.Pages
{
	public class ForgotPasswordPage : MonoBehaviour, IZInjection
	{
		public UnityEngine.Events.UnityAction<string> OnNextPhone;
		public UnityEngine.Events.UnityAction<string> OnNextEmail;

		[SerializeField] private TMP_InputField _inputField;
		[SerializeField] private Button _applyButton;

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
			_inputField.text = "";
		}

		private bool ValidateForm()
		{
			string txt = _inputField.text;

			if (string.IsNullOrEmpty(txt))
			{
				_meassage.SetMessage(ErrorMessages.FieldEmpty);
				return false;
			}

			var isEmail = InputValidate.IsEmail(txt);
			var isPhone = InputValidate.IsPhone(txt);

			if (!isEmail && !isPhone)
			{
				_meassage.SetMessage(ErrorMessages.UserNameErrorFormat);
				return false;
			}

			return true;
		}

		private async UniTaskVoid Process()
		{
			if (!ValidateForm()) return;

			string txt = _inputField.text;

			var isEmail = InputValidate.IsEmail(txt);
			var isPhone = InputValidate.IsPhone(txt);

			Dictionary<string, object> request = new();

			if (isPhone)
				request.Add("phone", "7" + txt);
			if (isEmail)
				request.Add("email", txt);

			_applyButton.interactable = false;
			(bool result, object response) = await _api.GetPinRestorePassword(request);
			_applyButton.interactable = true;

			if (!result)
			{
				return;
			}

			RestoreReturnData responseData = (RestoreReturnData)response;

			if (responseData.type == "email")
			{
				OnNextEmail?.Invoke(txt);
				return;
			}
			if (responseData.type == "phone")
			{
				OnNextPhone?.Invoke(txt);
				return;
			}

		}

		public void RecoverButtonTouch()
		{
			Process().Forget();
		}
	}
}
