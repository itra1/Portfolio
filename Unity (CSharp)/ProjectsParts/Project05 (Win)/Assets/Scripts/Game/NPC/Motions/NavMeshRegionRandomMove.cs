using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using com.ootii.Geometry;
using com.ootii.Timing;
using com.ootii.Actors;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace it.Game.NPC.Motions
{

  public class NavMeshRegionRandomMove : NavMeshDriver
  {
	 [SerializeField]
	 private RangeFloat _radiusFindPoint;
	 private NavMeshPath _path;
	 [SerializeField]
	 private LayerMask _layerRegion;
	 protected override void Awake()
    {
      base.Awake();

      mInputSource = null;
	 }

	 private void OnDrawGizmos()
	 {
		Gizmos.DrawLine(transform.position, mAgentDestination);

#if UNITY_EDITOR

		UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, _radiusFindPoint.Min);
		UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, _radiusFindPoint.Max);
#endif

		if (mNavMeshAgent != null && mNavMeshAgent.hasPath && mNavMeshAgent.path.corners.Length > 0)
		{
		  for (int i = 0; i < mNavMeshAgent.path.corners.Length; i++)
		  {
			 if (i < mNavMeshAgent.path.corners.Length - 1)
				Gizmos.DrawLine(mNavMeshAgent.path.corners[i], mNavMeshAgent.path.corners[i + 1]);
		  }
		}
	 }
	 protected override void SetAnimatorProperties(Vector3 rInput, Vector3 rMovement, Quaternion rRotation)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		// Determine the simulated input
		if (rMovement.sqrMagnitude > 0f)
		{
		  float lSpeed = 1f;
		  if (_MovementSpeed > 0f) { lSpeed = (rMovement.magnitude / lDeltaTime) / _MovementSpeed; }

		  rInput = Vector3.forward * lSpeed;
		}

		// Tell the animator what to do next
		mAnimator.SetFloat("Speed", rInput.magnitude);
		mAnimator.SetFloat("Direction", Mathf.Atan2(rInput.x, rInput.z) * 180.0f / 3.14159f);
		//mAnimator.SetFloat("Direction", rYawAngle);
	 }

	 protected override void OnArrived()
	 {
		for (int i = 0; i < 20; i++)
		{
		  Vector3 point = transform.position
			 + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * _radiusFindPoint.RandomRange;
		  Vector3 dir = transform.position - point;
		  Vector3 dirNorm = dir.normalized;


		  //correct = RaycastExt.SafeCircularCast(point, dirNorm, transform.up, out _hit, dir.magnitude, 30, _layerRegion);
		  if (RaycastExt.SafeRaycast(point, dirNorm, dir.magnitude, _layerRegion))
		  {
			 continue;
		  }

		  NavMeshPath _path = new NavMeshPath();
		  mNavMeshAgent.CalculatePath(point, _path);
		  if (_path.status != NavMeshPathStatus.PathComplete)
			 continue;


		  if ((_path.corners[_path.corners.Length - 1] - point).magnitude > 0.1f)
			 continue;


		  TargetPosition = point;
		  break;

		};

	 }


  }

}