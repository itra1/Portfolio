using UnityEngine;
using com.ootii.Helpers;
using com.ootii.Geometry;
using com.ootii.Timing;
using com.ootii.Actors;
using com.ootii.Actors.AnimationControllers;

namespace it.Game.Player.Actors
{
  public class PlayerNavMovePath : NavMeshDriver
  {

	 public UnityEngine.Events.UnityAction OnComplete;

	 [SerializeField]
	 public Transform[] _wayPoints;

	 private int _targetWaypointsIndex = 0;

	 MotionController _motionsController;

	 private float _actualSpeed = 0;

	 private float _angleMinLock = 1f;
	 protected override void Start()
	 {
		_motionsController = GetComponent<MotionController>();
		base.Start();
	 }

	 public void SetPath(Transform[] path)
	 {
		_wayPoints = path;
		_targetWaypointsIndex = -1;
		GetNextWayPoint(true);
	 }

	 protected override void Update()
	 {

		if (!_IsEnabled) { return; }
		if (mActorController == null) { return; }
		if (mNavMeshAgent == null) { return; }
		if (!mIsTargetSet) { return; }

		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;

		// Check if our first path is set and done
		if (mFirstPathSet && mNavMeshAgent.hasPath && !mNavMeshAgent.pathPending)
		{
		  mFirstPathValid = true;
		}

		// Set the destination
		if (_Target != null) { _TargetPosition = _Target.position; }
		SetDestination(_TargetPosition);

		mTargetVector = mAgentDestination - transform.position;
		mTargetDistance = mTargetVector.magnitude;

		if (mTargetDistance < _StopDistance)
		{
		  ClearTarget();
		  mHasArrived = true;

		  OnArrived();
		}

		// Determine the next move
		if (!mHasArrived /*&& mFirstPathValid*/)
		{
		  // Hold on to our next position before we change it
		  if (mNavMeshAgent.hasPath && !mNavMeshAgent.pathPending)
		  {
			 mIsPathValid = true;

			 mWaypoint = mNavMeshAgent.steeringTarget;
			 if (mTargetDistance > _SlowDistance) { mIsInSlowDistance = false; }
		  }

		  // Determine if we're within the slow distance. We only want to fire the 
		  // event once
		  if (_SlowDistance > 0f && mTargetDistance < _SlowDistance)
		  {
			 if (!mIsInSlowDistance) { OnSlowDistanceEntered(); }
			 mIsInSlowDistance = true;
		  }

		  // Calculate 
		  CalculateMove(mWaypoint, ref lMovement, ref lRotation);

		  // Check if we've reached the destination
		  //if (!mNavMeshAgent.pathPending)
		  {
			 //mActorController.Move(lMovement);
			 mActorController.Rotate(lRotation);
		  }

		  if (!_UseNavMeshPosition)
		  {
			 mNavMeshAgent.nextPosition = transform.position;
		  }
		}
		SetAnimator(Vector3.zero, lMovement);

	 }

	 protected void SetAnimator(Vector3 rInput, Vector3 rMovement)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		// Determine the simulated input
		if (rMovement.sqrMagnitude > 0f)
		{
		  float lSpeed = 1f;
		  if (_MovementSpeed > 0f) { lSpeed = (rMovement.magnitude / lDeltaTime) / _MovementSpeed; }

		  rInput = Vector3.forward * lSpeed;
		}

		_motionsController.Animator.SetFloat("InputMagnitude", rMovement.normalized.magnitude);
		//_motionsController.Animator.SetFloat("Direction", rRotation);

	 }

	 //protected void CalculateMove(Vector3 rWaypoint, ref Vector3 rMove, ref Quaternion rRotate, ref float lYawAngle)
	 //{
		//float lDeltaTime = TimeManager.SmoothedDeltaTime;

		//// Direction we need to travel in
		//Vector3 lDirection = rWaypoint - transform.position;
		//lDirection.y = lDirection.y - _PathHeight;
		//lDirection.Normalize();

