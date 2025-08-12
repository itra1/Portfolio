using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class TableThemeSettingsPage : MonoBehaviour
{
	[SerializeField] private it.UI.Elements.GraphicButtonUI _applyButton;
	[SerializeField] private TMPro.TMP_Dropdown _backDeckDropDown;
	[SerializeField] private TMPro.TMP_Dropdown _frontDeckDropDown;
	[SerializeField] private RawImage _backTable;
	[SerializeField] private RawImage _tableImage;
	[SerializeField] private Image _frontDeckImage;
	[SerializeField] private Image _backDeckImage;
	[SerializeField] private ButtonsStruct[] _foltButtons;
	[SerializeField] private ButtonsStruct[] _backgroundButtons;
	[SerializeField] private CardSettings[] _cards;
	[SerializeField] private CardSettings[] _backs;


	private UserProfilePost _pp;
	private Texture _startTableSprite;
	private Texture _startBackSprite;

	[System.Serializable]
	public struct ButtonsStruct
	{
		public it.UI.Elements.GraphicButtonUI Button;
		public RectTransform _select;
	}

	private void Awake()
	{
		_backDeckDropDown.onValueChanged.RemoveAllListeners();
		_frontDeckDropDown.onValueChanged.RemoveAllListeners();
		_backDeckDropDown.onValueChanged.AddListener((int val) =>
		{
			_pp.table_theme.back_deck = (val + 1).ToString();
			_applyButton.interactable = true;
			ConfirmValue();
		});
		_frontDeckDropDown.onValueChanged.AddListener((int val) =>
		{
			_pp.table_theme.front_deck = (val + 1).ToString();
			_applyButton.interactable = true;
			ConfirmValue();
		});

		for (int i = 0; i < _foltButtons.Length; i++)
		{
			int index = i;
			_foltButtons[index].Button.OnClick.RemoveAllListeners();
			_foltButtons[index].Button.OnClick.AddListener(() =>
			{
				_pp.table_theme.felt = (index + 1).ToString();
				_applyButton.interactable = true;
				ConfirmValue();
			});
		}
		for (int i = 0; i < _backgroundButtons.Length; i++)
		{
			int index = i;
			if (_backgroundButtons[index].Button == null) continue;
			_backgroundButtons[index].Button.OnClick.RemoveAllListeners();
			_backgroundButtons[index].Button.OnClick.AddListener(() =>
			{
				_pp.table_theme.background = (index + 1).ToString();
				_applyButton.interactable = true;
				ConfirmValue();
			});
		}

		if (_startTableSprite == null)
			_startTableSprite = _tableImage.texture;

		if (_startBackSprite == null)
			_startBackSprite = _backTable.texture;
	}

	private void OnEnable()
	{
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.SettingsUpdate, SettingsUpdateEvent);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, SettingsUpdateEvent);

		_pp = UserController.User.user_profile.PostProfile;
		_applyButton.interactable = false;
		ConfirmValue();
	}
	private void OnDisable()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.SettingsUpdate, SettingsUpdateEvent);
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, SettingsUpdateEvent);
#if !UNITY_STANDALONE
		ApplyButtonTouch();
#endif
	}
	private void SettingsUpdateEvent(com.ootii.Messages.IMessage handle)
	{
		_pp = UserController.User.user_profile.PostProfile;
		ConfirmValue();
	}

	private void ConfirmValue()
	{
		_backDeckImage.sprite = it.Settings.GameSettings.BackDeckStyles[int.Parse(_pp.table_theme.back_deck) - 1];
		_frontDeckImage.sprite = it.Settings.GameSettings.FrontDeckStyles[int.Parse(_pp.table_theme.front_deck) - 1];

		for (int i = 0; i < _foltButtons.Length; i++)
		{
			_foltButtons[i]._select.gameObject.SetActive(_pp.table_theme.felt != "default" && int.Parse(_pp.table_theme.felt) - 1 == i);
		}
		for (int i = 0; i < _cards.Length; i++)
		{
			_cards[i].SetData(int.Parse(_pp.table_theme.front_deck) - 1);
		}


		for (int i = 0; i < _backgroundButtons.Length; i++)
		{
			if (_backgroundButtons[i]._select == null) continue;
			_backgroundButtons[i]._select.gameObject.SetActive(_pp.table_theme.background != "default" && int.Parse(_pp.table_theme.background) - 1 == i);
		}
		for (int i = 0; i < _backs.Length; i++)
		{
			_backs[i].SetData(int.Parse(_pp.table_theme.back_deck) - 1);
		}

		_tableImage.texture = it.Settings.GameSettings.GameTheme.GetTableTextureFast("Round", _pp.table_theme.felt == "default" ? "1" : _pp.table_theme.felt, true);
		_backTable.texture = it.Settings.GameSettings.GameTheme.GetBackTableTextureFast(_pp.table_theme.background);

	}

	public void TableSelect(int index)
	{
		_tableImage.texture = it.Settings.GameSettings.GameTheme.GetTableTextureFast("Round", index.ToString(), true);
	}
	public void BackSelect(int index)
	{
		_backTable.texture = it.Settings.GameSettings.GameTheme.GetBackTableTextureFast((index + 1).ToString());
	}

	public void ApplyButtonTouch()
	{
		UserController.Instance.UpdateProfile(_pp);
	}
	public void DefaultButtonTouch()
	{
		_pp.table_theme.felt = "default";
		_pp.table_theme.background = "default";
		_pp.table_theme.back_deck = "1";
		_pp.table_theme.front_deck = "1";
		_tableImage.texture = _startTableSprite;
		_backTable.texture = _startBackSprite;
		_applyButton.interactable = true;
		ConfirmValue();
	}

}