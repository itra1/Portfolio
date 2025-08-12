using System.Reflection;
using Game.Common.Attributes;
using Game.Common.Factorys.Base;
using Game.Game.Settings;
using UnityEngine;
using Zenject;

namespace Game.Game.Elements.Barriers
{
	public class BarrierFactory : MultiInstanceFactory<string, Barrier>
	{
		public BarrierFactory(DiContainer container, GameSettings gameSettings) : base(container)
		{
			var allScreens = Resources.LoadAll<Barrier>(gameSettings.BarrierFolder);
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
