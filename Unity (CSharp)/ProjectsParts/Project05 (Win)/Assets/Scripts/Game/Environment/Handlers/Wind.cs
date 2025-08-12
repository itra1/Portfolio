using UnityEngine;
using System.Collections;

namespace it.Game.Environment.Wind
{
  public class Wind : MonoBehaviourBase
  {
	 [SerializeField]
	 private Vector3 _vector;
	 [SerializeField]
	 private float _power = 1;

	 private bool _isActive = false;

	 public void Active()
	 {
		if (_isActive)
		  return;
		_isActive = true;
		StartCoroutine(ForceEffect());
	 }

	 public void Disactive()
	 {

		if (!_isActive)
		  return;

		_isActive = false;
		Game.Player.PlayerBehaviour.Instance.Wind = Vector3.zero;
	 }

	 IEnumerator ForceEffect()
	 {
		while (_isActive)
		{
		  Game.Player.PlayerBehaviour.Instance.Wind = (_vector.normalized * _power);
		  
		  yield return new WaitForEndOfFrame();
		}
	 }


#if UNITY_EDITOR
	 private void OnDrawGizmosSelected()
	 {
		Game.Utils.DrawArrow.GizmoToTarget(transform.position, transform.position + (_vector.normalized * _power));
	 }
#endif

  }
}