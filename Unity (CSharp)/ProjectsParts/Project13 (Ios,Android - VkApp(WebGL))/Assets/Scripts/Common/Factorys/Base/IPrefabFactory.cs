using UnityEngine;

namespace Game.Common.Factorys {
	public interface IPrefabFactory<TKey, TPrefab> :IFactory<TKey, TPrefab>
	where TPrefab : Component {
	}
	public interface IPrefabFactory<TComponent> :IFactory<TComponent>
	where TComponent : Component {
	}
}
