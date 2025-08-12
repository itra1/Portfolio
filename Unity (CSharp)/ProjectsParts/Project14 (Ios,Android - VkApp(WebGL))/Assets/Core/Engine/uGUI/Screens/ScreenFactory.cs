using System.Collections.Generic;
using System.Reflection;
using Core.Engine.App.Base.Attributes;
using Core.Engine.App.Settings;
using Core.Engine.Factorys;
using UnityEngine;

namespace Core.Engine.uGUI.Screens
{
	public class ScreenFactory
	:SingleInstanceFactory<string, Screen>
	, IScreenFactory
	{

		public ScreenFactory(Zenject.DiContainer container, IPrefabSettings prefabSettings) : base(container)
		{
			_prefabs = new Dictionary<string, Screen>();
			foreach (var wItem in prefabSettings.ScreenList)
			{
				var windowComponent = wItem.GetComponent<Screen>();
				if (windowComponent == null)
					continue;

				var attrs = windowComponent.GetType().GetCustomAttributes<PrefabNameAttribute>();

				foreach (var attr in attrs)
				{
					if (attr is not IPrefabName pName)
						continue;

					if (!_prefabs.ContainsKey(pName.PrefabName))
					{
						_prefabs.Add(pName.PrefabName, windowComponent);
					}
				}
			}
			Debug.Log("Screens " + _prefabs.Count);
		}

	}
}
