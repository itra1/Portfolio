using UnityEngine;
using com.ootii.Helpers;
using com.ootii.Geometry;
using com.ootii.Timing;
using com.ootii.Actors;


namespace it.Game.NPC.Motions
{
  public class AnimationWayPointsNawMeshMove : NavMeshDriver
  {
	 [SerializeField]
	 public Transform[] _wayPoints;

	 private int _targetWaypointsIndex = 0;


	 private float _actualSpeed = 0;

	 private float _angleMinLock = 1f;
	 /// <summary>
	 /// Determines if the actor wanders from the current position or his start postion
	 /// </summary>
	 public bool WanderFromCurrentPosition = false;

	 /// <summary>
	 /// Radius in which the actor will randomly wander
	 /// </summary>
	 public Vector3 WanderRadius = Vector3.zero;

	 /// <summary>
	 /// Store the start position
	 /// </summary>
	 private Vector3 mStartPosition = Vector3.zero;

	 /// <summary>
	 /// Once the objects are instanciated, awake is called before start. Use it
	 /// to setup references to other objects
	 /// </summary>
	 protected override void Awake()
	 {
		base.Awake();

		mInputSource = null;

		mStartPosition = transform.position;
	 }

	 protected override void Start()
	 {

		GetNextWayPoint(true);
		base.Start();
	 }

	 /// <summary>
	 /// Provides a place to set the properties of the animator
	 /// </summary>
	 /// <param name="rInput">Vector3 representing the input</param>
	 /// <param name="rMove">Vector3 representing the amount of movement taking place (in world space)</param>
	 /// <param name="rRotate">Quaternion representing the amount of rotation taking place</param>
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
		//mAnimator.SetFloat("Speed", rInput.magnitude);
		//mAnimator.SetFloat("Direction", Mathf.Atan2(rInput.x, rInput.z) * 180.0f / 3.14159f);
		//mAnimator.SetBool("Jump", false);
	 }

	 /// <summary>
	 /// Event function for when we arrive at the destination. In this instance,
	 /// we're going to find a random position and move there.
	 /// </summary>
	 protected override void OnArrived()
	 {
		GetNextWayPoint();
	 }

	 protected override void Update()
	 {
		if (!_IsEnabled) { return; }
		if (mActorController == null) { return; }
		if (mNavMeshAgent == null) { return; }
		if (!mIsTargetSet) { return; }

		// Simulated input for the animator
		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;
		float yawAngle = 0;

		// Check if our first path is set and done
		if (mFirstPathSet && mNavMeshAgent.hasPath && !mNavMeshAgent.pathPending)
		{
		  mFirstPathValid = true;
		}

		// Set the destination
		if (_Target != null) { _TargetPosition = _Target.position; }
		SetDestination(_TargetPosition);

		// Determine if we're at the destination
		mTargetVector = mAgentDestination - transform.position;
		mTargetDistance = mTargetVector.magnitude;

		if (mTargetDistance < _StopDistance)
		{
		  ClearTarget();
		  mHasArrived = true;

		  OnArrived();
		}

		// Determine the next move
		if (!mHasArrived && mFirstPathValid)
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
		  CalculateMove(mWaypoint, ref lMovement, ref lRotation, ref yawAngle);

		  // Check if we've reached the destination
		  //if (!mNavMeshAgent.pathPending)
		  {
			 mActorController.Move(lMovement);
			 mActorController.Rotate(lRotation);
		  }

		  // Force the agent to stay with our actor. This way, the path is
		  // alway relative to our current position. Then, we can use the AC
		  // to move to a valid position.
		  if (!_UseNavMeshPosition)
		  {
			 mNavMeshAgent.nextPosition = transform.position;
		  }
		}
		SetAnimator(Vector3.zero, lMovement, yawAngle);

