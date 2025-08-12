using Base.Factorys;
using Settings.Prefabs;
using System.Collections.Generic;
using UGui.Screens.Base;
using Base.Attributes;
using System.Reflection;
using UnityEngine;
using Screen = UGui.Screens.Base.Screen;

namespace UGui.Screens.Factorys
{
	public class ScreenFactory
	: SingleInstanceFactory<string, Screen>
	, IScreenFactory
	{

		public ScreenFactory(Zenject.DiContainer container, IPrefabSettings prefabSettings) : base(container)
		{
			_prefabs = new Dictionary<string, Screen>();
			foreach (var wItem in prefabSettings.ScreenList)
			{
				var windowComponent = wItem.GetComponent<Screen>();
				if (windowComponent == null) continue;

				var attrs = windowComponent.GetType().GetCustomAttributes<PrefabNameAttribute>();

				foreach (var attr in attrs)
				{
					if (attr is not IPrefabName pName) continue;

					if (!_prefabs.ContainsKey(pName.PrefabName))
						_prefabs.Add(pName.PrefabName, windowComponent);
				}
			}
		}

	}
}
