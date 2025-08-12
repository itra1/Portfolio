using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using com.ootii.Geometry;
using it.Game.Utils;
using Pathfinding;
using Pathfinding.Util;

namespace it.Game.PlayMaker.Transforms
{
  [ActionCategory("Transform")]
  public class GetPositionOutTargetRadius : FsmStateAction
  {

	 public FsmGameObject _gameObject;
	 public FsmGameObject source;
	 public FsmFloat radius = 5;
	 public FsmBool outNormal = true;
	 public FsmVector3 targetPosition;

	 public FsmBool _useNavMeshAgent;
	 public FsmBool _useAStar;

	 private UnityEngine.AI.NavMeshAgent _agent;
	 public Vector3 position { get { return _gameObject.Value.transform.position; } }

	 public FsmFloat pickNextWaypointDist = 2;
	 private Seeker _seeker;
	 protected Path path;
	 protected PathInterpolator interpolator = new PathInterpolator();
	 protected bool waitingForPathCalculation = false;
	 private Vector3 _finalNavPosition;
	 protected IMovementPlane movementPlane;
	 private bool _firstPath = false;
	 public bool reachedEndOfPath { get; protected set; }
	 public float remainingDistance
	 {
		get
		{
		  return interpolator.valid ? interpolator.remainingDistance + movementPlane.ToPlane(interpolator.position - position).magnitude : float.PositiveInfinity;
		}
	 }
	 public FsmFloat _StopDistance = new FsmFloat(0.1f);
	 public float StopDistance
	 {
		get { return _StopDistance.Value; }
		set { _StopDistance = value; }
	 }

	 public override void Awake()
	 {
		if (_useNavMeshAgent.Value)
		{
		  _agent = _gameObject.Value.GetComponent<UnityEngine.AI.NavMeshAgent>();
		}
		else if (_useAStar.Value)
		{
		  _seeker = _gameObject.Value.GetComponent<Seeker>();
		}
	 }

	 public override void OnEnter()
	 {
		if (_useAStar.Value)
		{
		  _seeker.pathCallback -= OnPathComplete;
		  _seeker.pathCallback += OnPathComplete;
		}

		FindNewPosition();
		if(!_useAStar.Value)
		  Finish();
	 }

	 public override void OnExit()
	 {
		if (_useAStar.Value)
		{
		  _seeker.pathCallback -= OnPathComplete;
		  _seeker.CancelCurrentPathRequest(true);
		}
	 }


	 protected void OnPathComplete(Pathfinding.Path newPath)
	 {
		Pathfinding.ABPath p = newPath as Pathfinding.ABPath;

		if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");

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

		if (_finalNavPosition != Vector3.zero && (_finalNavPosition - targetPoint).magnitude < 0.25f)
		{
		  targetPosition.Value = _finalNavPosition;
		  Finish();
		}
		else {
		  FindNewPosition();
		}

	 }
	 public virtual Vector3 GetFeetPosition()
	 {
		return _gameObject.Value.transform.position;
	 }
	 public virtual void OnTargetReached()
	 {
	 }

	 Vector3 targetPoint;
	 private bool FindNewPosition()
	 {
		targetPoint = Vector3.zero;

		if (outNormal.Value)
		{
		  Vector3 direction = _gameObject.Value.transform.position - source.Value.transform.position;
		  direction.Normalize();
		  targetPoint = source.Value.transform.position + direction * radius.Value;


		  if (CheckPointGround(ref targetPoint))
		  {
			 if (_useNavMeshAgent.Value)
			 {
				Vector3 pantEnd = NavMeshHelpers.CheckPath(_agent, targetPoint);
				if (pantEnd != Vector3.zero && (pantEnd - targetPoint).magnitude < 0.25f)
				{
				  targetPosition.Value = pantEnd;
				  return true;
				}
			 }
			 else if (_useAStar.Value)
			 {
				_seeker.StartPath(_gameObject.Value.transform.position, targetPoint);
				return false;
			 }
			 else
				return true;
		  }

		  float degree = 0;

		  while (degree < 360)
		  {
			 degree += 10;
			 Quaternion look = Quaternion.Euler(direction) * Quaternion.Euler(0, degree, 0);

			 targetPoint = source.Value.transform.position + look.Forward() * radius.Value;

			 if (CheckPointGround(ref targetPoint))
			 {

				if (_useNavMeshAgent.Value)
				{
				  Vector3 pantEnd = NavMeshHelpers.CheckPath(_agent, targetPoint);
				  if (pantEnd != Vector3.zero && (pantEnd - targetPoint).magnitude < 0.25f)
				  {
					 targetPosition.Value = pantEnd;
					 return true;
				  }
				}
				else if (_useAStar.Value)
				{
				  _seeker.StartPath(_gameObject.Value.transform.position, targetPoint);
				  return false;
				}
				else
				  return true;
			 }
			 look = Quaternion.Euler(direction) * Quaternion.Euler(0, -degree, 0);

			 targetPoint = source.Value.transform.position + look.Forward() * radius.Value;

			 if (CheckPointGround(ref targetPoint))
			 {

				if (_useNavMeshAgent.Value)
				{
				  Vector3 pantEnd = NavMeshHelpers.CheckPath(_agent, targetPoint);
				  if (pantEnd != Vector3.zero && (pantEnd - targetPoint).magnitude < 0.25f)
				  {
					 targetPosition.Value = pantEnd;
					 return true;
				  }
				}
				else if (_useAStar.Value)
				{
				  _seeker.StartPath(_gameObject.Value.transform.position, targetPoint);
				  return false;
				}
				else
				  return true;
			 }


		  }
		}

		if (!outNormal.Value)
		{
		  for (int i = 0; i < 20; i++)
		  {
			 Vector3 point = _gameObject.Value.transform.position
				+ new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * radius.Value;

			 if (!CheckPointGround(ref point))
				continue;

			 if (_useNavMeshAgent.Value)
			 {
				Vector3 pantEnd = NavMeshHelpers.CheckPath(_agent, targetPoint);
				if (pantEnd != Vector3.zero && (pantEnd - targetPoint).magnitude < 0.25f)
				{
				  targetPosition.Value = pantEnd;
				  return true;
				}
				else
				  continue;
			 }
			 else if (_useAStar.Value)
			 {
				_seeker.StartPath(_gameObject.Value.transform.position, targetPoint);
				return false;
			 }
			 else
				return true;


		  };
		}
		return false;
	 }

	 private bool CheckPointGround(ref Vector3 point)
	 {

		RaycastHit _hit;
		if (!RaycastExt.SafeRaycast(point + Vector3.up * 5, Vector3.down, out _hit, 15, ProjectSettings.GroundLayerMaks))
		  return false;
		point = _hit.point;

		return true;
	 }
  }
}