using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.Audio;
using Core.Engine.Components.Leaderboard;
using Core.Engine.Components.SaveGame;
using Core.Engine.Signals;
using Core.Engine.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core.Engine.uGUI.Screens
{
	[PrefabName(ScreenTypes.Leaderboard)]
	public class LeaderboardScreen :Screen, ILeaderboardScreen
	{
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private LeaderboardItemPanel _itemPrefab;

		private ILeaderboardProvider _leaderboardProvider;
		private PrefabPool<LeaderboardItemPanel> _shopPool;
		private GameLevelSG _gameLevel;

		[Inject]
		public void Initiate(ILeaderboardProvider leaderboardProvider
		,SaveGameProvider saveGameProvider)
		{
			_leaderboardProvider = leaderboardProvider;

			_gameLevel = (GameLevelSG)saveGameProvider.GetProperty<GameLevelSG>();
			_shopPool = new(_itemPrefab,_scrollRect.content);
			_itemPrefab.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			SpawnItems();
		}

		private void SpawnItems()
		{
			_shopPool.HideAll();

			System.Collections.Generic.List<LeaderboarItem> list = _leaderboardProvider.GetAroundItems(_gameLevel.Value);

			for (int i = 0; i < list.Count; i++)
			{
				LeaderboarItem item = list[i];
				LeaderboardItemPanel elem = _shopPool.GetItem();
				elem.Set(item);
				elem.RT.anchoredPosition = new(elem.RT.anchoredPosition.x,-(i * (elem.RT.rect.height + 5)));
				elem.gameObject.SetActive(true);
			}
			_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x,list.Count * (_itemPrefab.RT.rect.height + 5));
		}

		public void BackButtonTouch()
		{
			PlayAudio.PlaySound("click");
			_signalBus.Fire(new UGUIButtonClickSignal() { Name = ButtonTypes.FirstMenuOpen });
		}
	}
}
