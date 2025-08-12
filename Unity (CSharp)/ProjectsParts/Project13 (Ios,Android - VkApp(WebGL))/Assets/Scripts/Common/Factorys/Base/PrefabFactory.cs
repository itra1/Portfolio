using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game.Common.Factorys
{

	public abstract class PrefabFactory<TKey, TPrefab> : Factory<TKey, TPrefab>
		, IPrefabFactory<TKey, TPrefab>
		 where TPrefab : Component
	{
		protected IDictionary<TKey, TPrefab> _prefabs { get; set; } = new Dictionary<TKey, TPrefab>();

		public IDictionary<TKey, TPrefab> Prefabs => _prefabs;

		public PrefabFactory(DiContainer container) : base(container)
		{

		}

		public override TPrefab GetInstance(TKey key, Transform parent)
		{
			if (!_prefabs.ContainsKey(key))
				return null;

			return _prefabs[key];
		}
		public TPrefab GetRandomInstance(Transform parent)
		{
			return GetInstance(GetRandomKey(), parent);
		}
		public TPrefab GetRandomInstance(List<TKey> keyList, Transform parent)
		{
			return GetInstance(keyList[Random.Range(0, keyList.Count)], parent);
		}

		public TKey GetRandomKey()
		{
			return _prefabs.ElementAt(Random.Range(0, _prefabs.Keys.Count)).Key;
		}
	}
}
