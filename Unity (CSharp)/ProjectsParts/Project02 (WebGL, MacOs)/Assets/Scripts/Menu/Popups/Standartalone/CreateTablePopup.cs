using it.Popups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.UI.Elements;
 
using it.Main;
using DG.Tweening;
using it.Settings;

public class CreateTablePopup : PopupBase
{
	[SerializeField] private GameObject _firstRect;
	[SerializeField] private GameObject _tableRect;
	[SerializeField] private GameObject _comingsoonRect;
	[SerializeField] private RectTransform _formRect;
	[SerializeField] private List<ButtonData> _buttonsNavigation;
	//[SerializeField] private TextMeshProUGUI _title;
	[SerializeField] public MinMaxInputValueViewBase playersCountView;
	[SerializeField] private MinMaxInputValueViewBase autoStartPlayersView;
	[SerializeField] private MinMaxInputValueViewBase timeBankView;

	[SerializeField] private ActionTimeCrTableView actionTimeView;
	[SerializeField] private BlindsView blindsView;
	[SerializeField] private ButtonUI _createButton;

	[SerializeField] private TMP_InputField _pinInput;
	[SerializeField] private TMP_InputField[] _inputFields;
	[SerializeField] private string[] _inputValues;
	//[SerializeField] private TextMeshProUGUI _formTitleLabel;
	[SerializeField] private Image _icone;

	private PokerTableSettings currentSettings;
	private string _email;
	private bool _isCreate;
	private string _selectTableId = "";

	[System.Serializable]
	public struct ButtonData
	{
		public string Id;
		public string Title;
		public bool IsComingSoon;
		public it.UI.Elements.GraphicButtonUI Button;
		public RectTransform Light;
		public Sprite Icone;
	}

	private System.DateTime _waitToTime;

	protected override void Awake()
	{
		base.Awake();
		playersCountView.OnValueChange = () =>
		{
			var options = UserController.ReferenceData.create_vip_table_options[_selectTableId];
			autoStartPlayersView.SetSettings(options.auto_start_players_count_min, Mathf.Min(playersCountView.currentValue, options.auto_start_players_count_max));
		};
	}

