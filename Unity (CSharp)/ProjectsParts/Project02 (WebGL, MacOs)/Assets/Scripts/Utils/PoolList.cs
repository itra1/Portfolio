using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolList<T> where T : Component
{
	private GameObject _prefab;
	private Transform _parent;
	private List<T> _instances;

	public PoolList(T prefab, Transform parent):this(prefab.gameObject, parent)
	{
	}
	public PoolList(GameObject prefab, Transform parent)
	{
		this._prefab = prefab;
		this._parent = parent;
		this._instances = new List<T>();
	}

	public T GetItem(){

		T itm = _instances.Find(x => !x.gameObject.activeSelf);

		if (itm == null)
		{
			GameObject obj = GameObject.Instantiate(_prefab, _parent);
			itm = obj.GetComponent<T>();
			_instances.Add(itm);
		}

		itm.gameObject.SetActive(true);
		return itm;
	}

	public void HideAll(){
		_instances.ForEach(x => x.gameObject.SetActive(false));
	}

	public int CountActive => _instances.FindAll(x => x.gameObject.activeSelf).Count;

}
