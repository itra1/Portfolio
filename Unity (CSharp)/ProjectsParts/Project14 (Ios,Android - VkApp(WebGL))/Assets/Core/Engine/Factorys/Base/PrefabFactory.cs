using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.Engine.Factorys {
	public abstract class PrefabFactory<TKey, TPrefab> :Factory<TKey, TPrefab>
		, IPrefabFactory<TKey, TPrefab>
		 where TPrefab : Component {
		protected IDictionary<TKey, TPrefab> _prefabs { get; set; } = new Dictionary<TKey, TPrefab>();

		public PrefabFactory(DiContainer container) : base(container) {

		}

		public override TPrefab GetInstance(TKey key, Transform parent) {
			if (!_prefabs.ContainsKey(key))
				return null;

			return _prefabs[key];
		}

	}
}