	protected override void EnableInit()
	{
		base.EnableInit();
		SetTableSettings("");

		for (int i = 0; i < _buttonsNavigation.Count; i++)
		{
			int index = i;
			_buttonsNavigation[index].Button.OnClick.RemoveAllListeners();
			_buttonsNavigation[index].Button.OnClick.AddListener(() =>
			{
				SetTableSettings(_buttonsNavigation[index].Id);
			});
		}

		_inputValues = new string[_inputFields.Length];
		_isCreate = false;
		//_title.text = "popup.createTable.tableName".Localized() + ": ";
		for (int i = 0; i < _inputFields.Length; i++)
		{
			int index = i;
			_inputFields[index].text = "";
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
				bool isComplete = GetPassText().Length == _inputValues.Length;
				if (field.text.Length == 0)
					return;

				var elem = field.FindSelectableOnDown();
				if (elem != null) elem.Select();

			});
		}
	}

	private void ConfirmLight()
	{
		//bool exists = false;
		//for (int i = 0; i < UserController.Instance.CreateTableOptions.Count; i++)
		//{
		//	if (UserController.Instance.CreateTableOptions.ContainsKey(_selectTableId))
		//		exists = true;
		//}
		//_createButton.interactable = exists;
		//if (!exists){

		//	for (int i = 0; i < _buttonsNavigation.Length; i++)
		//	{
		//		_buttonsNavigation[i].Light.gameObject.SetActive(false);
		//		_buttonsNavigation[i].Button.StartColor = Color.white;
		//	}
		//	return;
		//}

		for (int i = 0; i < _buttonsNavigation.Count; i++)
		{
			_buttonsNavigation[i].Light.gameObject.SetActive(_buttonsNavigation[i].Id == _selectTableId);
			_buttonsNavigation[i].Button.StartColor = _buttonsNavigation[i].Id == _selectTableId ? new Color(1, 1, 1, 1f) : new Color(1, 1, 1, 0.4f);
		}
	}

	public void SetTableSettings(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			_selectTableId = "";
			ConfirmLight();
			if (_firstRect != null)
				_firstRect.gameObject.SetActive(true);
			if (_tableRect != null)
				_tableRect.gameObject.SetActive(false);
			if (_comingsoonRect != null)
				_comingsoonRect.gameObject.SetActive(false);

			if (_formRect != null)
				_formRect.anchoredPosition = new Vector2(_formRect.rect.width, 0);
			return;
		}
		if (!UserController.ReferenceData.create_vip_table_options.ContainsKey(id))
		{
			_selectTableId = id;
			ConfirmLight();
			if (_firstRect != null)
				_firstRect.gameObject.SetActive(false);
			if (_tableRect != null)
				_tableRect.gameObject.SetActive(false);
			if (_comingsoonRect != null)
				_comingsoonRect.gameObject.SetActive(true);

			PopupController.Instance.ShowPopup(PopupType.Develop);
			return;
		}

		if (_selectTableId == id) return;
		_selectTableId = id;
		ConfirmLight();
		var options = UserController.ReferenceData.create_vip_table_options[_selectTableId];

		if (_firstRect != null)
			_firstRect.gameObject.SetActive(false);
		if (_tableRect != null)
			_tableRect.gameObject.SetActive(true);
		if (_comingsoonRect != null)
			_comingsoonRect.gameObject.SetActive(false);

		if (_formRect != null)
			_formRect.DOAnchorPos(new Vector2(0, 0), 0.2f);

		//if (_formTitleLabel != null)
		//	_formTitleLabel.text = _buttonsNavigation.Find(x=>x.Id == _selectTableId).Title;

		if (_icone != null)
			_icone.sprite = _buttonsNavigation.Find(x => x.Id == _selectTableId).Icone;

		playersCountView.SetSettings(options.players_count_min, options.players_count_max);
		actionTimeView.SetSettings(options.action_time);
		timeBankView.SetSettings(options.time_bank_min, options.time_bank_max);
		autoStartPlayersView.SetSettings(options.auto_start_players_count_min, Mathf.Min(playersCountView.currentValue, options.auto_start_players_count_max));
		blindsView.SetSettings(options);

	}

	public void BackToMenuTouch()
	{

		if (_formRect != null)
			_formRect.DOAnchorPos(new Vector2(_formRect.rect.width, 0), 0.2f);
	}

	private string GetPassText()
	{
		if (_pinInput != null)
			return _pinInput.text;

		string pass = "";
		for (int i = 0; i < _inputValues.Length; i++)
		{
			pass += _inputValues[i];
		}
		return pass;
	}
	public void CreateTableButton()
	{
		it.Network.Rest.CreateTableInfo info = new it.Network.Rest.CreateTableInfo();

		info.id = _selectTableId;
		info.level_id = blindsView.GetLevelID(); //maybe not

		info.action_time = Mathf.RoundToInt(actionTimeView.currentValue);
		info.players_count = Mathf.RoundToInt(playersCountView.currentValue);
		info.auto_start_players_count = Mathf.RoundToInt(autoStartPlayersView.currentValue);
		info.time_bank = Mathf.RoundToInt(timeBankView.currentValue);

		info.password = GetPassText();

		if (info.password.Length < 4)
		{
			var infoPopup = it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info);
			infoPopup.SetDescriptionString("Необходимо ввести пин для входа");
			return;
		}
		Lock(true);
		TableManager.Instance.CreateTableReauest(info, (result) =>
		{
			Lock(false);
			if (result)
				Hide();

		});

		//TableApi.CreateTable(info, (result) =>
		//{

		//	if (result.IsSuccess)
		//	{
		//		TableManager.Instance.AddTable(result.Result);
		//		//_title.text = $"{"popup.createTable.tableName".Localized()}: {result.Result.Name}";
		//		LobbyManager.Instance.OpenTable(result.Result, info.Password);
		//	}
		//	else
		//	{
		//		//_title.text = "popup.createTable.tableNameError".Localized();
		//	}

		//});

		///Api to create table
	}
	//protected override void EnableInit()
	//{
	//	base.EnableInit();
	//	SetTableSettings(defaultSettings);
	//}
}
