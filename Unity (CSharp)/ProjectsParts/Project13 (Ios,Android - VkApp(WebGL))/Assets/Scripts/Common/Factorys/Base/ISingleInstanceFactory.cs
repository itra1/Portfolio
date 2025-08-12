using UnityEngine;

namespace Game.Common.Factorys {
	public interface ISingleInstanceFactory<TKey, TPrefab> :IPrefabFactory<TKey, TPrefab>
	 where TPrefab : Component {
	}
}
