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
  public class RandomRegionNavmeshMove : AnimatorDriver
  {
	 private NavMeshAgent _navMeshAgent;
	 private NavMeshPath _path;
	 public Vector3 _targetPosition = Vector3.zero;
	 public Vector3 TargetPosition
	 {
		get { return _targetPosition; }

		set
		{
		  _targetPosition = value;

		  if (_targetPosition == Vector3Ext.Null)
		  {
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
                    mNavMeshAgent.Stop();
#else
			 _navMeshAgent.isStopped = true;
#endif
			 _hasArrived = false;
			 _isInSlowDistance = false;

			 _isTargetSet = false;

			 mActorController.SetRelativeVelocity(Vector3.zero);
		  }
		  else
		  {
			 _isTargetSet = true;
		  }
		}
	 }
	 /// <summary>
	 /// Determines if a target is currently set
	 /// </summary>
	 protected bool _isTargetSet = false;
	 public bool IsTargetSet
	 {
		get { return _isTargetSet; }
	 }
	 private int _navPathIndex;
	 [SerializeField]
	 private RangeFloat _radiusFindPoint;
	 [SerializeField]
	 private Collider _regionCollider;

	 private float _distanceToOutRegion = 3;
	 protected Vector3 _waypoint = Vector3.zero;

	 [SerializeField]
	 private LayerMask _layerRegion;

	 protected float _targetDistance = 0f;
	 protected bool _firstPathSet = false;
	 protected bool _firstPathValid = false;

	 /// <summary>
	 /// Determines if the current path is valid
	 /// </summary>
	 protected bool _isPathValid = true;

	 /// <summary>
	 /// Set when we're within the slow distance
	 /// </summary>
	 protected bool _isInSlowDistance = false;

	 public bool IsInSlowDistance
	 {
		get { return IsInSlowDistance; }
	 }
	 protected bool _hasArrived = false;
	 public bool HasArrived
	 {
		get { return _hasArrived; }
	 }


	 public float _pathHeight = 0.05f;
	 public float PathHeight
	 {
		get { return _pathHeight; }
		set { _pathHeight = value; }
	 }

	 public bool _useNavMeshPosition = false;
	 public bool UseNavMeshPosition
	 {
		get { return _useNavMeshPosition; }
		set { _useNavMeshPosition = value; }
	 }
	 /// <summary>
	 /// Determines how far from the destination we'll consider
	 /// us to have arrived
	 /// </summary>
	 public float _stopDistance = 0.1f;
	 public float StopDistance
	 {
		get { return _stopDistance; }
		set { _stopDistance = value; }
	 }

	 /// <summary>
	 /// Distance we'll use to start slowing down so we can arrive nicely.
	 /// </summary>
	 public float _slowDistance = 4.0f;
	 public float SlowDistance
	 {
		get { return _slowDistance; }
		set { _slowDistance = value; }
	 }
	 /// <summary>
	 /// Speed we'll ultimately reduce to before stopping
	 /// </summary>
	 public float _slowFactor = 0.25f;
	 public float SlowFactor
	 {
		get { return _slowFactor; }
		set { _slowFactor = value; }
	 }

	 protected Vector3 _agentDestination = Vector3.zero;

	 protected Vector3 _targetVector = Vector3.zero;
	 protected override void Awake()
	 {
		base.Awake();

		mInputSource = null;
		_navMeshAgent = GetComponent<NavMeshAgent>();
		_path = new NavMeshPath();
	 }

	 private void Start()
	 {
		OnArrived();
		SetDestination(_targetPosition);
	 }


	 protected override void Update()
	 {
		if (!_IsEnabled) { return; }
		if (!mActorController) { return; }
		if (!_navMeshAgent) { return; }
		if (!_isTargetSet) { return; }

		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;
		float lYawAngle = 0;

		// Check if our first path is set and done
		if (_firstPathSet && _navMeshAgent.hasPath && !_navMeshAgent.pathPending)
		{
		  _firstPathValid = true;
		}

		SetDestination(TargetPosition);

		_targetVector = _agentDestination - transform.position;
		_targetDistance = _targetVector.magnitude;

		if (_useNavMeshPosition)
		{
		  if (!_navMeshAgent.pathPending &&
				_navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete &&
				_navMeshAgent.remainingDistance == 0f)
		  {
			 ClearTarget();
			 _hasArrived = true;
			 _firstPathSet = false;
			 _firstPathValid = false;

			 OnArrived();
		  }
		}
		else
		{
		  if (_targetDistance < _stopDistance)
		  {
			 ClearTarget();
			 _hasArrived = true;

			 OnArrived();
		  }


		}

		if (!_hasArrived && _firstPathValid)
		{
		  // Hold on to our next position before we change it
		  if (_navMeshAgent.hasPath && !_navMeshAgent.pathPending)
		  {
			 _isPathValid = true;

			 _waypoint = _navMeshAgent.steeringTarget;
			 if (_targetDistance > _slowDistance) { _isInSlowDistance = false; }
		  }

		  // Determine if we're within the slow distance. We only want to fire the 
		  // event once
		  if (_slowDistance > 0f && _targetDistance < _slowDistance)
		  {
			 if (!_isInSlowDistance) { OnSlowDistanceEntered(); }
			 _isInSlowDistance = true;
		  }

		  // Calculate 
		  CalculateMove(_waypoint, ref lYawAngle, ref lMovement, ref lRotation);

		  // Check if we've reached the destination
		  //if (!mNavMeshAgent.pathPending)
		  {
			 mActorController.Move(lMovement);
			 mActorController.Rotate(lRotation);
		  }

		  // Force the agent to stay with our actor. This way, the path is
		  // alway relative to our current position. Then, we can use the AC
		  // to move to a valid position.
		  if (!_useNavMeshPosition)
		  {
			 _navMeshAgent.nextPosition = transform.position;
		  }
		}

		SetAnimatorPropertiesNew(Vector3.zero, lMovement, lRotation, lYawAngle);
	 }
	 private void ClearTarget()
	 {
		//if (_ClearTargetOnStop)
		//{
		//  _Target = null;
		//  _TargetPosition = Vector3Ext.Null;
		//  mIsTargetSet = false;
		//}

#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
            mNavMeshAgent.Stop();
#else
		_navMeshAgent.isStopped = true;
#endif

		_hasArrived = false;
		_isPathValid = true;
		_firstPathSet = false;
		_firstPathValid = false;
		_isInSlowDistance = false;
		_agentDestination = mActorController.transform.position;

		mActorController.SetRelativeVelocity(Vector3.zero);
	 }

	 private void CheckOutRegion()
	 {

	 }
	 protected virtual void OnSlowDistanceEntered()
	 {
	 }
	 private void OnArrived()
	 {
		for(int i = 0; i < 20; i++)
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
		  _navMeshAgent.CalculatePath(point, _path);
		  if(_path.status != NavMeshPathStatus.PathComplete)
			 continue;


		  if ((_path.corners[_path.corners.Length-1] - point).magnitude > 0.1f)
			 continue;


		  TargetPosition = point;
		  break;

		};

	 }

	 protected void SetDestination(Vector3 rDestination)
	 {
		if (!_hasArrived && _agentDestination == rDestination) { return; }

		// Reset the properties
		_hasArrived = false;

		// Set the new destination
		_agentDestination = rDestination;

		// Recalculate the path
		if (_isPathValid && !_navMeshAgent.pathPending)
		{
		  _isPathValid = false;

		  _navMeshAgent.updatePosition = false;
		  _navMeshAgent.updateRotation = false;
		  _navMeshAgent.stoppingDistance = _stopDistance;

		  _navMeshAgent.ResetPath();
		  _navMeshAgent.SetDestination(_agentDestination);

		  _firstPathSet = true;
		}
	 }

	 private void OnDrawGizmos()
	 {
		Gizmos.DrawLine(transform.position, _agentDestination);

#if UNITY_EDITOR

		UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, _radiusFindPoint.Min);
		UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, _radiusFindPoint.Max);
