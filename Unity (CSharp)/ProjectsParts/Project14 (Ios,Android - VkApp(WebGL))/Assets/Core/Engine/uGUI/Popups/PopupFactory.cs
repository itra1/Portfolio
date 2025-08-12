using System.Collections.Generic;
using System.Reflection;
using Core.Engine.App.Base.Attributes;
using Core.Engine.App.Settings;
using Core.Engine.Factorys;
using Zenject;

namespace Core.Engine.uGUI.Popups {
	public class PopupFactory
	:SingleInstanceFactory<string, Popup>
	, IPopupFactory {
		public PopupFactory(DiContainer container
		, IPrefabSettings prefabSettings) :
		base(container) {

			_prefabs = new Dictionary<string, Popup>();
			foreach (var wItem in prefabSettings.PopupList) {
				var windowComponent = wItem.GetComponent<Popup>();
				if (windowComponent == null)
					continue;

				var attrs = windowComponent.GetType().GetCustomAttributes<PrefabNameAttribute>();

				foreach (var attr in attrs) {
					if (attr is not IPrefabName pName)
						continue;

					if (!_prefabs.ContainsKey(pName.PrefabName))
						_prefabs.Add(pName.PrefabName, windowComponent);
				}
			}
		}
	}
}
