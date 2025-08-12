using Garilla.WebGL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace it.Settings
{

	[CreateAssetMenu(fileName = "Settings", menuName = "Tools/Create settings", order = 1)]
	public class AppSettings : ScriptableObject
	{
//#if UNITY_WEBGL
//		public static Settings Instance => WebGLResources.Settings;
//#else
		private static AppSettings _instance;
		public static AppSettings Instance
		{
			get
			{
				if (_instance == null)
					_instance = (AppSettings)Garilla.ResourceManager.GetResource<AppSettings>("AppSettings");
				//_instance = Resources.Load<Settings>("Settings");
				return _instance;
			}
		}
//#endif

		public static string ReleaseServer => Instance._releaseServer;
		public static string AppKey => Instance._appKey;
		public static string EmailSupport => Instance._emailSupport;
		public static Texture2D Pointer => Instance._pointer;
		public static Texture2D ResizeCursor => Instance._resizeCursor;
		public static FoldersData Folders => Instance._folders;
		public static ServerData Servers => Instance._servers;
		public static List<LanguageSettings> Languages => Instance._languages;
		public static List<CurrencySettings> Currency => Instance._currency;
		public static StickColorData Sticks => Instance._sticks;
		public static ChipsData Chips => Instance._chips;
		public static SpriteAtlas SpriteAtlas => Instance._spriteAtlas;

		[SerializeField] private string _releaseServer = "https://app.garillapoker.com/api/v1";
		[SerializeField] private string _appKey = string.Empty;

#if UNITY_WEBGL
		[SerializeField] private it.UI.UILibrary _webGlUiLibrary;
#endif


		[Space]
		[SerializeField] private string _emailSupport = "support@GarillaPoker.com";
		[SerializeField] private Texture2D _pointer;
		[SerializeField] private Texture2D _resizeCursor;
		[SerializeField] private FoldersData _folders;
		[SerializeField] private ServerData _servers;
		[SerializeField] private SpriteAtlas _spriteAtlas;
		[SerializeField] private List<LanguageSettings> _languages;
		[SerializeField] private List<CurrencySettings> _currency;
		[SerializeField] private StickColorData _sticks;
		[SerializeField] private ChipsData _chips;

		[System.Serializable]
		public class ServerData
		{

			public ConnectType Type;

			public string Server
			{
				get
				{
					return ConfigManager.Configs.ContainsKey("server")
					? ConfigManager.Configs["server"]
					: ServersList.Find(x => x.Type == Type).Server;
				}
			}

			public string ServerApi
			{
				get
				{
					return ConfigManager.Configs.ContainsKey("server")
					? "https://" + Server + "/api/v1"
					: ServersList.Find(x => x.Type == Type).ServerApi;
				}
			}

			public string Socket
			{
				get
				{
					return ConfigManager.Configs.ContainsKey("socket")
					? ConfigManager.Configs["socket"]
					: ServersList.Find(x => x.Type == Type).Socket;
				}
			}

			public List<ConnectData> ServersList;

			[System.Serializable]
			public class ConnectData
			{
				public ConnectType Type;
				public string Server;
				public string ServerApi => "https://" + Server + "/api/v1";
				public string Socket;
			}

			public enum ConnectType { Work, Develop }
		}

		[System.Serializable]
		public class LanguageSettings
		{
			public string Title;
			public string EnglishName;
			public string NativeName;
			public string Code;
			public string IsoCode;
			public Sprite Flag => SpriteAtlas.GetSprite("flag_" + Code);

			public string CodeUpcase => Code.ToUpper();
		}
		[System.Serializable]
		public class CurrencySettings
		{
			public string Title;
			public string Symbol;
			public StringPositionType Position;
		}

		[System.Serializable]
		public class FoldersData
		{
			public string UILibrary;
			public string UIPanels;
		}

		[System.Serializable]
		public class StickColorData
		{

			public List<ColorItem> Sticks;

			[System.Serializable]
			public class ColorItem
			{
				public string Name;
				public Color Color;
			}
		}

		[System.Serializable]
		public class ChipsData
		{

			public List<ChipItem> Chip;

			[System.Serializable]
			public class ChipItem
			{
				public float Value;
				public Color Color;
				public Color ColorDark;
				public Sprite Flag => SpriteAtlas.GetSprite("chip_" + Value);
			}

		}

	}
}