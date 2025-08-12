using Garilla.Games;
using Garilla.WebGL;
using it.Network.Rest;
using it.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Settings
{
	[CreateAssetMenu(fileName = "PlatformSettings", menuName = "Tools/Create android settings", order = 1)]
	public class AndroidSettings : PlatformSettings<AndroidSettings>
	{
//#if UNITY_WEBGL
//		public static AndroidSettings Instance => WebGLResources.AndroidSettings;
//#else
		//private static AndroidSettings _instance;
		//public static AndroidSettings Instance
		//{
		//	get
		//	{
		//		if (_instance == null)
		//			_instance = Resources.Load<AndroidSettings>("AndroidSettings");

		//		return _instance;
		//	}
		//}
//#endif

		public static List<TablePlaceManager> TablesPrefab => Instance._tablesPrefab;
		public static List<TablePlaceManager> LobbyTablesPrefab => Instance._lobbyTablesPrefab;
		//public static List<PlaceController> LobbyPlaces => Instance._lobbyPlaces;
		//public static List<PlaceController> GamePlaces => Instance._gamePlaces;
		public static List<GameInfoData> GameInfoPrefabs => Instance._gameInfoPrefabs;
		//public static List<TableInfoPlaces> TableInfoPlaces => Instance._tableInfoPlaces;

		//[SerializeField] private List<PlaceController> _lobbyPlaces;
		//[SerializeField] private List<PlaceController> _gamePlaces;
		//[SerializeField] private List<TableInfoPlaces> _tableInfoPlaces;
		[SerializeField] private List<TablePlaceManager> _tablesPrefab;
		[SerializeField] private List<TablePlaceManager> _lobbyTablesPrefab;

		public static PlaceController GetTableLobbyPlaceStatic(TablePlaceTypes placeType, int placeCount) => Instance.GetTableLobbyPlace(placeType, placeCount);
		public PlaceController GetTableLobbyPlace(TablePlaceTypes placeType, int placeCount)
		{
			return ((GameObject)Garilla.ResourceManager.GetResource<GameObject>($"Prefabs/UI/Lobby/Places/Place{placeType.ToString()}{placeCount}")).GetComponent<PlaceController>();

			//return LobbyPlaces.Find(x => x.TablePlace == placeType && x.PlaceCount == placeCount);
		}
		public static PlaceController GetGameTablePlaceStatic(TablePlaceTypes placeType, int placeCount) => Instance.GetGameTablePlace(placeType, placeCount);
		public PlaceController GetGameTablePlace(TablePlaceTypes placeType, int placeCount)
		{
			it.Logger.Log("[Game positions]");

			return ((GameObject)Garilla.ResourceManager.GetResource<GameObject>($"Prefabs/Game/Places/Place{placeType.ToString()}{placeCount}")).GetComponent<PlaceController>();
			//return GamePlaces.Find(x => x.TablePlace == placeType && x.PlaceCount == placeCount);
		}
		public static TablePlaceManager GetTablePlaceManagerStatic(it.Network.Rest.Table table, bool checkVip = true) => Instance.GetTablePlaceManager(table, checkVip);
		public TablePlaceManager GetTablePlaceManager(it.Network.Rest.Table table, bool checkVip = true)
		{
			var gameBlock = GameSettings.GetBlock(table, checkVip);

			return TablesPrefab.Find(x => (x.Table == LobbyType.None || x.Table == gameBlock.Lobby || gameBlock.Lobby == LobbyType.VipGame)
			&& x.GameTypes.Contains((GameType)table.game_rule_id)
			&& x.IsDealerChoise == table.is_dealer_choice
			&& x.AllOrNothing == table.is_all_or_nothing
			&& (table.is_vip
			|| ((table.MaxPlayers > 2 && !x.FaceToFace) || (table.MaxPlayers <= 2 && x.FaceToFace))));

		}
		public static TablePlaceManager GetLobbyTablePlaceManagerStatic(it.Network.Rest.Table table, bool checkVip = true) => Instance.GetLobbyTablePlaceManager(table, checkVip);
		public TablePlaceManager GetLobbyTablePlaceManager(it.Network.Rest.Table table, bool checkVip = true)
		{
			var gameBlock = GameSettings.GetBlock(table, checkVip);

			return LobbyTablesPrefab.Find(x => (x.Table == LobbyType.None || x.Table == gameBlock.Lobby || gameBlock.Lobby == LobbyType.VipGame)
			&& x.GameTypes.Contains((GameType)table.game_rule_id)
			&& x.IsDealerChoise == table.is_dealer_choice
			&& x.AllOrNothing == table.is_all_or_nothing
			//&& (/*table.IsVip
			//||*/ ((table.MaxPlayers > 2 && !x.FaceToFace) || (table.MaxPlayers <= 2 && x.FaceToFace))));
			&& (table.is_vip
			|| ((table.MaxPlayers > 2 && !x.FaceToFace) || (table.MaxPlayers <= 2 && x.FaceToFace))));

		}
	}
}