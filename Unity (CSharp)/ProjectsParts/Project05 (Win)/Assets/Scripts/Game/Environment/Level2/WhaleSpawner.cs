using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.Level2
{
  public class WhaleSpawner : Environment
  {

	 /// <summary>
	 /// Расположение префаба в ресурсах
	 /// </summary>
	 [SerializeField]
	 private string _prefabResources;
	 private List<KeyValuePair<float, GameObject>> _instanceList = new List<KeyValuePair<float, GameObject>>();

	 /// <summary>
	 /// Загруженный префаб
	 /// </summary>
	 private GameObject _prefab;

	 [SerializeField]
	 private int _countInstance;
	 [SerializeField]
	 private RangeFloat _speedSpan;
	 [SerializeField]
	 private RangeFloat _sizeSpan;

	 [SerializeField]
	 private Vector3 _centerRegions = Vector3.zero;
	 [SerializeField]
	 private float _radiusRegion = 5;
	 [SerializeField]
	 private float _heightGerion = 50;

	 protected override void Start()
	 {
		base.Start();
		LoadPrefab();
		FillList();
		//VisibleList();
		StartCoroutine(ProcessCoroutine());
	 }

	 IEnumerator ProcessCoroutine()
	 {
		while (true)
		{
		  yield return new WaitForSeconds(10);

		  VisibleList();
		  CheckOut();
		}
	 }

	 private void FillList()
	 {
		int count = _countInstance - 1;
		if (count <= 0)
		  count = 1;
		float oneHeidht = _heightGerion / count;
		float minHeight = _centerRegions.y - _heightGerion / 2;
		for (int i = 0; i < _countInstance; i++)
		{
		  _instanceList.Add(new KeyValuePair<float, GameObject>(minHeight + oneHeidht * i, GetInstance()));
		}
	 }

	 private void LoadPrefab()
	 {

		if (_prefab == null)
		  _prefab = (GameObject)Resources.Load<GameObject>(_prefabResources);
	 }

	 private GameObject GetInstance()
	 {
		GameObject inst = Instantiate(_prefab);
		inst.gameObject.SetActive(false);
		return inst;
	 }

	 private void CheckOut()
	 {
		float dist = 0;
		float newDist = 0;
		for (int i = 0; i < _instanceList.Count; i++)
		{
		  dist = (_instanceList[i].Value.transform.position - _centerRegions).magnitude;
		  newDist = ((_instanceList[i].Value.transform.position + _instanceList[i].Value.transform.forward) - _centerRegions).magnitude;
		  if (dist > _radiusRegion && newDist > dist)
		  {
			 _instanceList[i].Value.SetActive(false);
		  }
		}
	 }

	 private void VisibleList()
	 {
		for (int i = 0; i < _instanceList.Count; i++)
		{

		  if (_instanceList[i].Value.activeInHierarchy)
			 continue;

		  Vector3 norm = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
		  Vector3 position = new Vector3(_centerRegions.x, _instanceList[i].Key, _centerRegions.z) + norm * _radiusRegion;
		  _instanceList[i].Value.transform.rotation = Quaternion.LookRotation(-norm, Vector3.up) * Quaternion.Euler(0, Random.Range(-60f, 60f), 0);
		  _instanceList[i].Value.transform.position = position;
		  _instanceList[i].Value.SetActive(true);
		  float scale = _sizeSpan.RandomRange;
		  _instanceList[i].Value.transform.localScale = Vector3.one * scale;
		  float speed = _speedSpan.RandomRange;
		  _instanceList[i].Value.GetComponent<it.Game.NPC.Motions.ForwardMove>().Speed = speed;
		  _instanceList[i].Value.GetComponent<Animator>().SetFloat("speed", (speed / _speedSpan.Max) / scale);

		  return;
		}
	 }

#if UNITY_EDITOR

	 private void OnDrawGizmosSelected()
	 {

		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(_centerRegions, 1);
		//Gizmos.DrawWireSphere(_centerRegions, _radiusRegion);
		Gizmos.DrawWireCube(_centerRegions + Vector3.up * _heightGerion / 2,
		  new Vector3(_radiusRegion * 2, 0.1f, _radiusRegion * 2));
		Gizmos.DrawWireCube(_centerRegions - Vector3.up * _heightGerion / 2,
		  new Vector3(_radiusRegion * 2, 0.1f, _radiusRegion * 2));

	 }
#endif
  }
}