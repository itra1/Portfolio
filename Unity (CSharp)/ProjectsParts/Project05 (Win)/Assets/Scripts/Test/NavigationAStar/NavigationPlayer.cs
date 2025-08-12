using System.Collections;
using Pathfinding;
using UnityEngine;
using Pathfinding.Util;

namespace Game.Assets.Scripts.Test.NavigationAStar
{
  public class NavigationPlayer : MonoBehaviour
  {
	 [SerializeField]
	 private Transform _target;

	 [SerializeField]
	 private Seeker _seeker;
	 private Path path;
	 protected PathInterpolator interpolator = new PathInterpolator();
	 protected bool waitingForPathCalculation = false;
	 public float pickNextWaypointDist = 2;
	 public float endReachedDistance = 0.2F;
	 public bool reachedEndOfPath { get; protected set; }
	 public Vector3 position { get { return transform.position; } }
	 private int segment;

	 [SerializeField]
	 private float _speed = 0.5f;

	 public Vector3 steeringTarget
	 {
		get
		{
		  return interpolator.valid ? interpolator.position : position;
		}
	 }
	 public IMovementPlane movementPlane = GraphTransform.identityTransform; 
	 public float remainingDistance
	 {
		get
		{
		  return interpolator.valid ? interpolator.remainingDistance + movementPlane.ToPlane(interpolator.position - position).magnitude : float.PositiveInfinity;
		}
	 }

	 private float _timepath;
	 private bool _UpdatePath
	 {
		get
		{
			 return _timepath < Time.timeSinceLevelLoad - 0.3f;
		}
	 }

	 private void Start()
	 {
		_seeker.pathCallback += OnPathComplete;

		path = _seeker.StartPath(transform.position, _target.position);
		_seeker.GetCurrentPath();
	 }

	 private void Update()
	 {
		path = _seeker.GetCurrentPath();

		if (_UpdatePath)
		{
		  _timepath = Time.timeSinceLevelLoad;
		  path = _seeker.StartPath(transform.position, _target.position);
		}

		Vector3 targetMove = GetTargetPosition();

		Vector3 targetvect = targetMove - transform.position;

		if (targetvect.magnitude == 0)
		  return;

		transform.position += targetvect.normalized * _speed*Time.deltaTime;

	 }

	 Vector3 GetTargetPosition()
	 {
		while (segment < path.vectorPath.Count)
		{

		  Vector3 target = path.vectorPath[segment];
		  Vector3 velocity = target - transform.position;
		  Vector3 nextPosition = transform.position + velocity.normalized * _speed * Time.deltaTime;

		  if (velocity.magnitude > _speed * Time.deltaTime)
		  {
			 if ((target - transform.position).magnitude < _speed * Time.deltaTime)
			 {
				segment++;
				continue;
			 }

			 return target;
		  }

		  segment++;
		}
		  return transform.position;

	 }

	 private void OnDrawGizmos()
	 {
		if (Application.isPlaying && path != null)
		{
		  for (int i = 0; i < path.vectorPath.Count; i++)
		  {
			 if (i == 0)
				continue;
			 Gizmos.DrawLine(path.vectorPath[i - 1], path.vectorPath[i]);
		  }
		}
	 }

	 protected void OnPathComplete(Path newPath)
	 {
		ABPath p = newPath as ABPath;
		segment = 0;

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
		interpolator.MoveToCircleIntersection2D(position, pickNextWaypointDist, movementPlane);

		var distanceToEnd = remainingDistance;
		if (distanceToEnd <= endReachedDistance)
		{
		  reachedEndOfPath = true;
		  OnTargetReached();
		}
	 }
	 public virtual Vector3 GetFeetPosition()
	 {
		return position;
	 }
	 public virtual void OnTargetReached()
	 {
	 }

  }
}