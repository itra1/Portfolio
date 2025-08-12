using System.Reflection;
using Game.Common.Attributes;
using Game.Common.Factorys.Base;
using Game.Providers.Ui.Items;
using Game.Providers.Ui.Settings;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Factorys
{
	public class UiPlayerResourcesFactory : MultiInstanceFactory<string, FlyingItem>
	{
		public UiPlayerResourcesFactory(DiContainer container, FlyingResourcesSettings settings) : base(container)
		{

			var allScreens = Resources.LoadAll<FlyingItem>(settings.Path);
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
