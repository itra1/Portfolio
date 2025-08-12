using System.Reflection;
using Game.Common.Attributes;
using Game.Common.Factorys;
using Game.Providers.Ui.Windows.Base;
using UnityEngine;

namespace Game.Providers.Ui.Windows.Factorys
{
	public class WindowFactory : SingleInstanceFactory<string, Presenter>, IWindowFactory
	{
		public WindowFactory(Zenject.DiContainer container, IWindowsSettings screenSettings) : base(container)
		{
			var allScreens = Resources.LoadAll<Presenter>(screenSettings.ScreensFolder);

			foreach (var wItem in allScreens)
			{
				var attrs = wItem.GetType().GetCustomAttributes<PrefabNameAttribute>();

				foreach (var attr in attrs)
				{
					if (attr is not IPrefabName pName)
						continue;

					if (!_prefabs.ContainsKey(pName.PrefabName))
					{
						_prefabs.Add(pName.PrefabName, wItem);
					}
				}
			}
		}
	}
}