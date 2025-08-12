using Garilla.Games;
using it.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Settings
{
	[CreateAssetMenu(fileName = "PlatformSettings", menuName = "Tools/Create Standalone settings", order = 1)]
	public class StandaloneSettings : PlatformSettings<StandaloneSettings>
	{
		//private static StandaloneSettings _instance;
		//public static StandaloneSettings Instance
		//{
		//	get
		//	{
		//		if (_instance == null)
		//			_instance = Resources.Load<StandaloneSettings>("StandaloneSettings");

		//		return _instance;
		//	}
		//}

		public static List<TablePlaceManager> TablesPrefab => Instance._tablesPrefab;
		public static List<PlaceController> TablePlaces => Instance._places;
		public static List<TableInfoPlaces> TableInfoPlaces => Instance._tableInfoPlaces;
		public static List<GameInfoData> GameInfoPrefabs => Instance._gameInfoPrefabs;

		[SerializeField] private List<PlaceController> _places;
		[SerializeField] private List<TableInfoPlaces> _tableInfoPlaces;
		[SerializeField] private List<TablePlaceManager> _tablesPrefab;
		public static PlaceController GetTablePlace(TablePlaceTypes placeType, int placeCount)
		{
			return TablePlaces.Find(x => x.TablePlace == placeType && x.PlaceCount == placeCount);
		}
		public static TablePlaceManager GetTablePlaceManager(it.Network.Rest.Table table, bool checkVip = true)
		{
			var gameBlock = GameSettings.GetBlock(table, checkVip);

			return TablesPrefab.Find(x => (x.Table == LobbyType.None || x.Table == gameBlock.Lobby || gameBlock.Lobby == LobbyType.VipGame)
			&& x.GameTypes.Contains((GameType)table.game_rule_id)
			&& x.IsDealerChoise == table.is_dealer_choice
			&& x.AllOrNothing == table.is_all_or_nothing
			&& (table.is_vip
			|| ((table.MaxPlayers > 2 && !x.FaceToFace) || (table.MaxPlayers <= 2 && x.FaceToFace))));

		}

	}
}