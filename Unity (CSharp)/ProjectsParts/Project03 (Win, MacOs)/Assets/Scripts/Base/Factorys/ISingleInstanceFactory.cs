
using UnityEngine;

namespace Base.Factorys
{
	public interface ISingleInstanceFactory<TKey, TPrefab> : IPrefabFactory<TKey, TPrefab>
	 where TPrefab : Component
	{
	}
}
