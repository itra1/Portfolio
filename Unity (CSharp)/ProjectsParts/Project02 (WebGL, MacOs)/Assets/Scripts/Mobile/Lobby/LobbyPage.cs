using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using it.Mobile.Lobby;
using it.Main;
using System.Linq;
using DG.Tweening;
using it.Popups;
using it.UI.Elements;
using TMPro;
using it.Network.Rest;
using it.Settings;

namespace it.Mobile.Main
{
	public class LobbyPage : MonoBehaviour, MobilePageBase
	{
		[SerializeField] private LobbyNavigations _navigations;
		[SerializeField] private RectTransform _content;
		[SerializeField] private Garilla.UI.TableInfoManager _tableInfoManager;
		[SerializeField] private ScrollRect _menuScroll;
		[SerializeField] private RectTransform _casinoBlock;

		private List<GameListPage> _pages;
		private RectTransform _rt;
		private LobbyType _selectedPage;

		private void Awake()
		{
#if UNITY_IOS

			RemoveCasinoBlock();

#endif

			_rt = GetComponent<RectTransform>();
			_pages = GetComponentsInChildren<GameListPage>(true).ToList();
			for (int i = 0; i < _pages.Count; i++)
			{
				_pages[i].gameObject.SetActive(false);
				_pages[i].OnInfoButton = (table) =>
				{
					_tableInfoManager.Open(table);
				};
			}

			_navigations.OnSelectPage = SelectPage;
		}

		private void OnEnable()
		{
			_content.gameObject.SetActive(false);
			_navigations.gameObject.SetActive(true);
			Clear();
		}

		private void RemoveCasinoBlock()
		{

			if (_menuScroll != null && _casinoBlock != null)
			{
				_casinoBlock.gameObject.SetActive(false);
				_menuScroll.content.sizeDelta = new Vector2(_menuScroll.content.sizeDelta.x, _menuScroll.content.sizeDelta.y - _casinoBlock.rect.height);
			}

		}

		public void PlaySound()
		{
			//#if !UNITY_WEBGL
			//DarkTonic.MasterAudio.MasterAudio.PlaySound(StringConstants.SOUND_GAME_TIMER, 1);
			//DarkTonic.MasterAudio.MasterAudio.PlaySound(StringConstants.SOUND_BUTTON_CLICK, 1);
			//#endif
		}

		public void StartPage()
		{
			if (_selectedPage != LobbyType.None)
				BackTouch();
		}

		public void Clear()
		{
			_selectedPage = LobbyType.None;
			_content.gameObject.SetActive(false);
			_navigations.gameObject.SetActive(true);
		}

		public void BackTouch()
		{
			_selectedPage = LobbyType.None;
			_navigations.gameObject.SetActive(true);
			_content.DOAnchorPos(new Vector2(_content.rect.width, _content.anchoredPosition.y), 0.3f).OnComplete(() =>
			{
				_content.gameObject.SetActive(false);
			});
		}
		public void OpenLobbyByTable(Table table)
		{
			SelectPage(GameSettings.GetBlock(table).Lobby);
		}

		public void SelectPage(LobbyType type)
		{
			_selectedPage = type;

			for (int i = 0; i < _pages.Count; i++)
				_pages[i].gameObject.SetActive(false);
			var targetPage = _pages.Find(x => x.Lobby == _selectedPage);
			var topPanel = targetPage.GetComponentInChildren<TopMobilePanel>();
			topPanel.OnBackAction.AddListener(() =>
			{
				_selectedPage = LobbyType.None;
				_navigations.gameObject.SetActive(true);
				targetPage.gameObject.SetActive(false);
			});
			targetPage.gameObject.SetActive(true);

			_content.anchoredPosition = new Vector2(_content.rect.width, _content.anchoredPosition.y);
			_content.gameObject.SetActive(true);
			_content.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
			{
				_navigations.gameObject.SetActive(false);
			});
		}

		public void CreateTable()
		{
			TableManager.Instance.CreateTable();
		}

	}
}