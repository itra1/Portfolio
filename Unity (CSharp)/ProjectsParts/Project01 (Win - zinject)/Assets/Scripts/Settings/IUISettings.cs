using System.Collections.Generic;
using Settings.Data;
using UnityEngine;

namespace Settings
{
	public interface IUISettings
	{
		string AudioMixersFolder { get; }
		string ShadedElementsFolder { get; }
		string ScreenModesFolder { get; }
		string DesktopsFolder { get; }
		string PresentationsFolder { get; }
		string StatusesFolder { get; }
		string FloatingWindowsFolder { get; }
		string WindowAdaptersFolder { get; }
		string WindowsFolder { get; }
		string WidgetsFolder { get; }
		string ComponentsFolder { get; }
		string RenderStreamingFolder { get; }
		LayerMask MapLayer { get; }
		IReadOnlyList<GraphColorData> GraphColors { get; }
		IReadOnlyList<Sprite> AuthSprites { get; }
		IReadOnlyList<Color> AuthColors { get; }
		IReadOnlyList<MouseCursorInfo> MouseCursors { get; }
		WidgetColors WidgetColors { get; }
		IReadOnlyList<WeatherData> Weather { get; }
		TextAsset DistrictsTable { get; }
		TextAsset RegionsTable { get; }
		IReadOnlyList<Texture2D> RegionalHeadAvatars { get; }

		Color GetGraphColor(string colorName);
		Sprite GetWindowMaterialIconSprite(WindowMaterialIconType type);
		WeatherData GetWeatherData(string key);
		Texture2D GetRegionalHeadAvatar(string avatarName);
	}
}