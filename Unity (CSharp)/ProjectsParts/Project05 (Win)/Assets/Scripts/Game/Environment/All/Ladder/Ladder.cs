using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.All.Ladder
{

  public abstract class Ladder : MonoBehaviourBase
  {
	 public List<GameObject> ItemsList => _itemsList;
	 [SerializeField]
	 private GameObject[] _items;
	 [SerializeField]
	 protected List<GameObject> _itemsList = new List<GameObject>();
	 [SerializeField]
	 protected bool _randomItems = true;
	 [SerializeField]
	 protected float _height = 10;
	 [SerializeField]
	 protected float _width = 2;
	 [SerializeField]
	 [Range(0,2)]
	 protected float _cut = 1;

	 protected Transform _itemsParent;

	 private BoxCollider _boxCollider;



	 protected virtual void Spawn()
	 {
		Clear();

		_boxCollider = GetComponentInChildren<BoxCollider>();
		_boxCollider.center = new Vector3(0, _height / 2);
		_boxCollider.size = new Vector3(_width, _height, 0.1f);

		_itemsParent = (new GameObject()).transform;
		_itemsParent.SetParent(transform);
		_itemsParent.localPosition = Vector3.zero;
		_itemsParent.localRotation = Quaternion.identity;
		_itemsParent.localScale = Vector3.one;
		_itemsParent.name = "Items";

	 }

	 [ContextMenu("Clear")]
	 protected void Clear()
	 {

		foreach (var elem in _itemsList)
		  DestroyImmediate(elem);
		_itemsList.Clear();
		if (_itemsParent != null)
		  DestroyImmediate(_itemsParent.gameObject);

	 }

	 protected int _correntItem;


	 protected GameObject GetNextItem()
	 {
		if (_randomItems)
		  return _items[Random.Range(0, _items.Length)];
		else
		{
		  _correntItem++;
		  _correntItem = Mathf.Clamp(_correntItem, 0, _items.Length - 1);
		  return _items[_correntItem];
		}
	 }

	 private void OnDrawGizmosSelected()
	 {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + transform.up * _cut*_height);

	 }

	 protected Transform InstanceItem()
	 {
		GameObject prefab = GetNextItem();
		GameObject inst = Instantiate(prefab, _itemsParent);
		inst.transform.localPosition = Vector3.zero;
		inst.transform.localRotation = Quaternion.identity;
		inst.transform.localScale = Vector3.one;
		inst.gameObject.SetActive(true);
		_itemsList.Add(inst.gameObject);
		return inst.transform;
	 }

  }
}