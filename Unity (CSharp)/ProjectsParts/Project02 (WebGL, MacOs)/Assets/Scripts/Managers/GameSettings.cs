using Garilla.Games;
using Garilla.WebGL;

using it.Network.Rest;
using it.UI;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace it.Settings
{

	[CreateAssetMenu(fileName = "GameSettings", menuName = "Tools/Create game settings", order = 1)]
	public class GameSettings : ScriptableObject
	{
		//#if UNITY_WEBGL
		//		public static GameSettings Instance => WebGLResources.GameSettings;
		//#else
		private static GameSettings _instance;
		public static GameSettings Instance
		{
			get
			{
				if (_instance == null)
					_instance = (GameSettings)Garilla.ResourceManager.GetResource<GameSettings>("GameSettings");
				//_instance = Resources.Load<GameSettings>("GameSettings");

				return _instance;
			}
		}
		//#endif

		public static List<GaneType> Games => Instance._games;
		public static List<GameBlock> Blocks => Instance._blocks;
		public static List<GameColors> Colors => Instance._colors;
		public static List<TableStylesData> TableStyle => Instance._tableStyles;
		//public static List<Garilla.Games.UI.HandHistoryTable> HandTablesPrefab => Instance._handTablesPrefab;
		public static List<RankData> RankSettings => Instance._rankSettings;
		public static Sprite[] BackDeckStyles => Instance._backDeckStyles;
		public static Sprite[] FrontDeckStyles => Instance._frontDeckStyles;
		public static List<ChipsBlock> ChipsData => Instance._chipsData;
		public static List<GameNameTypes> GameNames => Instance._gameNames;
		public static GameInfoPanel GameInfoPanelBlock => Instance._gameInfoPane;
		public static GameThemeData GameTheme => Instance._gameTheme;
		public static BadBeatOptions BadBeat => Instance._badBeat;
		//public static List<GameInfoData> GameInfoPrefabs => Instance._gameInfoPrefabs;

		[SerializeField] private List<GaneType> _games;
		[SerializeField] private List<GameNameTypes> _gameNames;
		[SerializeField] private List<GameBlock> _blocks;
		[SerializeField] private List<GameColors> _colors;
		[SerializeField] private List<TableStylesData> _tableStyles;
		[SerializeField] private List<Garilla.Games.UI.HandHistoryTable> _handTablesPrefab;
		[SerializeField] private List<RankData> _rankSettings;
		[SerializeField] private Sprite[] _backDeckStyles;
		[SerializeField] private Sprite[] _frontDeckStyles;
		[SerializeField] private List<ChipsBlock> _chipsData;
		[SerializeField] private GameInfoPanel _gameInfoPane;
		[SerializeField] private GameThemeData _gameTheme;
		[SerializeField] private BadBeatOptions _badBeat;
		//[SerializeField] private List<GameInfoData> _gameInfoPrefabs;

		[System.Serializable]
		public class GameBlock
		{
			public string Name;
			public string ShortName;
			public AppGameRules Rules;
			public LobbyType Lobby;
			public List<GameType> TypeGame = new List<GameType>();
			public int MaxPlayers;
			public int MinPlayers = 3;
			public bool IsPrivate;
			public bool AllOrNothing;
			public bool IsVip;
			public bool IsDealerChoice;
			public string keyRulesLink;
		}

		[System.Serializable]
		public class BadBeatOptions{
			public List<GameType> GameTypes;
		}

		[System.Serializable]
		public class GaneType
		{
			public string Name;
			public string SlugRequest;
			public bool IsSmartHud;
			public Sprite RectIcone;
			public NabigationsUse NavigationFor;

		}

		[System.Serializable]
		public class GameColors
		{
			public string Name;
			public string NameUp;
			public List<GameType> TypeGame = new List<GameType>();
			public Color Color;
		}

		[System.Serializable]
		public class RankData
		{
			public RankType Type;
			public string Slug;
			public Sprite SpriteInfo;
			public Sprite SpriteProfile;
			public Sprite SpriteMain;

			public Sprite GetSprite(int index)
			{
				if (index == 1)
					return SpriteProfile;
				if (index == 2)
					return SpriteMain;
				return SpriteInfo;
			}

		}

		public static GameBlock GetBlock(it.Network.Rest.Table table, bool checkVip = true) => Blocks.Find(x =>
				(checkVip && (x.IsVip && x.IsVip == table.is_vip))
				|| (table.is_dealer_choice && x.IsDealerChoice == table.is_dealer_choice)
				||
				((!checkVip || !table.is_vip)
				&& !table.is_dealer_choice
				&& x.MaxPlayers >= table.MaxPlayers
				&& x.AllOrNothing == table.is_all_or_nothing
				&& x.TypeGame.Contains((GameType)table.game_rule_id)
				)

				);

		//public static Garilla.Games.UI.HandHistoryTable GetHandHistoryTable(it.Network.Rest.Table table)
		//{
		//	var gameBlock = GetBlock(table);

		//	return HandTablesPrefab.Find(x => x.Table == gameBlock.Lobby && (gameBlock.Lobby != LobbyType.Plo || gameBlock.TypeGame.Contains((GameType)table.game_rule_id)));
		//}

		//[ContextMenu("Copy")]
		//public void CopyData()
		//{
		//	_blocks = Blocks;
		//	_colors = Colors;
		//	_handTablesPrefab = HandTablesPrefab;
		//}
		[System.Serializable]
		public class GameNameTypes
		{
			public string Name;
			public string Slug;
			public AppGameRules Rules;
			public GameType GameType;
		}

		[System.Serializable]
		public class ChipsBlock
		{
			public string Name;
			public List<ChipsSprite> Chips;
		}

		[System.Serializable]
		public class ChipsSprite
		{
			public double Min;
			public double Max;
			public Sprite Sprite;
		}

		public Sprite GetChipSprite(string name, double value)
		{
			var chipList = _chipsData.Find(x => x.Name == name);
			if (chipList == null)
				return _chipsData[0].Chips[0].Sprite;

			for (int i = 0; i < chipList.Chips.Count; i++)
			{
				if (i == chipList.Chips.Count - 1)
					return chipList.Chips[i].Sprite;
				if (chipList.Chips[i].Min <= value && chipList.Chips[i].Max > value)
					return chipList.Chips[i].Sprite;
			}
			return _chipsData[0].Chips[0].Sprite;

		}

		//[System.Serializable]
		//public class GameInfoData
		//{
		//	public string PrefabName;
		//	public List<GameType> GameTypes;
		//	public bool IsAllOrNofing;
		//	public bool IsDealerChoise;
		//}

		[System.Serializable]
		public class GameInfoPanel
		{
			public it.UI.TableInfoItem[] InfoList;
		}

		[System.Serializable]
		public class GameThemeData
		{
			[HideInInspector] public Texture BackTableTheme;

			private string _backKey;
			private string _tableKey;

			public void SetStyle(string table, string back)
			{
				_backKey = back;
				_tableKey = table;
				BackTableTheme = GetBackTableTextureFast(_backKey);
			}

			public Texture GetBackTableTextureFast(string key, bool ignoteTheme = false)
			{
				//return key == "default" || ignoteTheme
				//	? Resources.Load<Texture>($"{StringConstants.RESOURCES_TEXTURES}/TableBacks/TableBacks1")
				//	: Resources.Load<Texture>($"{StringConstants.RESOURCES_TEXTURES}/TableBacks/TableBacks{key}");
				return key == "default" || ignoteTheme
					? (Texture)Garilla.ResourceManager.GetResource<Texture>($"{StringConstants.RESOURCES_TEXTURES}/TableBacks/TableBacks1")
					: (Texture)Garilla.ResourceManager.GetResource<Texture>($"{StringConstants.RESOURCES_TEXTURES}/TableBacks/TableBacks{key}");

			}
			public Texture GetTableTextureFast(string subString, string key, bool ignoteTheme = false)
			{
				string userKey = key;
				if (!ignoteTheme && _tableKey != "default")
					userKey = _tableKey;

				//var spr = Resources.Load<Texture>($"{StringConstants.RESOURCES_TEXTURES}/Tables/{subString}{userKey}");
				var spr = (Texture)Garilla.ResourceManager.GetResource<Texture>($"{StringConstants.RESOURCES_TEXTURES}/Tables/{subString}{userKey}");

				if (spr != null)
					return spr;

				//return Resources.Load<Texture>($"{StringConstants.RESOURCES_TEXTURES}/Tables/{subString}{key}");
				return (Texture)Garilla.ResourceManager.GetResource<Texture>($"{StringConstants.RESOURCES_TEXTURES}/Tables/{subString}{key}");
			}

		}
		public static string GetFullVisibleNameOnTable(Table table)
		{
			GameBlock gb = GetBlock(table);
			var gm = GameNames.Find(x => x.GameType == (GameType)table.game_rule_id);

			if (table.is_vip)
				return $"VIP {gm.Name} {table.name}";

			if (table.is_all_or_nothing)
				if (GameType.Holdem == (GameType)table.game_rule_id)
					return $"AoN HOLDEM {table.name}";
				else
					return $"AoN OMAHA {table.name}";

			if (table.is_dealer_choice)
				return $"Dealer Choice {table.name}";

			if (gb.Lobby == LobbyType.Plo)
				return $"OMAHA {gm.Name} {table.name}";

			return $"{gm.Name} {table.name}";
		}

		public static string GetFullVisibleName(Table table)
		{
			GameBlock gb = GetBlock(table);
			var gm = GameNames.Find(x => x.GameType == (GameType)table.game_rule_id);

			if (table.is_vip)
				return $"VIP {gm.Name} {table.name}";

			if (table.is_all_or_nothing)
				if (GameType.Holdem == (GameType)table.game_rule_id)
					return $"AoN HOLDEM {table.name}";
				else
					return $"AoN OMAHA {table.name}";

			if (table.is_dealer_choice)
				return $"DC {table.name}";

			if (gb.Lobby == LobbyType.Plo)
				return $"{gm.Name} {table.name}";

			return table.name;
		}

		[System.Serializable]
		public class TableStylesData
		{
			public TableStyle Style;
			public bool IsDealerChoice;
			public List<GameType> GameType;
			public string Key;
		}

	}
}