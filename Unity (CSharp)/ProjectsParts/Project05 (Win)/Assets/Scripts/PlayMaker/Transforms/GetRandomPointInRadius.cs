using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Geometry;
using it.Game.Utils;
using Pathfinding.Util;
using Pathfinding;


namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Transforms")]
  public class GetRandomPointInRadius : FsmStateAction
  {
	 [RequiredField]
	 public FsmGameObject _gameObject;
	 [RequiredField]
	 public FsmGameObject _objectCenter;
	 public FsmFloat radius = 3;
	 public FsmFloat _minDistance = 2;
	 public FsmVector3 targetPosition;

	 public FsmBool aStar;

	 public FsmEvent onFailed;

	 public FsmBool _useNavMeshAgent;
	 public FsmBool _sendFailedEvent = false;

	 public override void Reset()
	 {
		_gameObject = null;
		_objectCenter = null;
		radius.Value = 3;
		_minDistance.Value = 2;
		targetPosition = null;
	 }

	 private UnityEngine.AI.NavMeshAgent _agent;
	 private Pathfinding.Seeker _seeker;

	 public override void Awake()
	 {
		if (aStar.Value)
		{
		  _seeker = _gameObject.Value.GetComponent<Pathfinding.Seeker>();
		}
		
		if (_useNavMeshAgent.Value)
		{
		  _agent = _gameObject.Value.GetComponent<UnityEngine.AI.NavMeshAgent>();
		}
	 }

	 public override void OnEnter()
	 {
		bool isFind = FindNewPosition();

		if (!aStar.Value)
		{

		  if (!_sendFailedEvent.Value)
		  {
			 Finish();
		  }
		  else
		  {
			 if (isFind)
			 {
				_agent.ResetPath();
				Finish();
			 }
			 else
				Fsm.Event(onFailed);
		  }
		}


	 }

	 private bool FindNewPosition()
	 {
		for (int i = 0; i < 20; i++)
		{
		  Vector3 point = _objectCenter.Value.transform.position
			 + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * Random.Range(0.1f, radius.Value);

		  if (!CheckPointGround(ref point))
			 continue;

		  if ((point - _gameObject.Value.transform.position).magnitude < _minDistance.Value)
			 continue;

		  //targetPosition.Value = point;

		  if (aStar.Value)
		  {
			 _seeker.StartPath(Owner.transform.position, point, OnPathComplete);
		  }

		  if (_useNavMeshAgent.Value)
		  {
			 Vector3 pantEnd = NavMeshHelpers.CheckPath(_agent, point);
			 if (pantEnd == Vector3.zero || (pantEnd - targetPosition.Value).magnitude > 0.25f)
				continue;

			 targetPosition.Value = pantEnd;
			 return true;

		  }
		  else
			 return true;

		};

		return false;
	 }

	 protected PathInterpolator interpolator = new PathInterpolator();
	 protected void OnPathComplete(Pathfinding.Path newPath)
	 {
		Pathfinding.ABPath p = newPath as Pathfinding.ABPath;

		if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");


		// Increase the reference count on the new path.
		// This is used for object pooling to reduce allocations.
		p.Claim(this);

		// Path couldn't be calculated of some reason.
		// More info in p.errorLog (debug string)
		if (p.error)
		{
		  p.Release(this);
		  FindNewPosition();
		  return;
		}

		// Release the previous path.
		//if (path != null) path.Release(this);

		// Replace the old path
		Path path = p;


		// Make sure the path contains at least 2 points
		if (path.vectorPath.Count == 1) path.vectorPath.Add(path.vectorPath[0]);
		interpolator.SetPath(path.vectorPath);
		targetPosition.Value = interpolator.endPoint;

		Finish();

	 }

	 private bool CheckPointGround(ref Vector3 point)
	 {

		RaycastHit _hit;
		if (!RaycastExt.SafeRaycast(point + Vector3.up * 5, Vector3.down, out _hit, 15, ProjectSettings.GroundLayerMaks))
		  return false;
		point = _hit.point;

		return true;
	 }

	 public override void OnDrawActionGizmos()
	 {
#if UNITY_EDITOR
		if (_objectCenter.Value == null)
		{
		  return;
		}
		UnityEditor.Handles.color = Color.green;
		UnityEditor.Handles.DrawWireDisc(_objectCenter.Value.transform.position, Vector3.up, radius.Value);
#endif
	 }

  }
}