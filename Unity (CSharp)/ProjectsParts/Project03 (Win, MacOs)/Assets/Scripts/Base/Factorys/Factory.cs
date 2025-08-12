using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zenject;
using UnityEngine;

namespace Base.Factorys
{
	public abstract class Factory<TComponent> : IFactory<TComponent>
	 where TComponent : Component
	{
		protected Zenject.DiContainer _container;

		[Inject]
		public void InitFactory(Zenject.DiContainer container)
		{
			_container = container;
		}

		public abstract TComponent GetInstance();
	}
	public abstract class Factory<TKey, TPrefab> : IFactory<TKey, TPrefab>
	 where TPrefab : Component
	{
		protected Zenject.DiContainer _container;

		public Factory(Zenject.DiContainer container)
		{
			_container = container;
		}

		public abstract TPrefab GetInstance(TKey key, Transform parent);

	}
}
