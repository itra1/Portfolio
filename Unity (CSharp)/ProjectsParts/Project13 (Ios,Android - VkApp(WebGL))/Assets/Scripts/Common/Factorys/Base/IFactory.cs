using UnityEngine;

namespace Game.Common.Factorys {
	public interface IFactory<TComponent>
	where TComponent : Component {
		TComponent GetInstance();
	}
	public interface IFactory<TKey, TPrefab>
	where TPrefab : Component {
		TPrefab GetInstance(TKey key, Transform parent);
	}
}
