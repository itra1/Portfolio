using System.Collections;

using UnityEngine;

namespace it.Game.Environment.Level5.MultiheadArena
{
  public class BoxChamfer : MonoBehaviour
  {
	 [SerializeField]
	 private GameObject _base;
	 [SerializeField]
	 private GameObject _cutter;

	 private void Start()
	 {
		_base.SetActive(true);
		_cutter.SetActive(false);
	 }

	 [ContextMenu("Damage")]
	 public void Damage()
	 {
		_base.SetActive(false);
		_cutter.SetActive(true);

		Rigidbody[] _rbs = _cutter.GetComponentsInChildren<Rigidbody>(true);
		for(int i = 0; i < _rbs.Length; i++)
		{
		  _rbs[i].AddForce(Vector3.up * 10);
		}

	 }

	 [ContextMenu("Clear")]
	 private void Clear()
	 {
		Rigidbody[] _rbs = _cutter.GetComponentsInChildren<Rigidbody>(true);
		for (int i = 0; i < _rbs.Length; i++)
		{
		  _rbs[i].isKinematic = true;
		  _rbs[i].useGravity = false;
		  if (_rbs[i].transform.position.x < 1000)
			 DestroyImmediate(_rbs[i].gameObject);
		}
	 }
  }
}