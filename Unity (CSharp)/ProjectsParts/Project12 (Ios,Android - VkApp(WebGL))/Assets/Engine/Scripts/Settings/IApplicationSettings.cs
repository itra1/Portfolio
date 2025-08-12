using Engine.Scripts.Settings;

namespace Engine.Engine.Scripts.Settings
{
	public interface IApplicationSettings
	{
		/// <summary>
		/// Включает отображение дев элементов
		/// </summary>
		bool DevMode { get; }
		string SceneVisibleMode { get; }
		string GameMissMode { get; }
		IInputSettingsStruct InputSettings { get; }
	}
}
