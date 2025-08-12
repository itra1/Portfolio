using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using it.Main;
using Garilla.UI;

public class MobileTablesUIManager : Singleton<MobileTablesUIManager>
{
	[SerializeField] private RectTransform _baseWindow;
	[SerializeField] private MobileAppNavigation _navigations;
	[SerializeField] private BaseBlocks[] _pages;
	[SerializeField] private RectTransform _contentBlock;
	[SerializeField] private RectTransform tableChoiceSwitchable;
	[Header("Game Name Label")]
	[SerializeField] private TMP_Text GameNameLabel;
	public RectTransform ContentBlock { get => _contentBlock; set => _contentBlock = value; }
	public MobileAppNavigation Navigations { get => _navigations; set => _navigations = value; }

	public string CurrentPage => _pages[_currentIndex].Pagetitle;

	private int _currentIndex;
	private bool _isAnimate = false;
	private TableInfoManager _tableInfo;

	[System.Serializable]
	private struct BaseBlocks
	{
		public string Pagetitle;
		public RectTransform Page;
		public TMP_Text SelectElement;
		public Color defaultColor;
		public Color targetColor;
	}

	public GameObject SelectPage(string titlePage)
	{
		if (_isAnimate) return _pages[_currentIndex].Page.gameObject;

		int newIndex = GetIndex(titlePage);

		if (_tableInfo == null)
			_tableInfo = gameObject.GetComponentInChildren<TableInfoManager>(true);

		if (_tableInfo != null)
			_tableInfo.DisableAll();

		if (newIndex == _currentIndex)
		{
			var p = _pages[newIndex].Page.GetComponent<it.Mobile.Main.LobbyPage>();
			if (p != null)
				p.StartPage();

			return _pages[newIndex].Page.gameObject;
		}

		bool isLeft = newIndex < _currentIndex;
		BaseBlocks targetBlock = _pages[newIndex];
		BaseBlocks oldBlock = _pages[_currentIndex];


		targetBlock.Page.anchoredPosition = new Vector2(oldBlock.Page.anchoredPosition.x + (isLeft ? -_contentBlock.rect.width : _contentBlock.rect.width), oldBlock.Page.anchoredPosition.y);

		targetBlock.Page.gameObject.SetActive(true);
		_isAnimate = true;
		_currentIndex = newIndex;
		com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.PageSelect, targetBlock.Pagetitle, 0.01f);

		_contentBlock.DOAnchorPos(new Vector2(_contentBlock.anchoredPosition.x + (isLeft ? _contentBlock.rect.width : -_contentBlock.rect.width), _contentBlock.anchoredPosition.y), 0.3f).OnComplete(() =>
		{
			oldBlock.SelectElement.color = oldBlock.defaultColor;
			_isAnimate = false;
			targetBlock.SelectElement.color = targetBlock.targetColor;
			oldBlock.Page.gameObject.SetActive(false);
		});

		return targetBlock.Page.gameObject;
	}

	private int GetIndex(string title)
	{
		for (int i = 0; i < _pages.Length; i++)
		{
			if (_pages[i].Pagetitle == title)
			{
				return i;
			}
		}
		return 0;
	}

	private void Awake()
	{
#if UNITY_IOS

		if(_baseWindow != null){
			_baseWindow.sizeDelta = new Vector2(0, -100);
			_baseWindow.anchoredPosition = new Vector2(0, -50);
		}

#endif

		_navigations.OnNewsEvent = () => { SelectPage("News"); };
		_navigations.OnWclEvent = () => { SelectPage("Leaderboard"); };
		_navigations.OnMainEvent = () => { SelectPage("Main"); };
		_navigations.OnProfileEvent = () =>
		{
			if (!UserController.IsLogin)
			{
				it.Logger.Log("check login");
				it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
				return;
			}
			else
				SelectPage("Profile");

		};
		_navigations.OnSettingsEvent = () => { SelectPage("Settings"); };
	}


	private void OnEnable()
	{
		_contentBlock.anchoredPosition = Vector2.zero;
		_currentIndex = 2;
		for (int i = 0; i < _pages.Length; i++)
		{
			_pages[i].Page.gameObject.SetActive(i == _currentIndex);
			//_pages[i].SelectElement.gameObject.SetActive(i == _currentIndex);

		}
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserUnauthorized, UserNotAuthorized);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserUnauthorized, UserNotAuthorized);
	}

	private void UserNotAuthorized(com.ootii.Messages.IMessage handle)
	{
		PopupController.Instance.ShowPopup(PopupType.Authorization);

		SelectPage("Main");
	}

	enum ScreenType { game = 0, tourment = 1, main = 2, settings = 3, profile = 4, table = 5 };

	public void MainScreenSwitch(int screenChoice)
	{
		tableChoiceSwitchable.gameObject.SetActive((int)ScreenType.table == screenChoice);
	}

	private string[] GameTypeNames = new string[]
	{
				"Holdem",
				"PLO",
				"Short Deck",
				"All or Nothing",
				"VIP Games",
				"Face to Face",
				"OFC",
				"MTT",
				"GP Millions"
	};


	public void SetGameTypeLabel(int GameType)
	{
		GameNameLabel.text = GameTypeNames[GameType];
	}

	//private void Awake()
	//{
	//	MainScreenSwitch(2);
	//}
}

/// <summary>
/// Базовый класс страниц в лобби
/// </summary>
public interface MobilePageBase
{

}