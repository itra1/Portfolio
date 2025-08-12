using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using I2.Loc;
using Sett = it.Settings;
using UnityEngine.UI;
using JetBrains.Annotations;
using System;
using it.UI.Elements;

public class LanguageSettingsPage : MonoBehaviour
{
	[SerializeField] private it.UI.Elements.GraphicButtonUI _appluButton;
	[SerializeField] private ButtonData[] _buttons;

	private string _selectCode;

	[System.Serializable]
	public struct ButtonData
	{
		public string Code;
		public GraphicButtonUI Button;
		public bool IsDevelop;
		[HideInInspector] public LocalizationButton Controller;
	}

	private void Awake()
	{

		for (int i = 0; i < _buttons.Length; i++)
		{
			int index = i;
			_buttons[index].Controller = _buttons[index].Button.gameObject.AddComponent<LocalizationButton>();
			_buttons[index].Button.GetComponentInChildren<TextMeshProUGUI>().text = Sett.AppSettings.Languages.Find(x => x.Code == _buttons[i].Code).NativeName;
			string code = _buttons[i].Code;
			_buttons[index].Button.OnClick.RemoveAllListeners();
			_buttons[index].Button.OnClick.AddListener(() =>
			{
				if (_buttons[index].IsDevelop)
				{
					it.Main.PopupController.Instance.ShowPopup(PopupType.Develop);
					return;
				}

				if (_selectCode == code) return;
				_selectCode = code;

				if (_appluButton != null)
					_appluButton.interactable = true;
				SetLanguage(_selectCode);
			});
		}
	}

	private void OnEnable()
	{
		if (_appluButton != null)
			_appluButton.interactable = false;
		StartCoroutine(EnableCor());
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, SettingsUpdateEvent);
	}

	private void OnDisable()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, SettingsUpdateEvent);
#if !UNITY_STANDALONE
		ApplyButton();
#endif
	}
	private void SettingsUpdateEvent(com.ootii.Messages.IMessage handle)
	{
		_selectCode = I2.Loc.LocalizationManager.CurrentLanguageCode;
	}

	IEnumerator EnableCor()
	{
		_selectCode = I2.Loc.LocalizationManager.CurrentLanguageCode;
		yield return null;

		//for (int i = 0; i < _buttons.Length; i++)
		//	_buttons[i].Button.gameObject.SetActive(true);
		I2.Loc.LocalizationManager.OnLocalizeEvent -= ConfirmPanel;
		I2.Loc.LocalizationManager.OnLocalizeEvent += ConfirmPanel;
		//for (int i = 0; i < _buttons.Length; i++)
		//	SetShadow(i);
		SetLanguage(I2.Loc.LocalizationManager.CurrentLanguageCode);
	}

	private void ConfirmPanel()
	{
		SetLanguage(I2.Loc.LocalizationManager.CurrentLanguageCode);
	}

	private void SetLanguage(string code)
	{
		var lng = Sett.AppSettings.Languages.Find(x => x.Code.ToLower() == code.ToLower());

		for (int i = 0; i < _buttons.Length; i++)
		{
			_buttons[i].Controller.IsSelect = _buttons[i].Code.ToLower() == lng.Code.ToLower();
			//if (_buttons[i].Code.ToLower() == lng.Code.ToLower())
			//	SetLight(i);
			//else
			//	SetShadow(i);
		}
	}

	//private void SetLight(int index)
	//{
	//	var bt = _buttons[index].Button;
	//	bt.StartColor = Color.white;
	//	//bt.NormalState();

	//	it.UI.Elements.TextButtonUI txt = bt.GetComponent<it.UI.Elements.TextButtonUI>();
	//	if (txt == null)
	//		txt = GetComponentInChildren<it.UI.Elements.TextButtonUI>();
	//	txt.StartColor = Color.white;
	//	//txt.NormalState();
	//}

	//private void SetShadow(int index)
	//{
	//	var bt = _buttons[index].Button;
	//	Color w = Color.white;
	//	w.a = 0.2f;
	//	bt.StartColor = w;
	//	//bt.NormalState();

	//	it.UI.Elements.TextButtonUI txt = bt.GetComponent<it.UI.Elements.TextButtonUI>();
	//	if (txt == null)
	//		txt = bt.GetComponentInChildren<it.UI.Elements.TextButtonUI>();
	//	txt.StartColor = Color.white * 0.47f;
	//	//txt.NormalState();
	//}

	public void CancelButton()
	{
		SetLanguage(_selectCode = I2.Loc.LocalizationManager.CurrentLanguageCode);
	}

	public void ApplyButton()
	{
		LocalizeController.Instance.SelLocale(_selectCode);
	}

	public class LocalizationButton : MonoBehaviour
	{
		public Image Image;
		public TextMeshProUGUI Label;

		private bool _isSelect;

		public bool IsSelect
		{
			get
			{
				return _isSelect;
			}
			set
			{
				//if (_isSelect == value) return;
				_isSelect = value;
				Image.color = _isSelect ? Color.white : Color.gray;
				Label.color = _isSelect ? Color.white : Color.gray;
			}
		}

		public void Awake()
		{
			Image = transform.Find("Image").GetComponent<Image>();
			Label = transform.Find("Label").GetComponent<TextMeshProUGUI>();
		}

	}

}
