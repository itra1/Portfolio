using UnityEngine;
using com.ootii.Helpers;
using com.ootii.Geometry;
using com.ootii.Timing;
using com.ootii.Actors;
using com.ootii.Actors.AnimationControllers;

namespace it.Game.Player.Actors
{
  public class PlayerActorMovePath : AnimatorDriver
  {

	 public UnityEngine.Events.UnityAction OnComplete;

	 [SerializeField]
	 public Transform[] _wayPoints;
	 private Vector3 _transformPosition;

	 private int _targetWaypointsIndex = 0;

	 MotionController _motionsController;

	 private float _actualSpeed = 0;

	 private bool isMove = false;
	 private float _StopDistance = 0.2f;

	 private Vector3 mTargetVector;
	 private float mTargetDistance;

	 private float _angleMinLock = 1f;
	 protected void Start()
	 {
		_motionsController = GetComponent<MotionController>();
	 }

	 public void SetPath(Transform[] path)
	 {
		_wayPoints = path;
		isMove = true;
		_targetWaypointsIndex = -1;
		GetNextWayPoint(true);
	 }

	 protected override void Update()
	 {

		if (!_IsEnabled) { return; }
		if (mActorController == null) { return; }
		if (!isMove) { return; }

		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;



		mTargetVector = _transformPosition - transform.position;
		mTargetDistance = mTargetVector.magnitude;

		if (mTargetDistance < _StopDistance)
		{

		  OnArrived();
		}

		// Calculate 
		CalculateMove(_transformPosition, ref lMovement, ref lRotation);

		// Check if we've reached the destination
		//if (!mNavMeshAgent.pathPending)
		{
		  //mActorController.Move(lMovement);
		  mActorController.Rotate(lRotation);
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

		_motionsController.Animator.SetFloat("InputMagnitude", rMovement.normalized.magnitude/2);
		_motionsController.Animator.SetInteger("L0MotionPhase", 3050);
		//_motionsController.Animator.SetFloat("Direction", rRotation);

	 }
	 protected virtual void CalculateMove(Vector3 rWaypoint, ref Vector3 rMove, ref Quaternion rRotate)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		// Direction we need to travel in
		Vector3 lDirection = rWaypoint - transform.position;
		lDirection.y = lDirection.y - 0.05f;
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

		// Grab the base movement speed
		float lMoveSpeed = mRootMotionMovement.magnitude / lDeltaTime;
		if (lMoveSpeed == 0f) { lMoveSpeed = _MovementSpeed; }

		// Calculate our own slowing
		float lRelativeMoveSpeed = 1f;
		

		// Set the final velocity based on the future rotation
		Quaternion lFutureRotation = transform.rotation * rRotate;
		rMove = lFutureRotation.Forward() * (lMoveSpeed * lRelativeMoveSpeed * lDeltaTime);

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
		  _transformPosition = lNewPosition;
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

	 protected void OnArrived()
	 {
		GetNextWayPoint();
	 }


  }
}