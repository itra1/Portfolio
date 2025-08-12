using System.Collections.Generic;
using Game.Base;
using UnityEngine;

namespace Game.Common.Factorys.Base
{
	public class MultiInstanceFactory<TKey, TPrefab> : PrefabFactory<TKey, TPrefab>
	where TPrefab : Component
	{
		public MultiInstanceFactory(Zenject.DiContainer container) : base(container) { }

		protected IDictionary<TKey, List<TPrefab>> Instances = new Dictionary<TKey, List<TPrefab>>();

		public override TPrefab GetInstance(TKey key, Transform parent)
		{
			if (!Instances.ContainsKey(key))
				Instances.Add(key, new());

			var existsInstance = Instances[key].Find(x => !x.gameObject.activeSelf);

			if (existsInstance != null)
				return existsInstance;

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

			Instances[key].Add(component);
			return component;
		}
	}
}
