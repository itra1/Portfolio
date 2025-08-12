using Core.Engine.Utils;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Factorys.Base
{
	public abstract class PrefabPoolFactory<TComponent>
	: Factory<TComponent>
	 where TComponent : Component
	{
		protected IPrefabPool<TComponent> _prefabPooler;

		public PrefabPoolFactory()
		{

		}

		protected void InitPooler(TComponent prefab, Transform parent)
		{
			_prefabPooler = new PrefabPool<TComponent>(prefab, parent);
		}
		public override TComponent GetInstance()
		{
			return _prefabPooler.GetItem();
		}

	}
}
