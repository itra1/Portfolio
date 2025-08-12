using Core.Engine.Components.Shop;
using Core.Engine.Components.Skins;
using Core.Engine.uGUI.Popups;
using Core.Engine.uGUI.Screens;

using System.Collections.Generic;

using UnityEngine;

namespace Core.Engine.App.Settings
{
	public interface IPrefabSettings
	{
		IEnumerable<MonoBehaviour> ScreenList { get; }
		IEnumerable<MonoBehaviour> PopupList { get; }
		IEnumerable<MonoBehaviour> ShopProduct { get; }
		IEnumerable<MonoBehaviour> SkinsList { get; }

#if UNITY_EDITOR
		void Actualize(IScreenSettings settings
		, IPopupSettings popupSettings
		, IShopSettings shopSettings
		, ISkinSettings skinsSettings);
#endif
	}
}
