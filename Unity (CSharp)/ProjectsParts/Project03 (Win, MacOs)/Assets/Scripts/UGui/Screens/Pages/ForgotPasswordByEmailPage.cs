using Cysharp.Threading.Tasks;
using Providers.Network.Common;
using System.Collections.Generic;
using TMPro;
using UGui.Screens.Common;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using Providers.SystemMessage.Common;
using Common;
using System.Threading;

namespace UGui.Screens.Pages
{
	public class ForgotPasswordByEmailPage: MonoBehaviour, IZInjection
	{
		public UnityEngine.Events.UnityAction<string> OnConfirm;

		[SerializeField] private Button _applyButton;
		[SerializeField] private TMP_InputField _inputField;
		[SerializeField] private TMP_Text _repeatTextLabel;
		[SerializeField] private Sprite _defaultSprite;
		[SerializeField] private Sprite _errorSprite;

		private Image _itputBack;
		private string _email;
		private INetworkApi _api;
		private ISystemMessageVisible _meassage;
		private CancellationTokenSource _timerTS;
		private bool _isReadyResend = false;

		private readonly string _templateMessageResend = "Отправить повторно через {0} сек";

		[Inject]
		private void Initialize(INetworkApi api, ISystemMessageVisible message)
		{
			_api = api;
			_meassage = message;
			_itputBack = _inputField.gameObject.GetComponent<Image>();
			_inputField.onValueChanged.RemoveAllListeners();
			_inputField.onValueChanged.AddListener((res) =>
			{
				_itputBack.sprite = _defaultSprite;
			});
		}

		private void OnEnable()
		{
			ClearForm();
			StartTimer();
		}

		private void OnDisable()
		{
			_timerTS?.Cancel();
			_timerTS?.Dispose();
		}

		private void ClearForm()
		{
			_applyButton.interactable = true;
			_inputField.text = "";
		}

		public void SetValue(string value)
		{
			_email = value;
		}

		private async UniTaskVoid Process(){

			string txt = _inputField.text;

			if (string.IsNullOrEmpty(txt))
			{
				_itputBack.sprite = _errorSprite;
				_meassage.SetMessage(ErrorMessages.FieldEmpty);
				return;
			}

			Dictionary<string, object> request = new()
			{
				{ "email", _email },
				{ "code", txt }
			};

			_applyButton.interactable = false;
			(bool result, object response) = await _api.ChechPinPassword(request);
			_applyButton.interactable = true;

			if (!result)
			{
				_itputBack.sprite = _errorSprite;
				return;
			}

			OnConfirm?.Invoke(txt);

		}

		public void ConfirmButtonTouch()
		{
			Process().Forget();
		}
		private async UniTask ResendCode()
		{

			Dictionary<string, object> request = new();
			request.Add("email", _email);
			await _api.GetPinRestorePassword(request);
		}


		#region Timer

		private async void StartTimer()
		{
			_timerTS?.Cancel();
			_timerTS?.Dispose();

			_timerTS = new();
			int allSeconds = 180000;
			_isReadyResend = false;

			_repeatTextLabel.text = string.Format(_templateMessageResend, allSeconds / 1000);

			try
			{
				while (allSeconds > 0)
				{
					await UniTask.Delay(1000);
					allSeconds -= 1000;
					_repeatTextLabel.text = string.Format(_templateMessageResend, allSeconds / 1000);
				}
				_repeatTextLabel.text = "Отправить повторно";
				_isReadyResend = true;
			}
			catch { }

		}

		public void RepeatPinButtonTouch()
		{
			if (!_isReadyResend) return;
			ResendCode().Forget();
			StartTimer();
		}

		#endregion
	}
}
