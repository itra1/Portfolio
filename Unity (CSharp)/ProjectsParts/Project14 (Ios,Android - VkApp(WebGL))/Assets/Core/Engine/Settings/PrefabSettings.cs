using System.Collections.Generic;
using UnityEngine;
using Core.Engine.uGUI.Popups;
using Core.Engine.uGUI.Screens;
using Core.Engine.Components.Shop;
using Core.Engine.Components.Skins;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Engine.App.Settings {
	[CreateAssetMenu(fileName = "PrefabSettings", menuName = "App/Create/Settings/PrefabSettings", order = 2)]
	public class PrefabSettings : ScriptableObject, IPrefabSettings {

		[SerializeField] protected MonoBehaviour[] _windowList;
		[SerializeField] protected MonoBehaviour[] _popupList;
		[SerializeField] protected MonoBehaviour[] _shopProductList;
		[SerializeField] protected MonoBehaviour[] _skinsList;

		public IEnumerable<MonoBehaviour> ScreenList => _windowList;
		public IEnumerable<MonoBehaviour> PopupList => _popupList;
		public IEnumerable<MonoBehaviour> ShopProduct => _shopProductList;
		public IEnumerable<MonoBehaviour> SkinsList => _skinsList;

#if UNITY_EDITOR

		[MenuItem("App/Actualize prefab settings &#p")]
		[ContextMenu("Actualize prefab settings")]
		private static void ActualizePrefabSettings() {
			var container = StaticContext.Container;
			var uiSettings = container.TryResolve<IScreenSettings>();
			var popupSettings = container.TryResolve<IPopupSettings>();
			var shopSettings = container.TryResolve<IShopSettings>();
			var skinsSettings = container.TryResolve<ISkinSettings>();

			if (uiSettings == null) {
				Debug.LogError("You need to create prefab settings first");
				return;
			}

			var prefabSettings = container.TryResolve<IPrefabSettings>();

			if (prefabSettings == null) {
				Debug.LogError("You need to create prefab settings first");
				return;
			}

			prefabSettings.Actualize(uiSettings, popupSettings, shopSettings, skinsSettings);
		}

		public void Actualize(IScreenSettings screenSettings
		, IPopupSettings popupSettings
		, IShopSettings shopSettings
		, ISkinSettings skinsSettings) {
			ActializeComponent(screenSettings
			, popupSettings
			, shopSettings
			, skinsSettings);
			EditorUtility.SetDirty(this);
		}

		protected virtual void ActializeComponent(IScreenSettings settings
		, IPopupSettings popupSettings
		, IShopSettings shopSettings
		, ISkinSettings skinsSettings) {
			_windowList = LoadAssets<MonoBehaviour>(settings.ScreensFolder);
			_popupList = LoadAssets<MonoBehaviour>(popupSettings.PopupFolder);
			_shopProductList = LoadAssets<MonoBehaviour>(shopSettings.ShopProductsFolder);
			_skinsList = LoadAssets<MonoBehaviour>(skinsSettings.SkinFolder);
		}

		private T[] LoadAssets<T>(string path) where T : UnityEngine.Object => Resources.LoadAll<T>(path);

#endif
	}
}
