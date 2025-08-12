using Core.Engine.uGUI.Screens;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Factorys
{
	public abstract class InstanceFactory<TKey, TPrefab> : PrefabFactory<TKey, TPrefab>
	, ISingleInstanceFactory<TKey, TPrefab>
	 where TPrefab : Component
	{
		protected InstanceFactory(Zenject.DiContainer container) : base(container)
		{
		}
	}
}