		//// Determine our rotation
		//Vector3 lVerticalDirection = Vector3.Project(lDirection, transform.up);
		//Vector3 lLateralDirection = lDirection - lVerticalDirection;

		//lYawAngle = Vector3Ext.SignedAngle(transform.forward, lLateralDirection);

		////if (Mathf.Abs(lYawAngle) < 20)
		////{
		////  _actualSpeed += Time.deltaTime;
		////}
		////else
		////{
		////  _actualSpeed -= Time.deltaTime;
		////}

		//_actualSpeed = Mathf.Clamp(_actualSpeed, 0, 0.5f);


		////if (Mathf.Abs(lYawAngle) < _angleMinLock)
		////  lYawAngle = 0;
		////else if (Mathf.Abs(lYawAngle) < 10)
		////  lYawAngle /= 10;
		////else
		////  lYawAngle = Mathf.Sign(lYawAngle);

		//if (_RotationSpeed == 0f)
		//{
		//  rRotate = Quaternion.AngleAxis(lYawAngle, transform.up);
		//}
		//else
		//{
		//  rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), _RotationSpeed * lDeltaTime), transform.up);
		//}

		//// Grab the base movement speed
		//float lMoveSpeed = mRootMotionMovement.magnitude / lDeltaTime;
		//if (lMoveSpeed == 0f) { lMoveSpeed = _MovementSpeed; }

		//// Calculate our own slowing
		//float lRelativeMoveSpeed = 1f;

		//// Set the final velocity based on the future rotation
		//Quaternion lFutureRotation = transform.rotation * rRotate;
		//rMove = lFutureRotation.Forward() * _actualSpeed;
		//rMove = lFutureRotation.Forward() * (lMoveSpeed * lRelativeMoveSpeed * lDeltaTime);

	 //}

	 private void GetNextWayPoint(bool isFirst = false)
	 {

		bool lIsPositionValid = false;
		Vector3 lNewPosition = Vector3.zero;

		for (int i = 0; i < 20; i++)
		{
		  lNewPosition = GetNextPosition(isFirst);

		  Collider[] lColliders = null;
		  int lHits = RaycastExt.SafeOverlapSphere(lNewPosition, mActorController.OverlapRadius * 2f, out lColliders, Game.ProjectSettings.GroundLayerMaks, transform);

		  // Ensure the position isn't someplace we can't get to
		  if (lColliders == null || lHits == 0)
		  {
			 lIsPositionValid = true;
		  }
		  else
		  {
			 lIsPositionValid = true;
		  }

		  if (lIsPositionValid) { break; }
		}

		// If we have a valid position, set it
		if (lIsPositionValid)
		{
		  TargetPosition = lNewPosition;
		}
	 }

	 protected override void OnAnimatorMove()
	 {
		if (Time.deltaTime == 0f) { return; }
		transform.position += _motionsController.Animator.deltaPosition;

		//mActorController.Rotate(_motionsController.Animator.deltaRotation);

	 }

	 private Vector3 GetNextPosition(bool isFirst)
	 {
		Vector3 targetPoint = Vector3.zero;
		if (isFirst)
		{
		  float? maxDist = null;

		  for (int i = 0; i < _wayPoints.Length; i++)
		  {
			 if (maxDist == null || (_wayPoints[i].position - transform.position).sqrMagnitude < maxDist)
			 {
				maxDist = (_wayPoints[i].position - transform.position).sqrMagnitude;
				targetPoint = _wayPoints[i].position;
				_targetWaypointsIndex = i;
			 }
		  }
		}
		else
		{
		  _targetWaypointsIndex++;
		  if (_targetWaypointsIndex > _wayPoints.Length - 1)
		  {
			 _targetWaypointsIndex = 0;
			 OnComplete?.Invoke();
		  }
		  targetPoint = _wayPoints[_targetWaypointsIndex].position;
		}

		return targetPoint;

	 }

	 protected override void OnArrived()
	 {
		GetNextWayPoint();
	 }


  }
}