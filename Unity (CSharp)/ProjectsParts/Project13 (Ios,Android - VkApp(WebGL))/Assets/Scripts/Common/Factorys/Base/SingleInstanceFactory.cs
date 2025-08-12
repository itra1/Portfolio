using System.Collections.Generic;
using Game.Base;
using UnityEngine;

namespace Game.Common.Factorys
{
	public abstract class SingleInstanceFactory<TKey, TPrefab> : PrefabFactory<TKey, TPrefab>
	, ISingleInstanceFactory<TKey, TPrefab>
	 where TPrefab : Component
	{

		public SingleInstanceFactory(Zenject.DiContainer container) : base(container) { }

		protected IDictionary<TKey, TPrefab> Instances = new Dictionary<TKey, TPrefab>();

		public override TPrefab GetInstance(TKey key, Transform parent)
		{
			if (Instances.ContainsKey(key))
				return Instances[key];

			var prefab = base.GetInstance(key, parent);

			if (prefab == null)
			{
				AppLog.LogError($"No exists prefab in factory {this.GetType()}");
				return null;
			}
			prefab.gameObject.SetActive(false);

			var instancePrefab = Behaviour.Instantiate(prefab.gameObject, parent);

			if (!instancePrefab.TryGetComponent<TPrefab>(out var component))
			{
				AppLog.LogError($"Prefab {instancePrefab.name} no conteins component {typeof(TPrefab)}");
				return null;
			}

			_container.Inject(component);

			//TODO не есть хорошо, но пока так
			var injectList = component.GetComponentsInChildren<IInjection>(true);
			for (var i = 0; i < injectList.Length; i++)
				_container.Inject(injectList[i]);

			Instances.Add(key, component);
			return component;
		}
	}
}
