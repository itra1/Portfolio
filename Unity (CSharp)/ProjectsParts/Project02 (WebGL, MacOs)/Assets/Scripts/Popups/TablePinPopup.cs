using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine;
using TMPro;
using it.Network.Rest;

namespace it.Popups
{
	public class TablePinPopup : PopupBase
	{
		public UnityEngine.Events.UnityAction<string> OnConfirm; 
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TMP_InputField[] _inputFields;
		[SerializeField] private string[] _inputValues;
		[SerializeField] private TMP_InputField _pinInput;
		[SerializeField] private TextMeshProUGUI _errorLabel;

		private Table _table;

		protected override void EnableInit()
		{
			base.EnableInit();
			_errorLabel.gameObject.SetActive(false);
			_inputValues = new string[_inputFields.Length];


			for (int i = 0; i < _inputFields.Length; i++)
			{
				int index = i;
				_inputFields[index].text = "";
				_inputFields[index].caretPosition = 0;
				_inputValues[index] = _inputFields[index].text;

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
					if (field.text.Length == 0)
						return;

					var elem = field.FindSelectableOnDown();
					if (elem != null) elem.Select();

				});
			}
		}
		private string GetPassText()
		{
			string pass = "";
			for (int i = 0; i < _inputValues.Length; i++)
			{
				pass += _inputValues[i];
			}
			return pass;
		}

		public void Set(Table table)
		{
			_table = table;
			if(_title != null)
			_title.text = _table.name;
		}

		public void OkButtonTouch()
		{
			string pass = GetPassText();

			if (_pinInput != null)
				pass = _pinInput.text;


			Lock(true);
			_errorLabel.gameObject.SetActive(false);
			TableApi.ObserveTable(_table.id, pass, (response) =>
			{
				Lock(false);

				if (response.IsSuccess)
				{
					Hide();
					OnConfirm?.Invoke(pass);
				}
				else
				{

					//var data = (ErrorRest)it.Helpers.ParserHelper.Parse(typeof(ErrorRest), response.ErrorMessage);
					var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(response.ErrorMessage);
					if (data.errors[0].id == "user_is_already_player"){
						Hide();
						OnConfirm?.Invoke(GetPassText());
						return;
					}

					_errorLabel.gameObject.SetActive(true);
				}

			});
		}

	}
}