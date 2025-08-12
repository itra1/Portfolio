using System.Collections.Generic;
using Engine.Scripts.Base;
using itra.Attributes;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI
{
	public abstract class UiFactory<TObject>
	where TObject : MonoBehaviour
	{
		protected Dictionary<string, TObject> _prefabs = new();
		protected Dictionary<string, TObject> _instances = new();
		protected DiContainer _container;

		private RectTransform _parentInstances;

		public UiFactory(DiContainer container)
		{
			_container = container;
		}

		protected void LoadResouurces(string path, RectTransform parent)
		{
			_parentInstances = parent;

			var sourcePrefabs = Resources.LoadAll<TObject>(path);

			foreach (var sourcePrefab in sourcePrefabs)
			{
				var attr = sourcePrefab.GetType().GetAttribute<PrefabNameAttribute>();

				if (attr != null)
				{
					if (_prefabs.ContainsKey(attr.Name))
					{
						Debug.LogError("Дублирование имени на префабе");
						continue;
					}

					_prefabs.Add(attr.Name, sourcePrefab);
				}
			}
		}

		public TObject GetInstance(string name)
		{
			if (_instances.ContainsKey(name))
				return _instances[name];

			if (!_prefabs.ContainsKey(name))
			{
				throw new System.Exception("Отсутствует префаб с таким именем");
			}
			var prefab = _prefabs[name];
			prefab.gameObject.SetActive(false);

			var instance = MonoBehaviour.Instantiate(prefab.gameObject, _parentInstances);

			var component = instance.GetComponent<TObject>();

			_container.Inject(component);

			var components = component.GetComponentsInChildren<IInjection>();
			for (int i = 0; i < components.Length; i++)
				_container.Inject(components[i]);

			return component;
		}
	}
}