#endif

		if (_navMeshAgent != null && _navMeshAgent.hasPath && _navMeshAgent.path.corners.Length > 0)
		{
		  for (int i = 0; i < _path.corners.Length; i++)
		  {
			 if (i < _path.corners.Length - 1)
				Gizmos.DrawLine(_path.corners[i], _path.corners[i + 1]);
		  }
		}
	 }
	 protected void SetAnimatorPropertiesNew(Vector3 rInput, Vector3 rMovement, Quaternion rRotation, float rYawAngle)
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

	 private void OnDrawGizmosSelected()
	 {
	 }


	 protected virtual void CalculateMove(Vector3 rWaypoint, ref float lYawAngle, ref Vector3 rMove, ref Quaternion rRotate)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		Vector3 lDirection = rWaypoint - transform.position;
		lDirection.y = lDirection.y - _pathHeight;
		lDirection.Normalize();

		Vector3 lVerticalDirection = Vector3.Project(lDirection, transform.up);
		Vector3 lLateralDirection = lDirection - lVerticalDirection;

		lYawAngle = Vector3Ext.SignedAngle(transform.forward, lLateralDirection);

		if (_RotationSpeed == 0f)
		{
		  rRotate = Quaternion.AngleAxis(lYawAngle, transform.up);
		}
		else
		{
		  rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), _RotationSpeed * lDeltaTime), transform.up);
		}

		if (_useNavMeshPosition)
		{
		  rMove = _navMeshAgent.nextPosition - transform.position;
		}
		// In this case, we'll calculate movement ourselves
		else
		{
		  // Grab the base movement speed
		  float lMoveSpeed = mRootMotionMovement.magnitude / lDeltaTime;
		  if (lMoveSpeed == 0f) { lMoveSpeed = _MovementSpeed; }

		  // Calculate our own slowing
		  float lRelativeMoveSpeed = 1f;
		  if (_isInSlowDistance && _slowFactor > 0f)
		  {
			 float lSlowPercent = (_targetDistance - _stopDistance) / (_slowDistance - _stopDistance);
			 lRelativeMoveSpeed = ((1f - _slowFactor) * lSlowPercent) + _slowFactor;
		  }

		  // TRT 4/5/2016: Force the slow distance as an absolute value
		  if (_isInSlowDistance && _slowFactor > 0f)
		  {
			 lMoveSpeed = _slowFactor;
			 lRelativeMoveSpeed = 1f;
		  }

		  // Set the final velocity based on the future rotation
		  Quaternion lFutureRotation = transform.rotation * rRotate;
		  rMove = lFutureRotation.Forward() * (lMoveSpeed * lRelativeMoveSpeed * lDeltaTime);
		}

	 }
  }
}