		// Tell the animator what to do next
		//SetAnimatorProperties(Vector3.zero, lMovement, lRotation);
	 }

	 protected void SetAnimator(Vector3 rInput, Vector3 rMovement, float rRotation)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		// Determine the simulated input
		if (rMovement.sqrMagnitude > 0f)
		{
		  float lSpeed = 1f;
		  if (_MovementSpeed > 0f) { lSpeed = (rMovement.magnitude / lDeltaTime) / _MovementSpeed; }

		  rInput = Vector3.forward * lSpeed;
		}

		mAnimator.SetFloat("Speed", _actualSpeed);
		mAnimator.SetFloat("Direction", rRotation);


		//mAnimator.SetBool("Jump", false);
	 }

	 protected void CalculateMove(Vector3 rWaypoint, ref Vector3 rMove, ref Quaternion rRotate, ref float lYawAngle)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		// Direction we need to travel in
		Vector3 lDirection = rWaypoint - transform.position;
		lDirection.y = lDirection.y - _PathHeight;
		lDirection.Normalize();

		// Determine our rotation
		Vector3 lVerticalDirection = Vector3.Project(lDirection, transform.up);
		Vector3 lLateralDirection = lDirection - lVerticalDirection;

		lYawAngle = Vector3Ext.SignedAngle(transform.forward, lLateralDirection);

		if (Mathf.Abs(lYawAngle) < 20)
		{
		  _actualSpeed += Time.deltaTime;
		}
		else
		{
		  _actualSpeed -= Time.deltaTime;
		}

		_actualSpeed = Mathf.Clamp(_actualSpeed, 0, 0.5f);


		if (Mathf.Abs(lYawAngle) < _angleMinLock)
		  lYawAngle = 0;
		else if (Mathf.Abs(lYawAngle) < 10)
		  lYawAngle /= 10;
		else
		  lYawAngle = Mathf.Sign(lYawAngle);

		//if (_RotationSpeed == 0f)
		//{
		//  rRotate = Quaternion.AngleAxis(lYawAngle, transform.up);
		//}
		//else
		//{
		//  rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), _RotationSpeed * lDeltaTime), transform.up);
		//}

		//  // Grab the base movement speed
		//  float lMoveSpeed = mRootMotionMovement.magnitude / lDeltaTime;
		//  if (lMoveSpeed == 0f) { lMoveSpeed = _MovementSpeed; }

		//  // Calculate our own slowing
		//  float lRelativeMoveSpeed = 1f;

		//  // Set the final velocity based on the future rotation
		//  Quaternion lFutureRotation = transform.rotation * rRotate;
		//rMove = lFutureRotation.Forward()* _actualSpeed;
		//rMove = lFutureRotation.Forward() * (lMoveSpeed * lRelativeMoveSpeed * lDeltaTime);

	 }

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
		  else if (lHits == 1)
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

	 private void OnDrawGizmos()
	 {
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, TargetPosition);

		Gizmos.color = Color.blue;
		if (Application.isPlaying)
		{
		  if (mNavMeshAgent.path.corners.Length > 0)
		  {
			 var path = mNavMeshAgent.path;
			 for (int i = 0; i < path.corners.Length; i++)
			 {
				if (i == 0)
				  Gizmos.DrawLine(transform.position, path.corners[i]);
				else
				  Gizmos.DrawLine(path.corners[i - 1], path.corners[i]);

			 }
		  }
		}


		Gizmos.color = Color.red;
		for (int i = 0; i < _wayPoints.Length; i++)
		{
		  if (i == 0)
			 Gizmos.DrawLine(_wayPoints[_wayPoints.Length - 1].position, _wayPoints[i].position);
		  else
			 Gizmos.DrawLine(_wayPoints[i - 1].position, _wayPoints[i].position);
		}

	 }

	 /// <summary>
	 /// Grabs a random
	 /// </summary>
	 /// <returns></returns>
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
			 _targetWaypointsIndex = 0;
		  targetPoint = _wayPoints[_targetWaypointsIndex].position;
		}

		return targetPoint;

	 }

	 protected override void OnAnimatorMove()
	 {
		//Debug.Log(mAnimator.deltaPosition);
		//transform.position += new Vector3(mAnimator.deltaPosition.x,0, mAnimator.deltaPosition.z);
		//transform.rotation *= mAnimator.deltaRotation;
		//transform.rotation *= Quaternion.AngleAxis(10,Vector3.up);
		if (Time.deltaTime == 0f) { return; }
		transform.position += mAnimator.deltaPosition;
		//transform.rotation = mAnimator.bodyRotation;m
		mActorController.Rotate(mAnimator.deltaRotation);

		//if (Time.deltaTime == 0f) { return; }

		//// Clear any root motion values
		//if (mAnimator == null)
		//{
		//  mRootMotionMovement = Vector3.zero;
		//  mRootMotionRotation = Quaternion.identity;
		//}
		//// Store the root motion as a velocity per second. We also
		//// want to keep it relative to the avatar's forward vector (for now).
		//// Use Time.deltaTime to create an accurate velocity (as opposed to Time.fixedDeltaTime).
		//else
		//{
		//  // Convert the movement to relative the current rotation
		//  mRootMotionMovement = Quaternion.Inverse(transform.rotation) * (mAnimator.deltaPosition);

		//  // Store the rotation as a velocity per second.
		//  mRootMotionRotation = mAnimator.deltaRotation;
		//}
	 }

  }
}