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
  public class FlyingWayPoints : ActorDriver
  {
	 public Transform[] _wayPoints;

	 private int _indexPoint;

	 [SerializeField]
	 private float _verticalOffset = 0;

	 private Transform ActualPoint
	 {
		get
		{
		  return _wayPoints[_indexPoint];
		}
	 }

	 private float _movementSpeed = 5;

	 protected override void Awake()
	 {
		base.Awake();
		CheckNullwayPoints();

		mInputSource = null;
		_indexPoint = 0;
		mActorController.IsGravityEnabled = false;
		mActorController.ForceGrounding = false;
		mActorController.FixGroundPenetration = false;
	 }

	 private void CheckNullwayPoints()
	 {

		for (int i = 0; i < _wayPoints.Length; i++)
		{
		  if(_wayPoints[i] == null)
		  {
			 _wayPoints = new Transform[0];
			 return;
		  }
		}
	 }

	 private void OnDrawGizmosSelected()
	 {
		if(_wayPoints.Length > 1)
		{
		  for(int i = 0; i < _wayPoints.Length; i++)
		  {
			 if(i == _wayPoints.Length -1)
				Gizmos.DrawLine(_wayPoints[i].position, _wayPoints[0].position);
			 else
			 Gizmos.DrawLine(_wayPoints[i].position, _wayPoints[i + 1].position);
		  }
		}
	 }

	 protected override void Update()
	 {
		if (!_IsEnabled) { return; }
		if (_wayPoints.Length <2 ) { return; }
		base.Update();

		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;

		CalculateMove(ActualPoint.position + transform.up * _verticalOffset, ref lMovement, ref lRotation);

		mActorController.Move(lMovement);
		mActorController.Rotate(lRotation);

		if(((ActualPoint.position + transform.up * _verticalOffset) - transform.position).magnitude <= 0.1f)
		{
		  IncrementIndex();
		}

	 }

	 private void IncrementIndex()
	 {
		_indexPoint++;
		if (_indexPoint >= _wayPoints.Length)
		  _indexPoint = 0;
	 }

	 protected virtual void CalculateMove(Vector3 rWaypoint, ref Vector3 rMove, ref Quaternion rRotate)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		// Direction we need to travel in
		Vector3 lDirection = rWaypoint - transform.position;
		lDirection.Normalize();

		// Determine our rotation
		Vector3 lVerticalDirection = Vector3.Project(lDirection, transform.up);
		Vector3 lLateralDirection = lDirection - lVerticalDirection;

		float lYawAngle = Vector3Ext.SignedAngle(transform.forward, lLateralDirection);

		if (_RotationSpeed == 0f)
		{
		  rRotate = Quaternion.AngleAxis(lYawAngle, transform.up);
		}
		else
		{
		  rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), _RotationSpeed * lDeltaTime), transform.up);
		}


		// Set the final velocity based on the future rotation
		Quaternion lFutureRotation = transform.rotation * rRotate;
		rMove = (lFutureRotation.Forward() + lVerticalDirection) * (_movementSpeed * MovementSpeed * lDeltaTime);

	 }



  }
}