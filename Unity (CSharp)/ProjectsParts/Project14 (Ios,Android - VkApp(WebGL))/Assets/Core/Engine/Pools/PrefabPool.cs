using System.Collections.Generic;
using Core.Engine.Factorys.Base;
using UnityEngine;

namespace Core.Engine.Utils
{
	public class PrefabPool<TComponent> :IPrefabPool<TComponent>
	where TComponent : Component
	{
		private GameObject _prefab;
		private Transform _parent;
		private List<TComponent> _instances = new();

		private bool _isInitAfterCreate;

		public int CountActive => _instances.FindAll(x => x.gameObject.activeSelf).Count;

		public PrefabPool(TComponent prefab,Transform parent)
		: this(prefab.gameObject,parent)
		{
		}

		public PrefabPool(GameObject prefab,Transform parent)
		{
			this._prefab = prefab;
			this._parent = parent;
			_prefab.SetActive(false);
			_isInitAfterCreate = typeof(TComponent).GetInterface(nameof(IInitAfterCreatePool)) != null;
		}

		public IPrefabPool<TComponent> InitInstances(int count)
		{
			while (_instances.Count < count)
			{
				AddInstance();
			}
			return this;
		}

		public TComponent GetItem()
		{
			var itm = _instances.Find(x => !x.gameObject.activeSelf);

			if (itm == null)
			{
				itm = AddInstance();
			}
			return itm;
		}

		private TComponent AddInstance()
		{
			var obj = GameObject.Instantiate(_prefab,_parent);

			var components = obj.GetComponentsInChildren<IFactoryInstantiateAfter>();

			foreach (var elem in components)
				elem.AfterFactoryCreate();

			var itm = obj.GetComponent<TComponent>();
			_instances.Add(itm);

			if (_isInitAfterCreate)
				(itm as IInitAfterCreatePool).InitAfterCreatePool();

			return itm;
		}

		public IPrefabPool<TComponent> HideAll()
		{
			_instances.ForEach(x => x.gameObject.SetActive(false));
			return this;
		}

	}
}