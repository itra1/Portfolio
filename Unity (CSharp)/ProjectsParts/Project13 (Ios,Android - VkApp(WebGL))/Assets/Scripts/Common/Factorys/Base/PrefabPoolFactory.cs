using Game.Helpers.Utils;
using UnityEngine;

namespace Game.Common.Factorys.Base {
	public abstract class PrefabPoolFactory<TComponent>
	:Factory<TComponent>
	 where TComponent : Component {
		protected IPrefabPool<TComponent> _prefabPooler;

		public PrefabPoolFactory() {

		}

		protected void InitPooler(TComponent prefab, Transform parent) {
			_prefabPooler = new PrefabPool<TComponent>(prefab, parent);
		}
		public override TComponent GetInstance() {
			return _prefabPooler.GetItem();
		}
	}
}
