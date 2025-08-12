using System.Reflection;
using Game.Common.Attributes;
using Game.Common.Factorys;
using Game.Providers.Ui.Popups.Base;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Popups.Factorys
{
	public class PopupFactory : SingleInstanceFactory<string, Popup>, IPopupFactory
	{
		public PopupFactory(DiContainer container, IPopupSettings popupSettings) : base(container)
		{
			var allScreens = Resources.LoadAll<Popup>(popupSettings.PopupsFolder);
			foreach (var wItem in allScreens)
			{
				var attrs = wItem.GetType().GetCustomAttributes<PrefabNameAttribute>();

				foreach (var attr in attrs)
				{
					if (attr is not IPrefabName pName)
						continue;

					if (!_prefabs.ContainsKey(pName.PrefabName))
						_prefabs.Add(pName.PrefabName, wItem);
				}
			}
		}
	}
}
