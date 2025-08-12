using System.Collections.Generic;
using UGui.Screens.Common;
using UnityEngine;

namespace Base.Factorys
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
				Debug.LogError($"No exists prefab in factory {this.GetType()}");
				return null;
			}
			prefab.gameObject.SetActive(false);
			var instancePrefab = Behaviour.Instantiate(prefab.gameObject, parent);
			TPrefab component = instancePrefab.GetComponent<TPrefab>();

			if (component == null)
			{
				Debug.LogError($"Prefab {instancePrefab.name} no conteins component {typeof(TPrefab)}");
				return null;
			}

			_container.Inject(component);

			//TODO не есть хорошо, но пока так
			var injectList = component.GetComponentsInChildren<IZInjection>(true);
			for (int i = 0; i < injectList.Length; i++)
				_container.Inject(injectList[i]);

			Instances.Add(key, component);
			return component;

		}
	}
}
