using Engine.Engine.Scripts.Settings;
using Engine.Scripts.Settings;
using Game.Scripts.Settings.Common;
using StringDrop;
using UnityEngine;

namespace Game.Scripts.Settings
{
	[CreateAssetMenu(fileName = "ApplicationSettings", menuName = "ScriptableObjects/ApplicationSettings")]
	public class ApplicationSettings : ScriptableObject, IApplicationSettings
	{
		[Tooltip("Включает отображение дев элементов")]
		[SerializeField] private bool _devMode;
		[SerializeField] private InputSettingsStruct _inputSettings;
		[SerializeField][StringDropList(typeof(SceneVisibleModeType))] private string _sceneVisibleMode;
		[SerializeField][StringDropList(typeof(GameMissModeType))] private string _gameMissMode;

		public bool DevMode => _devMode;
		public string SceneVisibleMode => _sceneVisibleMode;
		public string GameMissMode => _gameMissMode;
		public IInputSettingsStruct InputSettings => _inputSettings;
	}
}
