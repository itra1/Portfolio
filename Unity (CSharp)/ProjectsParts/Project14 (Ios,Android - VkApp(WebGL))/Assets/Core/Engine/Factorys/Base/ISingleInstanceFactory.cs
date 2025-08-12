using UnityEngine;

namespace Core.Engine.Factorys
{
	public interface ISingleInstanceFactory<TKey, TPrefab> : IPrefabFactory<TKey, TPrefab>
	 where TPrefab : Component
	{
	}
}
