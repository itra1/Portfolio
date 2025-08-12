using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Timing;
using com.ootii.Geometry;

namespace Game.Assets.Scripts.Game.Tools
{
  [ExecuteInEditMode]
  public class TestCalcPath : MonoBehaviour
  {
	 public Transform target;
	 public float rotationSpeed = 1;
	 private Vector3 toTarget;
	 private Quaternion ForvardRot;
	 private Quaternion left;
	 private Quaternion right;
	 private Vector3 checkPositionStart;
	 private Vector3 checkPositionEnd;
	 RaycastHit _hit1;
	 RaycastHit _hit2;

	 private Quaternion Rotate;
	 private Vector3 lVerticalDirection;
	 private Vector3 lLateralDirection;

	 private void Update()
	 {
		toTarget = target.transform.position - transform.position;
		ForvardRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
		left = ForvardRot * Quaternion.Euler(0, -90, 0);
		right = ForvardRot * Quaternion.Euler(0, 90, 0);
		checkPositionStart = transform.position + (toTarget.normalized * 2);
		checkPositionStart += Vector3.up*2;
		checkPositionEnd = transform.position + toTarget - (toTarget.normalized*2);
		checkPositionEnd += Vector3.up*2;

		RaycastExt.SafeRaycast(checkPositionStart, left * Vector3.forward, out _hit1, 5f, -1, transform);
		RaycastExt.SafeRaycast(checkPositionEnd, left * Vector3.forward, out _hit2, 5f, -1, transform);

		Vector3 lDirection = _hit2.point - _hit1.point;

		Rotate = Quaternion.LookRotation(lDirection);
		lVerticalDirection = Vector3.Project(lDirection, left * Vector3.forward);
		lLateralDirection = lDirection - lVerticalDirection;
	 }

	 private void OnDrawGizmos()
	 {
		Gizmos.DrawLine(transform.position, transform.position + ForvardRot * Vector3.forward*5);
		Gizmos.DrawLine(transform.position, transform.position + ForvardRot * Quaternion.Euler(0, 90, 0) * Vector3.forward * 5);

		if(_hit1.collider != null)
		{
		  Gizmos.color = Color.red;
		  //Gizmos.DrawSphere(_hit1.point, 0.1f);
		}
		if (_hit2.collider != null)
		{
		  Gizmos.color = Color.yellow;
		  //Gizmos.DrawSphere(_hit2.point, 0.1f);
		}
		Gizmos.DrawLine(_hit1.point, _hit2.point);

		Gizmos.color = Color.magenta;

		//Gizmos.DrawLine(_hit1.point, _hit1.point + Rotate * Vector3.forward);
		//Gizmos.DrawLine(_hit1.point, _hit1.point + lVerticalDirection);
		Gizmos.DrawLine(_hit1.point, _hit1.point + lLateralDirection);


		//Gizmos.DrawLine(transform.position, transform.position + ForvardRot * Quaternion.Euler(0, -90, 0) * Vector3.forward * 5);
		//Gizmos.DrawLine(transform.position, transform.position + ForvardRot * Vector3.up * 5);
		//Gizmos.DrawLine(transform.position, transform.position + ForvardRot * Vector3.left * 5);
		//Gizmos.DrawLine(transform.position, transform.position + ForvardRot * Vector3.right * 5);

		//Gizmos.DrawLine(transform.position, transform.position + toTarget);
		//Gizmos.DrawLine(transform.position, checkPositionStart);
		//Gizmos.DrawLine(checkPositionStart, checkPositionStart + left.eulerAngles);
		//Gizmos.DrawLine(checkPositionStart, checkPositionStart + right.eulerAngles);
	 }

  }
}