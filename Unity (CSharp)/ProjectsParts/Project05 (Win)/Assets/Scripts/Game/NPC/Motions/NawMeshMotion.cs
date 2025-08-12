using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using com.ootii.Actors;
using com.ootii.Geometry;
using com.ootii.Timing;

namespace it.Game.NPC.Motions
{
  public class NawMeshMotion : AnimatorDriver
  {
	 public UnityEngine.Events.UnityAction OnStartMove;
	 public UnityEngine.Events.UnityAction OnTargetArrived;

	 protected NavMeshAgent _navMeshAgent;

	 protected Vector3 _targetPosition = Vector3.zero;
	 [SerializeField]
	 protected Transform _targetTransform = null;

	 protected Vector3 Target
	 {
		get
		{
		  if (_targetTransform != null)
			 return _targetTransform.position;

		  return _targetPosition;
		}
		set
		{
		  _targetPosition = value;
		  _isTargetSet = true;
		  _hasArrived = false;
		}
	 }
	 protected Vector3 _targetVector = Vector3.zero;
	 /// <summary>
	 /// Установлен маршрут
	 /// </summary>
	 protected bool _isTargetSet;
	 protected bool IsTargetSet { get => _isTargetSet; set => _isTargetSet = value; }

	 protected Vector3 _agentDestination = Vector3.zero;
	 protected float _targetDistance = 0f;
	 /// <summary>
	 /// Прибыли на место
	 /// </summary>
	 protected bool _hasArrived = false;
	 public bool HasArrived
	 {
		get { return _hasArrived; }
	 }
	 public bool _useNavMeshPosition = false;
	 public bool UseNavMeshPosition
	 {
		get { return _useNavMeshPosition; }
		set { _useNavMeshPosition = value; }
	 }
	 public float _pathHeight = 0.05f;
	 public float PathHeight
	 {
		get { return _pathHeight; }
		set { _pathHeight = value; }
	 }
	 protected Vector3 _waypoint = Vector3.zero;
	 protected bool _firstPathSet = false;
	 protected bool _firstPathValid = false;

	 /// <summary>
	 /// Маршрут доступен
	 /// </summary>
	 protected bool _isPathValid = true;

	 /// <summary>
	 /// Расстояние до конечной точки, при которой считается путь пройден
	 /// </summary>
	 [SerializeField]
	 private float _stopDistance = 0.1f;
	 protected float StopDistance { get => _stopDistance; set => _stopDistance = value; }

	 protected override void Awake()
	 {
		base.Awake();

		_navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

		if (_navMeshAgent != null)
		{
		  _navMeshAgent.updatePosition = false;
		  _navMeshAgent.updateRotation = false;
		  if (_MovementSpeed > 0f) { _navMeshAgent.speed = _MovementSpeed; }
		  if (_RotationSpeed > 0f) { _navMeshAgent.angularSpeed = _RotationSpeed; }
		}
	 }

	 public void ClearTarget()
	 {
		_navMeshAgent.isStopped = true;

		_hasArrived = false;
		//_isPathValid = true;
		//mFirstPathSet = false;
		//mFirstPathValid = false;
		//mIsInSlowDistance = false;
		_agentDestination = mActorController.transform.position;

		mActorController.SetRelativeVelocity(Vector3.zero);
	 }

	 protected override void Update()
	 {
		if (!_IsEnabled) { return; }
		if (mActorController == null) { return; }
		if (_navMeshAgent == null) { return; }
		if (!_isTargetSet) { return; }

		// Эмоляция ввода
		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;

		SetDestination(_targetPosition);

		_targetVector = _agentDestination - transform.position;
		_targetDistance = _targetVector.magnitude;

		if (_targetDistance < _stopDistance)
		{
		  ClearTarget();
		  _hasArrived = true;

		  OnTargetArrived?.Invoke();
		}

		if (!_hasArrived)
		{
		  if (_navMeshAgent.hasPath && !_navMeshAgent.pathPending)
		  {
			 _isPathValid = true;

			 _waypoint = _navMeshAgent.steeringTarget;
			 //if (_targetDistance > _slowDistance) { _isInSlowDistance = false; }
		  }

		  CalculateMove(_waypoint, ref lMovement, ref lRotation);

		  mActorController.Move(lMovement);
		  mActorController.Rotate(lRotation);

		  if (!_useNavMeshPosition)
		  {
			 _navMeshAgent.nextPosition = transform.position;
		  }

		}

	 }

	 protected virtual void CalculateMove(Vector3 rWaypoint, ref Vector3 rMove, ref Quaternion rRotate)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		Vector3 lDirection = rWaypoint - transform.position;
		lDirection.y = lDirection.y - _pathHeight;
		lDirection.Normalize();

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

		// Determine the movement
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
		  //if (mIsInSlowDistance && _SlowFactor > 0f)
		  //{
			 //float lSlowPercent = (mTargetDistance - _StopDistance) / (_SlowDistance - _StopDistance);
			 //lRelativeMoveSpeed = ((1f - _SlowFactor) * lSlowPercent) + _SlowFactor;
		  //}

		  // TRT 4/5/2016: Force the slow distance as an absolute value
		  //if (mIsInSlowDistance && _SlowFactor > 0f)
		  //{
			 //lMoveSpeed = _SlowFactor;
			 //lRelativeMoveSpeed = 1f;
		  //}

		  // Set the final velocity based on the future rotation
		  Quaternion lFutureRotation = transform.rotation * rRotate;
		  rMove = lFutureRotation.Forward() * (lMoveSpeed * lRelativeMoveSpeed * lDeltaTime);
		}
	 }

	 protected virtual void SetDestination(Vector3 rDestination)
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

  }
}