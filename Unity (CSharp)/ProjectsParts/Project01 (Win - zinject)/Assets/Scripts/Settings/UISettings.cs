using System.Collections.Generic;
using System.Linq;
using Settings.Data;
using UnityEngine;

namespace Settings
{
	[CreateAssetMenu(fileName = "UISettings", menuName = "Settings/UISettings", order = 1)]
	public class UISettings : ScriptableObject, IUISettings
	{
		[SerializeField] private string _audioMixersFolder = "AudioMixers";
		[SerializeField] private string _shadedElementsFolder = "Prefabs/UI/ShadedElements";
		[SerializeField] private string _screenModesFolder = "Prefabs/UI/ScreenModes";
		[SerializeField] private string _desktopsFolder = "Prefabs/UI/Desktops";
		[SerializeField] private string _presentationsFolder = "Prefabs/UI/Presentations";
		[SerializeField] private string _statusesFolder = "Prefabs/UI/Statuses";
		[SerializeField] private string _floatingWindowsFolder = "Prefabs/UI/FloatingWindows";
		[SerializeField] private string _windowAdaptersFolder = "Prefabs/UI/WindowAdapters";
		[SerializeField] private string _windowsFolder = "Prefabs/UI/Windows";
		[SerializeField] private string _widgetsFolder = "Prefabs/UI/Widgets";
		[SerializeField] private string _componentsFolder = "Prefabs/UI/Components";
		[SerializeField] private string _renderStreamingFolder = "Prefabs/RenderStreaming";
		
		[SerializeField] private LayerMask _mapLayer;
		[SerializeField] private GraphColorData[] _graphColors;
		[SerializeField] private Sprite[] _authSprites;
		[SerializeField] private Color[] _authColors;
		[SerializeField] private MouseCursorInfo[] _mouseCursors;
		[SerializeField] private WidgetColors _widgetColors;
		[SerializeField] private WindowMaterialIcon[] _windowMaterialIcons;
		[SerializeField] private List<WeatherData> _weather;
		[SerializeField] private TextAsset _districtsTable;
		[SerializeField] private TextAsset _regionsTable;
		[SerializeField] private Texture2D[] _regionalHeadAvatars;
		
		private IDictionary<MouseCursorState, MouseCursorInfo> _mouseCursorsByState;
		private IDictionary<WindowMaterialIconType, WindowMaterialIcon> _windowMaterialIconsByType;
		private IDictionary<string, WeatherData> _weatherByKey;
		private IDictionary<string, Texture2D> _regionalHeadsAvatarsByName;
		
		public string AudioMixersFolder => _audioMixersFolder;
		public string ShadedElementsFolder => _shadedElementsFolder;
		public string ScreenModesFolder => _screenModesFolder;
		public string DesktopsFolder => _desktopsFolder;
		public string PresentationsFolder => _presentationsFolder;
		public string StatusesFolder => _statusesFolder;
		public string FloatingWindowsFolder => _floatingWindowsFolder;
		public string WindowAdaptersFolder => _windowAdaptersFolder;
		public string WindowsFolder => _windowsFolder;
		public string WidgetsFolder => _widgetsFolder;
		public string ComponentsFolder => _componentsFolder;
		public string RenderStreamingFolder => _renderStreamingFolder;
		
		public LayerMask MapLayer => _mapLayer;
		public IReadOnlyList<GraphColorData> GraphColors => _graphColors;
		public IReadOnlyList<Sprite> AuthSprites => _authSprites;
		public IReadOnlyList<Color> AuthColors => _authColors;
		public IReadOnlyList<MouseCursorInfo> MouseCursors => _mouseCursors;
		public WidgetColors WidgetColors => _widgetColors;
		public IReadOnlyList<WeatherData> Weather => _weather;
		public TextAsset DistrictsTable => _districtsTable;
		public TextAsset RegionsTable => _regionsTable;
		public IReadOnlyList<Texture2D> RegionalHeadAvatars => _regionalHeadAvatars;

		public Color GetGraphColor(string colorName)
		{
			foreach (var graphColor in _graphColors)
			{
				if (graphColor.Name == colorName)
					return graphColor.Color;
			}
			
			Debug.LogError($"Graph color named \"{colorName}\" was not found in UI settings");
			
			return _graphColors.FirstOrDefault().Color;
		}
		
		public Sprite GetWindowMaterialIconSprite(WindowMaterialIconType type)
		{
			WindowMaterialIcon icon;
			
			if (_windowMaterialIconsByType == null)
			{
				_windowMaterialIconsByType = new Dictionary<WindowMaterialIconType, WindowMaterialIcon>();

				for (var i = 0; i < _windowMaterialIcons.Length; i++)
				{
					icon = _windowMaterialIcons[i];
					
					if (!_windowMaterialIconsByType.TryAdd(icon.Type, icon))
						Debug.LogError($"An attempt was detected to add a duplicate material icon type \"{icon.Type}\" as a key to the dictionary in UI settings");
				}
			}
			
			if (_windowMaterialIconsByType.TryGetValue(type, out icon))
				return icon.Sprite;
			
			Debug.LogError($"Window material icon with type \"{type}\" was not found in UI settings");
			
			if (_windowMaterialIconsByType.TryGetValue(WindowMaterialIconType.None, out icon))
				return icon.Sprite;
			
			Debug.LogError($"Window material icon with default type \"{WindowMaterialIconType.None}\" was also not found in UI settings");
			return default;
		}

		public WeatherData GetWeatherData(string key)
		{
			WeatherData data;
			
			if (_weatherByKey == null)
			{
				_weatherByKey = new Dictionary<string, WeatherData>();

				for (var i = 0; i < _weather.Count; i++)
				{
					data = _weather[i];
					
					if (!_weatherByKey.TryAdd(data.Key, data))
						Debug.LogError($"An attempt was detected to add a duplicate key \"{data.Key}\" to the dictionary in UI settings");
				}
			}
			
			if (_weatherByKey.TryGetValue(key, out data))
				return data;
			
			Debug.LogError($"Weather data with key \"{key}\" was not found in the UI settings");
			return default;
		}
		
		public Texture2D GetRegionalHeadAvatar(string avatarName)
		{
			Texture2D avatar;
			
			if (_regionalHeadsAvatarsByName == null)
			{
				_regionalHeadsAvatarsByName = new Dictionary<string, Texture2D>();
				
				for (var i = 0; i < _regionalHeadAvatars.Length; i++)
				{
					avatar = _regionalHeadAvatars[i];
					
					if (!_regionalHeadsAvatarsByName.TryAdd(avatar.name, avatar))
						Debug.LogError($"An attempt was detected to add a duplicate key \"{avatar.name}\" to the dictionary in the UI settings");
				}
			}
			
			if (_regionalHeadsAvatarsByName.TryGetValue(avatarName, out avatar))
				return avatar;
			
			Debug.LogError($"Regional head avatar with name \"{avatarName}\" was not found in the UI settings");
			return default;
		}
	}
}