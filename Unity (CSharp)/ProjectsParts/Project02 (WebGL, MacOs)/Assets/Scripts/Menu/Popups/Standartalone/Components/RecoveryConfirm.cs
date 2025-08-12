 
using it.Network.Rest;
using it.Popups;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecoveryConfirm : MonoBehaviour
{
	public UnityEngine.Events.UnityAction<string, string> OnRecovery;


	Dictionary<string, string> errorTypes = new Dictionary<string, string>
	{
		["code"] = "errors.forms.noCorrectCode",
		["incorrect_confirmation_code"] = "errors.forms.noCorrectCode"
	};

	[SerializeField] private TMP_InputField[] _inputFields;
	[SerializeField] private string[] _inputValues;
	[SerializeField] private TextMeshProUGUI _errorLabel;
	[SerializeField] private it.UI.Elements.TextButtonUI _buttonResend;
	[SerializeField] private it.UI.Elements.GraphicButtonUI _applyButton;
	[SerializeField] private Button _buttonPaste;
	[SerializeField] private TextMeshProUGUI _resetButtonLabel;
	[SerializeField] private Color _activeResentColor;
	[SerializeField] private Color _disableResentColor;
	[SerializeField] private RectTransform _errorRect;

	private string _email;

	private System.DateTime _waitToTime;
	private string _clipboardText = "";

	private void Awake()
	{
		if (_buttonPaste != null)
			_buttonPaste.onClick.AddListener(() => { PasteClipboard(); });
	}

	private void Update()
	{
		var t = UniClipboard.GetText();
		if (t.Length == _inputFields.Length && int.TryParse(t, out _))
		{
			if (_buttonPaste != null)
				_buttonPaste.gameObject.SetActive(true);
			_clipboardText = t;
		}
		else
		{
			if (_buttonPaste != null)
				_buttonPaste.gameObject.SetActive(false);
		}
	}
	private void PasteClipboard()
	{
		if (_clipboardText.Length != _inputFields.Length) return;
		for (int i = 0; i < _inputFields.Length; i++)
		{
			_inputFields[i].text = _clipboardText[i].ToString();
		}
	}
	private void OnEnable()
	{
		_errorRect.gameObject.SetActive(false);

		SetDefaultResendButton();

		_inputValues = new string[_inputFields.Length];

		for (int i = 0; i < _inputFields.Length; i++)
		{
			int index = i;
			_inputFields[index].text = "";
			_inputValues[index] = _inputFields[index].text;
			_inputFields[index].caretPosition = 0;
			_inputFields[index].textComponent.fontSizeMax = 20;

			var field = _inputFields[i];

			field.onValueChanged.RemoveAllListeners();
			field.onValueChanged.AddListener((val) =>
			{
				if (_inputValues[index] == "" && val == "")
				{
					var beforeElement = field.FindSelectableOnUp();
					if (beforeElement != null)
					{
						beforeElement.GetComponent<TMP_InputField>().text = "";
						_inputValues[index - 1] = "";
						beforeElement.Select();
					}
				}

				_inputFields[index].textComponent.fontSizeMax = field.text.Length > 0 ? 35 : 20;
				_inputValues[index] = field.text;
				if (field.text.Length > 1)
					field.text = field.text.Substring(0, 1);
				_errorRect.gameObject.SetActive(false);
				if (field.text.Length == 0)
					return;

				var elem = field.FindSelectableOnDown();
				if (elem != null) elem.Select();

			});

		}
		_inputFields[0].onValueChanged.AddListener((val) =>
		{
			if (_clipboardText != "")
			{
				if (_buttonPaste != null)
					PasteClipboard();
			}
		});
		StartTimer(System.DateTime.Now.AddSeconds(30));
	}

	public void StartTimer(System.DateTime dateTime)
	{

		if (dateTime < System.DateTime.Now)
		{
			SetDefaultResendButton();
			return;
		}

		_waitToTime = dateTime;

		StartCoroutine(CoroutineTimer());
	}

	private IEnumerator CoroutineTimer()
	{
		_buttonResend.interactable = false;
		_resetButtonLabel.color = _disableResentColor;
		_buttonResend.StartColor = _disableResentColor;

		while (_waitToTime > System.DateTime.Now)
		{
			_resetButtonLabel.text = $"{"popup.passwordRecovery.resendButtonAfter".Localized()} {(int)(_waitToTime - System.DateTime.Now).TotalSeconds} {"popup.passwordRecovery.sec".Localized()}";
			yield return new WaitForSeconds(1);
		}
		SetDefaultResendButton();
	}

	private void SetDefaultResendButton()
	{
		_buttonResend.interactable = true;
		_buttonResend.StartColor = _activeResentColor;
		_resetButtonLabel.color = _activeResentColor;
		_resetButtonLabel.text = "popup.passwordRecovery.resendButton".Localized();
	}

	public void SetEmail(string email)
	{
		_email = email;
	}

	public void ApplyButton()
	{
		string code = "";

		for (int i = 0; i < _inputFields.Length; i++)
		{
			code += _inputFields[i].text;
		}

		if (code.Length != _inputFields.Length)
		{
			_errorRect.gameObject.SetActive(true);
			_errorLabel.text = "errors.forms.noCorrectCode".Localized();
		}

		GetComponentInParent<PopupBase>().Lock(true);

		UserController.Instance.ResetPassword(_email, code, () =>
		{
			GetComponentInParent<PopupBase>().Lock(false);
			OnRecovery?.Invoke(_email, code);
		}, (error) =>
		{
			GetComponentInParent<PopupBase>().Lock(false);
			_errorRect.gameObject.SetActive(true);

			try
			{
				//ErrorRest err = it.Helpers.ParserHelper.Parse<ErrorRest>(Leguar.TotalJSON.JSON.ParseString(error));
				ErrorResponse err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(error);
				_errorLabel.text = errorTypes[err.errors[0].id].Localized();
				_errorRect.gameObject.SetActive(true);
			}
			catch
			{
				_errorLabel.text = "errors.forms.noCorrectCode".Localized();
				_errorRect.gameObject.SetActive(true);
			}

		});

	}

	/// <summary>
	/// Запрос на новый емаил сщву
	/// </summary>
	public void ResendButton()
	{
		_errorRect.gameObject.SetActive(false);
		it.Api.UserApi.RequestRecoveryPassword(_email, (result) =>
		{
			StartTimer(System.DateTime.Now.AddSeconds(30));
		}, (error) =>
		{
			GetComponentInParent<PopupBase>().Lock(false);
			_errorRect.gameObject.SetActive(true);
			try
			{
				//ErrorRest err = it.Helpers.ParserHelper.Parse<ErrorRest>(Leguar.TotalJSON.JSON.ParseString(error));
				ErrorResponse err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(error);

				_errorLabel.text = errorTypes[err.errors[0].id].Localized();
				_errorRect.gameObject.SetActive(true);
			}
			catch
			{
				_errorLabel.text = "errors.forms.noCorrectDataForm".Localized();
				_errorRect.gameObject.SetActive(true);
			}
		});

	}

}
