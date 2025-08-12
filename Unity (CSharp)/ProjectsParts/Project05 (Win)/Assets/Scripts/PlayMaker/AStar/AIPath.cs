using UnityEngine;
using System.Collections;
using Pathfinding;
using UnityEngine.Serialization;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.AStar
{
  using System.Collections.Generic;

  using Pathfinding.Util;

  [ActionCategory("A* Pathfinding")]
  public class AIPath : AIBase, IAstarAI
  {
	 /// <summary>
	 /// How quickly the agent accelerates.
	 /// Positive values represent an acceleration in world units per second squared.
	 /// Negative values are interpreted as an inverse time of how long it should take for the agent to reach its max speed.
	 /// For example if it should take roughly 0.4 seconds for the agent to reach its max speed then this field should be set to -1/0.4 = -2.5.
	 /// For a negative value the final acceleration will be: -acceleration*maxSpeed.
	 /// This behaviour exists mostly for compatibility reasons.
	 ///
	 /// In the Unity inspector there are two modes: Default and Custom. In the Default mode this field is set to -2.5 which means that it takes about 0.4 seconds for the agent to reach its top speed.
	 /// In the Custom mode you can set the acceleration to any positive value.
	 /// </summary>
	 public FsmFloat maxAcceleration = -2.5f;

	 /// <summary>Distance from the end of the path where the AI will start to slow down</summary>
	 public FsmFloat slowdownDistance = 0.6F;

	 /// <summary>
	 /// How far the AI looks ahead along the path to determine the point it moves to.
	 /// In world units.
	 /// If you enable the <see cref="alwaysDrawGizmos"/> toggle this value will be visualized in the scene view as a blue circle around the agent.
	 /// [Open online documentation to see images]
	 ///
	 /// Here are a few example videos showing some typical outcomes with good values as well as how it looks when this value is too low and too high.
	 /// <table>
	 /// <tr><td>[Open online documentation to see videos]</td><td>\xmlonly <verbatim><span class="label label-danger">Too low</span><br/></verbatim>\endxmlonly A too low value and a too low acceleration will result in the agent overshooting a lot and not managing to follow the path well.</td></tr>
	 /// <tr><td>[Open online documentation to see videos]</td><td>\xmlonly <verbatim><span class="label label-warning">Ok</span><br/></verbatim>\endxmlonly A low value but a high acceleration works decently to make the AI follow the path more closely. Note that the <see cref="Pathfinding.AILerp"/> component is better suited if you want the agent to follow the path without any deviations.</td></tr>
	 /// <tr><td>[Open online documentation to see videos]</td><td>\xmlonly <verbatim><span class="label label-success">Ok</span><br/></verbatim>\endxmlonly A reasonable value in this example.</td></tr>
	 /// <tr><td>[Open online documentation to see videos]</td><td>\xmlonly <verbatim><span class="label label-success">Ok</span><br/></verbatim>\endxmlonly A reasonable value in this example, but the path is followed slightly more loosely than in the previous video.</td></tr>
	 /// <tr><td>[Open online documentation to see videos]</td><td>\xmlonly <verbatim><span class="label label-danger">Too high</span><br/></verbatim>\endxmlonly A too high value will make the agent follow the path too loosely and may cause it to try to move through obstacles.</td></tr>
	 /// </table>
	 /// </summary>
	 public FsmFloat pickNextWaypointDist = 2;

	 /// <summary>
	 /// Distance to the end point to consider the end of path to be reached.
	 /// When the end is within this distance then <see cref="OnTargetReached"/> will be called and <see cref="reachedEndOfPath"/> will return true.
	 /// </summary>
	 public FsmFloat endReachedDistance = 0.2F;

	 /// <summary>Draws detailed gizmos constantly in the scene view instead of only when the agent is selected and settings are being modified</summary>
	 public bool alwaysDrawGizmos;

	 /// <summary>
	 /// Slow down when not facing the target direction.
	 /// Incurs at a small performance overhead.
	 /// </summary>
	 public bool slowWhenNotFacingTarget = true;

	 /// <summary>
	 /// What to do when within <see cref="endReachedDistance"/> units from the destination.
	 /// The character can either stop immediately when it comes within that distance, which is useful for e.g archers
	 /// or other ranged units that want to fire on a target. Or the character can continue to try to reach the exact
	 /// destination point and come to a full stop there. This is useful if you want the character to reach the exact
	 /// point that you specified.
	 ///
	 /// Note: <see cref="reachedEndOfPath"/> will become true when the character is within <see cref="endReachedDistance"/> units from the destination
	 /// regardless of what this field is set to.
	 /// </summary>
	 public CloseToDestinationMode whenCloseToDestination = CloseToDestinationMode.Stop;

	 /// <summary>
	 /// Ensure that the character is always on the traversable surface of the navmesh.
	 /// When this option is enabled a <see cref="AstarPath.GetNearest"/> query will be done every frame to find the closest node that the agent can walk on
	 /// and if the agent is not inside that node, then the agent will be moved to it.
	 ///
	 /// This is especially useful together with local avoidance in order to avoid agents pushing each other into walls.
	 /// See: local-avoidance (view in online documentation for working links) for more info about this.
	 ///
	 /// This option also integrates with local avoidance so that if the agent is say forced into a wall by other agents the local avoidance
	 /// system will be informed about that wall and can take that into account.
	 ///
	 /// Enabling this has some performance impact depending on the graph type (pretty fast for grid graphs, slightly slower for navmesh/recast graphs).
	 /// If you are using a navmesh/recast graph you may want to switch to the <see cref="Pathfinding.RichAI"/> movement script which is specifically written for navmesh/recast graphs and
	 /// does this kind of clamping out of the box. In many cases it can also follow the path more smoothly around sharp bends in the path.
	 ///
	 /// It is not recommended that you use this option together with the funnel modifier on grid graphs because the funnel modifier will make the path
	 /// go very close to the border of the graph and this script has a tendency to try to cut corners a bit. This may cause it to try to go slightly outside the
	 /// traversable surface near corners and that will look bad if this option is enabled.
	 ///
	 /// Warning: This option makes no sense to use on point graphs because point graphs do not have a surface.
	 /// Enabling this option when using a point graph will lead to the agent being snapped to the closest node every frame which is likely not what you want.
	 ///
	 /// Below you can see an image where several agents using local avoidance were ordered to go to the same point in a corner.
	 /// When not constraining the agents to the graph they are easily pushed inside obstacles.
	 /// [Open online documentation to see images]
	 /// </summary>
	 public bool constrainInsideGraph = false;

	 /// <summary>Current path which is followed</summary>
	 protected Path path;

	 /// <summary>Helper which calculates points along the current path</summary>
	 protected PathInterpolator interpolator = new PathInterpolator();

	 #region IAstarAI implementation

	 /// <summary>\copydoc Pathfinding::IAstarAI::Teleport</summary>
	 public override void Teleport(Vector3 newPosition, bool clearPath = true)
	 {
		reachedEndOfPath = false;
		base.Teleport(newPosition, clearPath);
	 }

	 /// <summary>\copydoc Pathfinding::IAstarAI::remainingDistance</summary>
	 public float remainingDistance
	 {
		get
		{
		  return interpolator.valid ? interpolator.remainingDistance + movementPlane.ToPlane(interpolator.position - position).magnitude : float.PositiveInfinity;
		}
	 }

	 /// <summary>\copydoc Pathfinding::IAstarAI::reachedDestination</summary>
	 public bool reachedDestination
	 {
		get
		{
		  if (!reachedEndOfPath) return false;
		  if (remainingDistance + movementPlane.ToPlane(destination - interpolator.endPoint).magnitude > endReachedDistance.Value) return false;

			 // Check if the destination is above the head of the character or far below the feet of it
			 float yDifference;
			 movementPlane.ToPlane(destination - position, out yDifference);
			 var h = tr.localScale.y * height.Value;
			 if (yDifference > h || yDifference < -h * 0.5) return false;

		  return true;
		}
	 }

	 /// <summary>\copydoc Pathfinding::IAstarAI::reachedEndOfPath</summary>
	 public bool reachedEndOfPath { get; protected set; }

	 /// <summary>\copydoc Pathfinding::IAstarAI::hasPath</summary>
	 public bool hasPath
	 {
		get
		{
		  return interpolator.valid;
		}
	 }

	 /// <summary>\copydoc Pathfinding::IAstarAI::pathPending</summary>
	 public bool pathPending
	 {
		get
		{
		  return waitingForPathCalculation;
		}
	 }

	 /// <summary>\copydoc Pathfinding::IAstarAI::steeringTarget</summary>
	 public Vector3 steeringTarget
	 {
		get
		{
		  return interpolator.valid ? interpolator.position : position;
		}
	 }

	 /// <summary>\copydoc Pathfinding::IAstarAI::radius</summary>
	 float IAstarAI.radius { get { return radius.Value; } set { radius = value; } }

	 /// <summary>\copydoc Pathfinding::IAstarAI::height</summary>
	 float IAstarAI.height { get { return height.Value; } set { height = value; } }

	 /// <summary>\copydoc Pathfinding::IAstarAI::maxSpeed</summary>
	 float IAstarAI.maxSpeed { get { return movementSpeed.Value; } set { movementSpeed.Value = value; } }

	 /// <summary>\copydoc Pathfinding::IAstarAI::canSearch</summary>
	 bool IAstarAI.canSearch { get { return canSearch.Value; } set { canSearch = value; } }

	 /// <summary>\copydoc Pathfinding::IAstarAI::canMove</summary>
	 bool IAstarAI.canMove { get { return canMove.Value; } set { canMove = value; } }

	 #endregion

	 protected override void OnDisable()
	 {
		base.OnDisable();

		// Release current path so that it can be pooled
		if (path != null) path.Release(this);
		path = null;
		interpolator.SetPath(null);
	 }

	 /// <summary>
	 /// The end of the path has been reached.
	 /// If you want custom logic for when the AI has reached it's destination add it here. You can
	 /// also create a new script which inherits from this one and override the function in that script.
	 ///
	 /// This method will be called again if a new path is calculated as the destination may have changed.
	 /// So when the agent is close to the destination this method will typically be called every <see cref="repathRate"/> seconds.
	 /// </summary>
	 public virtual void OnTargetReached()
	 {
	 }

	 /// <summary>
	 /// Called when a requested path has been calculated.
	 /// A path is first requested by <see cref="UpdatePath"/>, it is then calculated, probably in the same or the next frame.
	 /// Finally it is returned to the seeker which forwards it to this function.
	 /// </summary>
	 protected override void OnPathComplete(Path newPath)
	 {
		ABPath p = newPath as ABPath;

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
		interpolator.MoveToCircleIntersection2D(position, pickNextWaypointDist.Value, movementPlane);

		var distanceToEnd = remainingDistance;
		if (distanceToEnd <= endReachedDistance.Value)
		{
		  reachedEndOfPath = true;
		  OnTargetReached();
		}
	 }

	 protected override void ClearPath()
	 {
		CancelCurrentPathRequest();
		interpolator.SetPath(null);
		reachedEndOfPath = false;
	 }

	 /// <summary>Called during either Update or FixedUpdate depending on if rigidbodies are used for movement or not</summary>
	 protected override void MovementUpdateInternal(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation)
	 {
		float currentAcceleration = maxAcceleration.Value;

		// If negative, calculate the acceleration from the max speed
		if (currentAcceleration < 0) currentAcceleration *= -movementSpeed.Value;

		if (updatePosition)
		{
		  // Get our current position. We read from transform.position as few times as possible as it is relatively slow
		  // (at least compared to a local variable)
		  simulatedPosition = tr.position;
		}
		if (updateRotation) simulatedRotation = tr.rotation;

		var currentPosition = simulatedPosition;

		// Update which point we are moving towards
		interpolator.MoveToCircleIntersection2D(currentPosition, pickNextWaypointDist.Value, movementPlane);
		var dir = movementPlane.ToPlane(steeringTarget - currentPosition);

		// Calculate the distance to the end of the path
		float distanceToEnd = dir.magnitude + Mathf.Max(0, interpolator.remainingDistance);

		// Check if we have reached the target
		var prevTargetReached = reachedEndOfPath;
		reachedEndOfPath = distanceToEnd <= endReachedDistance.Value && interpolator.valid;
		if (!prevTargetReached && reachedEndOfPath) OnTargetReached();
		float slowdown;

		// Normalized direction of where the agent is looking
		var forwards = movementPlane.ToPlane(simulatedRotation * Vector3.forward);

		// Check if we have a valid path to follow and some other script has not stopped the character
		if (interpolator.valid && !isStopped)
		{
		  // How fast to move depending on the distance to the destination.
		  // Move slower as the character gets closer to the destination.
		  // This is always a value between 0 and 1.
		  slowdown = distanceToEnd < slowdownDistance.Value ? Mathf.Sqrt(distanceToEnd / slowdownDistance.Value) : 1;

		  if (reachedEndOfPath && whenCloseToDestination == CloseToDestinationMode.Stop)
		  {
			 // Slow down as quickly as possible
			 velocity2D -= Vector2.ClampMagnitude(velocity2D, currentAcceleration * deltaTime);
		  }
		  else
		  {
			 velocity2D += MovementUtilities.CalculateAccelerationToReachPoint(dir, dir.normalized * movementSpeed.Value, velocity2D, currentAcceleration, rotationSpeed.Value, movementSpeed.Value, forwards) * deltaTime;
		  }
		}
		else
		{
		  slowdown = 1;
		  // Slow down as quickly as possible
		  velocity2D -= Vector2.ClampMagnitude(velocity2D, currentAcceleration * deltaTime);
		}

		velocity2D = MovementUtilities.ClampVelocity(velocity2D, movementSpeed.Value, slowdown, slowWhenNotFacingTarget && enableRotation, forwards);

		ApplyGravity(deltaTime);

		// Set how much the agent wants to move during this frame
		var delta2D = lastDeltaPosition = CalculateDeltaToMoveThisFrame(movementPlane.ToPlane(currentPosition), distanceToEnd, deltaTime);
		nextPosition = currentPosition + movementPlane.ToWorld(delta2D, 0);
		CalculateNextRotation(slowdown, out nextRotation);
	 }

	 protected virtual void CalculateNextRotation(float slowdown, out Quaternion nextRotation)
	 {
		if (lastDeltaTime > 0.00001f && enableRotation)
		{
		  Vector2 desiredRotationDirection;
		  desiredRotationDirection = velocity2D;

		  // Rotate towards the direction we are moving in.
		  // Don't rotate when we are very close to the target.
		  var currentRotationSpeed = rotationSpeed.Value * Mathf.Max(0, (slowdown - 0.3f) / 0.7f);
		  nextRotation = SimulateRotationTowards(desiredRotationDirection, currentRotationSpeed * lastDeltaTime);
		}
		else
		{
		  // TODO: simulatedRotation
		  nextRotation = rotation;
		}
	 }

	 protected void ApplyGravity(float deltaTime)
	 {
		// Apply gravity
		//if (usingGravity)
		//{
		//  float verticalGravity;
		//  velocity2D += movementPlane.ToPlane(deltaTime * (float.IsNaN(gravity.x) ? Physics.gravity : gravity), out verticalGravity);
		//  verticalVelocity += verticalGravity;
		//}
		//else
		//{
		//  verticalVelocity = 0;
		//}
	 }

	 static NNConstraint cachedNNConstraint = NNConstraint.Default;
	 protected override Vector3 ClampToNavmesh(Vector3 position, out bool positionChanged)
	 {
		if (constrainInsideGraph)
		{
		  cachedNNConstraint.tags = seeker.traversableTags;
		  cachedNNConstraint.graphMask = seeker.graphMask;
		  cachedNNConstraint.distanceXZ = true;
		  var clampedPosition = AstarPath.active.GetNearest(position, cachedNNConstraint).position;

		  // We cannot simply check for equality because some precision may be lost
		  // if any coordinate transformations are used.
		  var difference = movementPlane.ToPlane(clampedPosition - position);
		  float sqrDifference = difference.sqrMagnitude;
		  if (sqrDifference > 0.001f * 0.001f)
		  {
			 // The agent was outside the navmesh. Remove that component of the velocity
			 // so that the velocity only goes along the direction of the wall, not into it
			 velocity2D -= difference * Vector2.Dot(difference, velocity2D) / sqrDifference;

			 positionChanged = true;
			 // Return the new position, but ignore any changes in the y coordinate from the ClampToNavmesh method as the y coordinates in the navmesh are rarely very accurate
			 return position + movementPlane.ToWorld(difference);
		  }
		}

		positionChanged = false;
		return position;
	 }

#if UNITY_EDITOR
	 [System.NonSerialized]
	 int gizmoHash = 0;

	 [System.NonSerialized]
	 float lastChangedTime = float.NegativeInfinity;

	 protected static readonly Color GizmoColor = new Color(46.0f / 255, 104.0f / 255, 201.0f / 255);

	 protected override void OnDrawGizmos()
	 {
		base.OnDrawGizmos();
		if (alwaysDrawGizmos) OnDrawGizmosInternal();
	 }

	 protected override void OnDrawGizmosSelected()
	 {
		base.OnDrawGizmosSelected();
		if (!alwaysDrawGizmos) OnDrawGizmosInternal();
	 }

	 void OnDrawGizmosInternal()
	 {
		var newGizmoHash = pickNextWaypointDist.Value.GetHashCode() ^ slowdownDistance.Value.GetHashCode() ^ endReachedDistance.GetHashCode();

		if (newGizmoHash != gizmoHash && gizmoHash != 0) lastChangedTime = Time.realtimeSinceStartup;
		gizmoHash = newGizmoHash;
		float alpha = alwaysDrawGizmos ? 1 : Mathf.SmoothStep(1, 0, (Time.realtimeSinceStartup - lastChangedTime - 5f) / 0.5f) * (UnityEditor.Selection.gameObjects.Length == 1 ? 1 : 0);

		if (alpha > 0)
		{
		  // Make sure the scene view is repainted while the gizmos are visible
		  if (!alwaysDrawGizmos) UnityEditor.SceneView.RepaintAll();
		  Draw.Gizmos.Line(position, steeringTarget, GizmoColor * new Color(1, 1, 1, alpha));
		  Gizmos.matrix = Matrix4x4.TRS(position, _go.transform.rotation * Quaternion.identity, Vector3.one);
		  Draw.Gizmos.CircleXZ(Vector3.zero, pickNextWaypointDist.Value, GizmoColor * new Color(1, 1, 1, alpha));
		  Draw.Gizmos.CircleXZ(Vector3.zero, slowdownDistance.Value, Color.Lerp(GizmoColor, Color.red, 0.5f) * new Color(1, 1, 1, alpha));
		  Draw.Gizmos.CircleXZ(Vector3.zero, endReachedDistance.Value, Color.Lerp(GizmoColor, Color.red, 0.8f) * new Color(1, 1, 1, alpha));
		}
	 }
#endif

	 protected override int OnUpgradeSerializedData(int version, bool unityThread)
	 {
		base.OnUpgradeSerializedData(version, unityThread);
		// Approximately convert from a damping value to a degrees per second value.
		if (version < 1) rotationSpeed.Value *= 90;
		return 2;
	 }

	 public void GetRemainingPath(List<Vector3> buffer, out bool stale)
	 {
		buffer.Clear();
		buffer.Add(position);
		if (!interpolator.valid)
		{
		  stale = true;
		  return;
		}

		stale = false;
		interpolator.GetRemainingPath(buffer);
	 }
  }
}