using System.Collections.Generic;

using UGui.Screens.Base;
using UnityEditor;

using UnityEngine;

using Zenject;

namespace Settings.Prefabs
{
	[CreateAssetMenu(fileName = "PrefabSettings", menuName = "Settings/PrefabSettings")]
	public class PrefabSettings: ScriptableObject, IPrefabSettings
	{
		public IEnumerable<MonoBehaviour> ScreenList => _screenList;

		[SerializeField] protected MonoBehaviour[] _screenList;

#if UNITY_EDITOR


		[MenuItem("App/Actualize prefab settings &#p")]
		[ContextMenu("Actualize prefab settings")]
		private static void ActualizePrefabSettings()
		{
			var container = StaticContext.Container;
			var settings = container.TryResolve<ISettings>();

			if (settings == null)
			{
				Debug.LogError("You need to create prefab settings first");
				return;
			}

			var prefabSettings = container.TryResolve<IPrefabSettings>();

			if (prefabSettings == null)
			{
				Debug.LogError("You need to create prefab settings first");
				return;
			}

			prefabSettings.Actualize(settings);
		}

		public void Actualize(ISettings settings)
		{
			ActializeComponent(settings);
			EditorUtility.SetDirty(this);
		}

		protected virtual void ActializeComponent(ISettings settings)
		{
			_screenList = LoadAssets<MonoBehaviour>(settings.ScreensFolder);
		}

		private T[] LoadAssets<T>(string path) where T : UnityEngine.Object => Resources.LoadAll<T>(path);

#endif

	}
}
