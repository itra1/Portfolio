using UnityEngine;

namespace Game.Helpers.Utils {
	public interface IPrefabPool<TComponent>
	:IPool
	where TComponent : Component {
		int CountActive { get; }
		IPrefabPool<TComponent> InitInstances(int count);
		IPrefabPool<TComponent> HideAll();
		TComponent GetItem();

	}

}
