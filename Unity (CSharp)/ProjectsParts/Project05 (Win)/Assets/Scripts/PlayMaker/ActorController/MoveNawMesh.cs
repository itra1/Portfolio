using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine.AI;
using com.ootii.Actors;
using com.ootii.Geometry;
using com.ootii.Timing;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  public abstract class MoveNawMesh : ActorDriver
  {
	 //[Tooltip("Игнорирование разницу в высоте")]

	 public FsmBool _checkDistanceError = true;
	 public FsmFloat _maxDistanceError = 0.5f;
	 public FsmEvent _onCollision;
	 public FsmBool _forceRotateInShort;
	 public FsmBool _ignoreDeltaHeight;
	 public FsmBool _stopIfCollision;
	 public bool _UseNavMeshPosition = false;
	 protected NavMeshAgent _navMeshAgent;
	 protected bool _stopEverySetPosition = true;

	 private float _lastDiatanceError;



	 public bool UseNavMeshPosition
	 {
		get { return _UseNavMeshPosition; }
		set { _UseNavMeshPosition = value; }
	 }


	 protected Transform _Target = null;
	 protected Transform Target
	 {
		get { return _Target; }

		set
		{
		  _Target = value;
		  if (_Target == null)
		  {
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
                    _navMeshAgent.Stop();
#else
			 _navMeshAgent.isStopped = true;
#endif
			 ClearTarget();
			 _hasArrived = false;
			 mIsInSlowDistance = false;
			 _frameCount = 0;

			 mIsTargetSet = false;
			 _TargetPosition = Vector3Ext.Null;

			 if(_stopEverySetPosition)
				_actor.SetRelativeVelocity(Vector3.zero);
		  }
		  else
		  {
			 mIsTargetSet = true;
			 _frameCount = 0;
			 _TargetPosition = _Target.position;
		  }
		}
	 }
	 public Vector3 _TargetPosition = Vector3.zero;
	 public Vector3 TargetPosition
	 {
		get { return _TargetPosition; }

		set
		{
		  _Target = null;
		  _TargetPosition = value;

		  if (_TargetPosition == Vector3Ext.Null)
		  {
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
                    _navMeshAgent.Stop();
#else
			 _navMeshAgent.isStopped = true;
#endif
			 ClearTarget();
			 _hasArrived = false;
			 mIsInSlowDistance = false;
			 _frameCount = 0;

			 mIsTargetSet = false;
			 _actor.SetRelativeVelocity(Vector3.zero);
		  }
		  else
		  {
			 _frameCount = 0;
			 mIsTargetSet = true;
		  }
		}
	 }
	 public FsmFloat _StopDistance = new FsmFloat(0.1f) ;
	 public float StopDistance
	 {
		get { return _StopDistance.Value; }
		set { _StopDistance = value; }
	 }
	 public FsmFloat _SlowDistance = new FsmFloat(0f) ;
	 public float SlowDistance
	 {
		get { return _SlowDistance.Value; }
		set { _SlowDistance = value; }
	 }
	 /// <summary>
	 /// Speed we'll ultimately reduce to before stopping
	 /// </summary>
	 public FsmFloat _SlowFactor = new FsmFloat(0.25f);
	 public float SlowFactor
	 {
		get { return _SlowFactor.Value; }
		set { _SlowFactor.Value = value; }
	 }
	 public FsmFloat _PathHeight = new FsmFloat(0.05f);
	 public float PathHeight
	 {
		get { return _PathHeight.Value; }
		set { _PathHeight.Value = value; }
	 }
	 protected bool mIsTargetSet = false;
	 public bool IsTargetSet
	 {
		get { return mIsTargetSet; }
	 }
	 protected bool _hasArrived = false;
	 public bool HasArrived
	 {
		get { return _hasArrived; }
	 }
	 protected bool mIsInSlowDistance = false;
	 public bool IsInSlowDistance
	 {
		get { return IsInSlowDistance; }
	 }
	 protected Vector3 mWaypoint = Vector3.zero;
	 protected Vector3 mAgentDestination = Vector3.zero;
	 protected Vector3 mTargetVector = Vector3.zero;
	 protected float mTargetDistance = 0f;
	 protected bool mFirstPathSet = false;
	 protected bool mFirstPathValid = false;
	 protected bool mIsPathValid = true;
	 private int _frameCount = 0;

	 public override void OnEnter()
	 {
		base.OnEnter();
		_navMeshAgent = _go.GetComponent<NavMeshAgent>();

		if (_navMeshAgent != null)
		{
		  _navMeshAgent.updatePosition = false;
		  _navMeshAgent.updateRotation = false;
		  if (movementSpeed.Value > 0f) { _navMeshAgent.speed = movementSpeed.Value; }
		  if (rotationSpeed.Value > 0f) { _navMeshAgent.angularSpeed = rotationSpeed.Value; }
		}

		mTargetDistance = 999999999;
	 }

	 public override void OnDrawActionGizmos()
	 {
		if (!_hasArrived)
		  return;
		if(_navMeshAgent != null && _navMeshAgent.path != null && _navMeshAgent.path.corners.Length > 0)
		{
		  Gizmos.color = Color.blue;
		  for(int i = 0; i < _navMeshAgent.path.corners.Length-1; i++)
		  {

			 Gizmos.DrawLine(_navMeshAgent.path.corners[i], _navMeshAgent.path.corners[i+1]);
		  }
		}
		  Gizmos.color = Color.red;
		Gizmos.DrawLine(_go.transform.position, _TargetPosition);

	 }

	 public override void OnUpdate()
	 {
		base.OnUpdate();

		if (_actor == null) { return; }
		if (_navMeshAgent == null) { return; }

		// Simulated input for the animator
		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;

		// Check if our first path is set and done
		if (mFirstPathSet && _navMeshAgent.path.corners.Length > 0 && !_navMeshAgent.pathPending)
		{
		  mFirstPathValid = true;
		}

		// Set the destination
		if (_Target != null) { _TargetPosition = _Target.position; }
		SetDestination(_TargetPosition);

		// Determine if we're at the destination
		mTargetVector = mAgentDestination - _go.transform.position;

		if (_ignoreDeltaHeight.Value)
		  mTargetVector.y = 0;

		mTargetDistance = mTargetVector.magnitude;


		if (_onCollision != null && ExistsCollision())
		{
		  _hasArrived = true;
		  mFirstPathSet = false;
		  mFirstPathValid = false;

		  OnCollisionEmit();
		}

		// Check if we've arrived
		if (_UseNavMeshPosition)
		{
		  if (!_navMeshAgent.pathPending &&
				_navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete &&
				_navMeshAgent.remainingDistance == 0f)
		  {
			 ClearTarget();
			 _hasArrived = true;
			 mFirstPathSet = false;
			 mFirstPathValid = false;

			 OnArrived();
		  }
		}
		else
		{
		  if (mTargetDistance < _StopDistance.Value)
		  {
			 ClearTarget();
			 _hasArrived = true;
			 mFirstPathSet = false;
			 mFirstPathValid = false;

			 OnArrived();
		  }else if(_checkDistanceError.Value && mTargetDistance < _maxDistanceError.Value && mTargetDistance > _lastDiatanceError+0.05f)
		  {
			 ClearTarget();
			 _hasArrived = true;
			 mFirstPathSet = false;
			 mFirstPathValid = false;
			 OnArrived();
		  }
		}

		if(mTargetDistance < _lastDiatanceError)
		  _lastDiatanceError = mTargetDistance;

		// Determine the next move
		if (!_hasArrived && mFirstPathValid)
		{
		  // Hold on to our next position before we change it
		  if (_navMeshAgent.hasPath && !_navMeshAgent.pathPending)
		  {
			 mIsPathValid = true;

			 mWaypoint = _navMeshAgent.steeringTarget;

			 if (mTargetDistance > _SlowDistance.Value) { mIsInSlowDistance = false; }
		  }

		  // Determine if we're within the slow distance. We only want to fire the 
		  // event once
		  if (_SlowDistance.Value > 0f && mTargetDistance < _SlowDistance.Value)
		  {
			 if (!mIsInSlowDistance) { OnSlowDistanceEntered(); }
			 mIsInSlowDistance = true;
		  }

		  // Calculate 
		  CalculateMove(mWaypoint, ref lMovement, ref lRotation);

		  // Check if we've reached the destination
		  //if (!_navMeshAgent.pathPending)
		  {
			 SetActorRotation(lRotation);
			 SetActorMove(lMovement);
		  }

		  // Force the agent to stay with our actor. This way, the path is
		  // alway relative to our current position. Then, we can use the AC
		  // to move to a valid position.
		  if (!_UseNavMeshPosition)
		  {
			 _navMeshAgent.nextPosition = _go.transform.position;
		  }
		}
		else
		{
		  _frameCount++;

		  if(_frameCount >= 4)
		  {

			 ClearTarget();
			 _hasArrived = true;
			 mFirstPathSet = false;
			 mFirstPathValid = false;

			 OnArrived();
		  }
		}

	 }


	 protected virtual void SetActorRotation(Quaternion rotateion)
	 {
		_actor.Rotate(rotateion);
	 }

	 protected virtual void SetActorMove(Vector3 move)
	 {
		_actor.Move(move);
	 }

	 private bool ExistsCollision()
	 {
		for (int i = 0; i < _actor.BodyShapes.Count; i++)
		{
		  BodyShape lBodyShape = _actor.BodyShapes[i];


		  if ((lBodyShape.IsEnabledOnGround && _actor.State.IsGrounded) ||
				(lBodyShape.IsEnabledAboveGround && !_actor.State.IsGrounded))
		  {
			 // If there are collisions, we need to respond
			 BodyShapeHit[] lBodyShapeHits = lBodyShape.CollisionCastAll((_TargetPosition - _actor._Transform.position) * 0.2f, (_TargetPosition - _actor._Transform.position).normalized, 0.2f, _actor.CollisionLayers);
			 if (lBodyShapeHits != null && lBodyShapeHits.Length > 0)
			 {
				if (lBodyShapeHits[0] != null)
				  return true;
			 }
		  }
		}
		return false;
	 }

	 public void ClearTarget()
	 {
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
            mNavMeshAgent.Stop();
#else
		_navMeshAgent.isStopped = true;
#endif

		_hasArrived = false;
		mIsPathValid = true;
		mFirstPathSet = false;
		mFirstPathValid = false;
		mIsInSlowDistance = false;
		mAgentDestination = _actor.transform.position;

		_actor.SetRelativeVelocity(Vector3.zero);
	 }

	 protected virtual void SetDestination(Vector3 rDestination)
	 {
		if (!_hasArrived && mAgentDestination == rDestination) { return; }

		// Reset the properties
		_hasArrived = false;

		// Set the new destination
		mAgentDestination = rDestination;

		// Recalculate the path
		if (mIsPathValid && !_navMeshAgent.pathPending)
		{
		  mIsPathValid = false;

		  _navMeshAgent.updatePosition = false;
		  _navMeshAgent.updateRotation = false;
		  _navMeshAgent.stoppingDistance = _StopDistance.Value;

		  _navMeshAgent.ResetPath();
		  bool isDestenation = _navMeshAgent.SetDestination(mAgentDestination);

		  mFirstPathSet = true;
		}
	 }

	 protected virtual void CalculateMove(Vector3 rWaypoint, ref Vector3 rMove, ref Quaternion rRotate)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		// Direction we need to travel in
		Vector3 lDirection = rWaypoint - _go.transform.position;
		float distance = lDirection.magnitude;
		lDirection.y = lDirection.y - _PathHeight.Value;
		lDirection.Normalize();

		// Determine our rotation
		Vector3 lVerticalDirection = Vector3.Project(lDirection, _go.transform.up);
		Vector3 lLateralDirection = lDirection - lVerticalDirection;

		float lYawAngle = Vector3Ext.SignedAngle(_go.transform.forward, lLateralDirection);

		float lRotateSpeed = rotationSpeed.Value;

		if (_forceRotateInShort.Value && Mathf.Abs(lYawAngle) > 10 && distance < 0.5f)
		{
		  lRotateSpeed *= 3;
		}

		if (lRotateSpeed == 0f)
		{
		  rRotate = Quaternion.AngleAxis(lYawAngle, _go.transform.up);
		}
		else
		{
		  rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), lRotateSpeed * lDeltaTime), _go.transform.up);
		}

		// Determine the movement
		if (_UseNavMeshPosition)
		{
		  rMove = _navMeshAgent.nextPosition - _go.transform.position;
		}
		// In this case, we'll calculate movement ourselves
		else
		{
		  // Grab the base movement speed
		  float lMoveSpeed = movementSpeed.Value;

		  // Calculate our own slowing
		  float lRelativeMoveSpeed = 1f;
		  if (mIsInSlowDistance && _SlowFactor.Value > 0f)
		  {
			 float lSlowPercent = (mTargetDistance - _StopDistance.Value) / (_SlowDistance.Value - _StopDistance.Value);
			 lRelativeMoveSpeed = ((1f - _SlowFactor.Value) * lSlowPercent) + _SlowFactor.Value;
		  }

		  // TRT 4/5/2016: Force the slow distance as an absolute value
		  if (mIsInSlowDistance && _SlowFactor.Value > 0f)
		  {
			 lMoveSpeed = _SlowFactor.Value;
			 lRelativeMoveSpeed = 1f;
		  }

		  // Set the final velocity based on the future rotation
		  Quaternion lFutureRotation = _go.transform.rotation * rRotate;
		  rMove = lFutureRotation.Forward() * (lMoveSpeed * lRelativeMoveSpeed * lDeltaTime);
		  if(distance <= 0.5f && Mathf.Abs(lYawAngle) > 10)
			 rMove = lLateralDirection * (lMoveSpeed * lRelativeMoveSpeed * lDeltaTime);
		}
	 }

	 protected virtual void OnArrived()
	 {
	 }
	 protected virtual void OnCollisionEmit()
	 {
		Fsm.Event(_onCollision);
	 }

	 protected virtual void OnSlowDistanceEntered()
	 {
	 }

  }
}