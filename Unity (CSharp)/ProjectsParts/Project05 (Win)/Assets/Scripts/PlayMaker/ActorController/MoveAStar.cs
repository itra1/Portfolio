using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine.AI;
using com.ootii.Actors;
using com.ootii.Geometry;
using com.ootii.Timing;
using Pathfinding;
using Pathfinding.Util;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  public class MoveAStar : ActorDriver
  {
	 //[Tooltip("Игнорирование разницу в высоте")]

	 public FsmBool _checkDistanceError = true;
	 public FsmFloat _maxDistanceError = 0.5f;
	 public FsmBool _forceRotateInShort;
	 public FsmBool _ignoreDeltaHeight;
	 public FsmBool _stopIfCollision;
	 protected bool _stopEverySetPosition = true;
	 public FsmFloat repathRate = 0.5f;
	 public FsmBool canSearch = true;
	 /// <summary>\copydoc Pathfinding::IAstarAI::radius</summary>
	 public FsmFloat radius = 0.5f;

	 /// <summary>
	 /// Использовать движения с аниматора
	 /// </summary>
	 public bool _useRootMotion;

	 /// <summary>\copydoc Pathfinding::IAstarAI::height</summary>
	 public FsmFloat height = 2;

	 public FsmFloat pickNextWaypointDist = 2;
	 protected float lastRepath = float.NegativeInfinity;

	 private float _lastDiatanceError;

	 public FsmFloat _currentSpeed;

	 public float _speedChange = 1;
	 private float _actualSpeed = 0;

	 private Vector3 _finalNavPosition;

	 private bool _pathCalculate;

	 protected Transform _Target = null;
	 protected Transform Target
	 {
		get { return _Target; }

		set
		{
		  _Target = value;
		  if (_Target == null)
		  {
			 ClearTarget();
			 _hasArrived = false;
			 mIsInSlowDistance = false;
			 _frameCount = 0;

			 mIsTargetSet = false;
			 _TargetPosition = Vector3Ext.Null;

			 if (_stopEverySetPosition)
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
	 protected Vector3 _TargetPosition = Vector3.zero;
	 public Vector3 TargetPosition
	 {
		get { return _TargetPosition; }

		set
		{
		  _Target = null;
		  _TargetPosition = value;

		  if (_TargetPosition == Vector3Ext.Null)
		  {

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
			 SearchPath();
		  }
		}
	 }
	 public FsmFloat _StopDistance = new FsmFloat(0.3f);
	 public float StopDistance
	 {
		get { return _StopDistance.Value; }
		set { _StopDistance = value; }
	 }
	 public FsmFloat _SlowDistance = new FsmFloat(0f);
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
	 public bool pathPending
	 {
		get
		{
		  return waitingForPathCalculation;
		}
	 }
	 public bool hasPath
	 {
		get
		{
		  return interpolator.valid;
		}
	 }
	 public Vector3 steeringTarget
	 {
		get
		{
		  return interpolator.valid ? interpolator.position : position;
		}
	 }

	 /// <summary>\copydoc Pathfinding::IAstarAI::reachedEndOfPath</summary>
	 public bool reachedEndOfPath { get; protected set; }

	 protected bool waitingForPathCalculation = false;
	 protected Vector3 mWaypoint = Vector3.zero;
	 protected Vector3 mTargetVector = Vector3.zero;
	 protected float mTargetDistance = 0f;
	 protected bool mIsPathValid = true;
	 private int _frameCount = 0;
	 protected Seeker seeker;
	 protected Path path;
	 protected PathInterpolator interpolator = new PathInterpolator();

	 protected Animator _animator;

	 public static readonly Color ShapeGizmoColor = new Color(240 / 255f, 213 / 255f, 30 / 255f);
	 public Vector3 position { get { return _go.transform.position; } }

	 public float remainingDistance
	 {
		get
		{
		  return interpolator.valid ? interpolator.remainingDistance + movementPlane.ToPlane(interpolator.position - position).magnitude : float.PositiveInfinity;
		}
	 }

	 protected virtual bool shouldRecalculatePath
	 {
		get
		{
		  return Time.time - lastRepath >= repathRate.Value && !waitingForPathCalculation && canSearch.Value && !float.IsPositiveInfinity(_TargetPosition.x);
		}
	 }

	 public override void Awake()
	 {
		base.Awake();

		HandlesOnEvent = true;

		if (_useRootMotion)
		  Fsm.HandleAnimatorMove = true;

	 }

	 public override void Inicialization()
	 {
		base.Inicialization();

		if (_animator == null)
		  _animator = _go.GetComponent<Animator>();
	 }

	 /// <summary>
	 /// Plane which this agent is moving in.
	 /// This is used to convert between world space and a movement plane to make it possible to use this script in
	 /// both 2D games and 3D games.
	 /// </summary>
	 protected IMovementPlane movementPlane;

	 private bool _firstPath = false;

	 public override void OnEnter()
	 {
		if (_useRootMotion)
		  Fsm.HandleAnimatorMove = true;
		if (movementPlane == null)
		  movementPlane = GraphTransform.identityTransform;

		base.OnEnter();
		if (seeker == null)
		  seeker = _go.GetComponent<Pathfinding.Seeker>();

		seeker.pathCallback -= OnPathComplete;
		seeker.pathCallback += OnPathComplete;
		_firstPath = false;

		mTargetDistance = 999999999;
	 }

	 public override void OnExit()
	 {
		base.OnExit();
		_currentSpeed.Value = 0;
		seeker.pathCallback -= OnPathComplete;
		seeker.CancelCurrentPathRequest(true);
	 }

	 public override void OnDrawActionGizmos()
	 {
		if (!_hasArrived)
		  return;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(_go.transform.position, _TargetPosition);
	 }

	 public override void OnDrawActionGizmosSelected()
	 {

		var color = ShapeGizmoColor;
		//if (rvoController != null && rvoController.locked) color *= 0.5f;

		Draw.Gizmos.Cylinder(position, Owner.transform.rotation * Vector3.up, Owner.transform.localScale.y * height.Value, radius.Value * Owner.transform.localScale.x, color);

		if (!float.IsPositiveInfinity(_TargetPosition.x) && Application.isPlaying) Draw.Gizmos.CircleXZ(_TargetPosition, 0.2f, Color.blue);
	 }

	 public override void OnUpdate()
	 {
		base.OnUpdate();

		if (_actor == null) { return; }

		if (!_firstPath)
		  return;

		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;

		if (shouldRecalculatePath) SearchPath();

		if (_Target != null) { _TargetPosition = _Target.position; }
		// Set the destination
		//if (_Target != null) { _TargetPosition = _Target.position; }
		//SetDestination(_TargetPosition);

		interpolator.MoveToCircleIntersection2D(position, pickNextWaypointDist.Value, movementPlane);

		// Determine if we're at the destination
		mTargetVector = _finalNavPosition - _go.transform.position;

		if (_ignoreDeltaHeight.Value)
		  mTargetVector.y = 0;

		mTargetDistance = mTargetVector.magnitude;

		if (!_pathCalculate)
		{

		  if (mTargetDistance < _StopDistance.Value)
		  {
			 ClearTarget();
			 _hasArrived = true;

			 OnArrived();
		  }
		  else if (_checkDistanceError.Value && mTargetDistance < _maxDistanceError.Value && mTargetDistance > _lastDiatanceError + 0.05f)
		  {
			 ClearTarget();
			 _hasArrived = true;
			 OnArrived();
		  }
		}


		if (mTargetDistance < _lastDiatanceError)
		  _lastDiatanceError = mTargetDistance;

		// Determine the next move
		if (!_hasArrived)
		{
		  // Hold on to our next position before we change it
		  if (hasPath /*&& !pathPending*/)
		  {
			 mIsPathValid = true;

			 mWaypoint = steeringTarget;

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

		}
		else
		{
		  _frameCount++;

		  if (_frameCount >= 4)
		  {

			 ClearTarget();
			 _hasArrived = true;

			 OnArrived();
		  }
		}

	 }


	 public override void DoAnimatorMove()
	 {
		base.DoAnimatorMove();

		if (_useRootMotion)
		{
		  SetActorMove(_animator.deltaPosition);
		}

	 }

	 public override bool Event(FsmEvent fsmEvent)
	 {
		if(fsmEvent.Name == "DoAnimatorMove")
		{
		  DoAnimatorMove();
		}
		return false;
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
		ClearPath();
		_hasArrived = false;
		mIsPathValid = true;
		mIsInSlowDistance = false;
		//mAgentDestination = _actor.transform.position;

		_actor.SetRelativeVelocity(Vector3.zero);
	 }

	 //protected virtual void SetDestination(Vector3 rDestination)
	 //{
	 //if (!_hasArrived && mAgentDestination == rDestination) { return; }

	 //// Reset the properties
	 //_hasArrived = false;

	 //// Set the new destination
	 //mAgentDestination = rDestination;

	 //// Recalculate the path
	 //if (mIsPathValid && !_navMeshAgent.pathPending)
	 //{
	 //  mIsPathValid = false;

	 //  _navMeshAgent.updatePosition = false;
	 //  _navMeshAgent.updateRotation = false;
	 //  _navMeshAgent.stoppingDistance = _StopDistance.Value;

	 //  _navMeshAgent.ResetPath();
	 //  bool isDestenation = _navMeshAgent.SetDestination(mAgentDestination);

	 //  mFirstPathSet = true;
	 //}
	 //}

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

		// Grab the base movement speed
		float lMoveSpeed = movementSpeed.Value;

		//_currentSpeed.Value = lMoveSpeed / movementSpeed.Value;

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
		  lMoveSpeed *= _SlowFactor.Value;
		  _currentSpeed.Value = lMoveSpeed / movementSpeed.Value;
		  lRelativeMoveSpeed = 1f;
		}

		if (lMoveSpeed > _actualSpeed)
		{
		  _actualSpeed += movementSpeed.Value * (lDeltaTime * _speedChange);
		  if (_actualSpeed > lMoveSpeed)
			 _actualSpeed = lMoveSpeed;
		}
		else if (lMoveSpeed < _actualSpeed)
		{
		  _actualSpeed -= movementSpeed.Value * (lDeltaTime * _speedChange);
		  if (_actualSpeed < lMoveSpeed)
			 _actualSpeed = lMoveSpeed;
		}

		_currentSpeed.Value = _actualSpeed / movementSpeed.Value;

		// Set the final velocity based on the future rotation
		Quaternion lFutureRotation = _go.transform.rotation * rRotate;
		rMove = lFutureRotation.Forward() * (_actualSpeed * lDeltaTime);
		if (distance <= 0.5f && Mathf.Abs(lYawAngle) > 10)
		  rMove = lLateralDirection * (_actualSpeed * lDeltaTime);

	 }

	 protected virtual void OnArrived() { }

	 protected virtual void OnSlowDistanceEntered()
	 {
	 }

	 protected void OnPathComplete(Pathfinding.Path newPath)
	 {
		Pathfinding.ABPath p = newPath as Pathfinding.ABPath;

		if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");

		_pathCalculate = false;

		waitingForPathCalculation = false;

		// Increase the reference count on the new path.
		// This is used for object pooling to reduce allocations.
		p.Claim(this);

		// Path couldn't be calculated of some reason.
		// More info in p.errorLog (debug string)
		if (p.error)
		{
		  p.Release(this);
		  return;
		}

		// Release the previous path.
		if (path != null) path.Release(this);

		// Replace the old path
		path = p;

		// Make sure the path contains at least 2 points
		if (path.vectorPath.Count == 1) path.vectorPath.Add(path.vectorPath[0]);
		interpolator.SetPath(path.vectorPath);

		_finalNavPosition = path.vectorPath[path.vectorPath.Count - 1];

		var graph = path.path.Count > 0 ? AstarData.GetGraph(path.path[0]) as ITransformedGraph : null;
		movementPlane = graph != null ? graph.transform : GraphTransform.identityTransform;

		// Reset some variables
		reachedEndOfPath = false;

		// Simulate movement from the point where the path was requested
		// to where we are right now. This reduces the risk that the agent
		// gets confused because the first point in the path is far away
		// from the current position (possibly behind it which could cause
		// the agent to turn around, and that looks pretty bad).
		interpolator.MoveToLocallyClosestPoint((GetFeetPosition() + p.originalStartPoint) * 0.5f);
		interpolator.MoveToLocallyClosestPoint(GetFeetPosition());

		// Update which point we are moving towards.
		// Note that we need to do this here because otherwise the remainingDistance field might be incorrect for 1 frame.
		// (due to interpolator.remainingDistance being incorrect).
		interpolator.MoveToCircleIntersection2D(position, pickNextWaypointDist.Value, movementPlane);
		_firstPath = true;

		var distanceToEnd = remainingDistance;
		if (distanceToEnd <= _StopDistance.Value)
		{
		  reachedEndOfPath = true;
		  OnTargetReached();
		}
	 }

	 public virtual void SearchPath()
	 {
		if (float.IsPositiveInfinity(_TargetPosition.x)) return;
		//if (onSearchPath != null) onSearchPath();

		lastRepath = Time.time;
		waitingForPathCalculation = true;
		_hasArrived = false;

		seeker.CancelCurrentPathRequest();

		Vector3 start, end;
		CalculatePathRequestEndpoints(out start, out end);

		// Alternative way of requesting the path
		//ABPath p = ABPath.Construct(start, end, null);
		//seeker.StartPath(p);

		// This is where we should search to
		// Request a path to be calculated from our current position to the destination

		_pathCalculate = true;
		seeker.StartPath(start, end);
	 }

	 protected virtual void CalculatePathRequestEndpoints(out Vector3 start, out Vector3 end)
	 {
		start = GetFeetPosition();
		end = _TargetPosition;
	 }

	 protected void CancelCurrentPathRequest()
	 {
		waitingForPathCalculation = false;
		// Abort calculation of the current path
		if (seeker != null) seeker.CancelCurrentPathRequest();
	 }
	 public virtual Vector3 GetFeetPosition()
	 {
		return position;
	 }
	 public virtual void OnTargetReached()
	 {
	 }

	 protected void ClearPath()
	 {
		CancelCurrentPathRequest();
		interpolator.SetPath(null);
		reachedEndOfPath = false;
	 }



  }
}