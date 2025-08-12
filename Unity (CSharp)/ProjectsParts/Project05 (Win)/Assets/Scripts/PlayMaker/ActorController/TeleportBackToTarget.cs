using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Geometry;
using UnityEngine.AI;
using HutongGames.PlayMaker.Actions;
using System.Runtime.Remoting.Messaging;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  [HutongGames.PlayMaker.Tooltip("Телепорт на позицию сзади обьекта")]
  public class TeleportBackToTarget : ActorDriver
  {
	 [RequiredField]
	 public FsmGameObject target;

	 [RequiredField]
	 public FsmEvent _onFailed;

	 public FsmFloat _distanceBack = new FsmFloat(2);

	 private NavMeshAgent _agent;

	 public override void OnEnter()
	 {
		base.OnEnter();
		_agent = _go.GetComponent<NavMeshAgent>();

		Vector3 targetPosition = GetPosition();

		if (targetPosition == Vector3.zero)
		{
		  Fsm.Event(_onFailed);
		  return;
		}

		_actor.SetPosition(targetPosition);
		_actor.SetRotation(Quaternion.LookRotation(target.Value.transform.position - targetPosition));
		_actor.SetRelativeVelocity(Vector3.zero);
		_actor.SetVelocity(Vector3.zero);

		Finish();

	 }

	 private Vector3 GetPosition()
	 {
		Vector3 targetPos = target.Value.transform.position + target.Value.transform.forward * -_distanceBack.Value;

		if (CheckPath(targetPos) && CheckRay(targetPos))
		  return targetPos;
		else
		{
		  for(int i = 0; i < 20; i++)
		  {
			 targetPos = target.Value.transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * _distanceBack.Value;
			 if (CheckPath(targetPos) && CheckRay(targetPos))
				return targetPos;
		  }
		}

		return Vector3.zero;
	 }

	 private bool CheckPath(Vector3 target)
	 {
		NavMeshPath path = new NavMeshPath();
		_agent.CalculatePath(target, path);

		if (path.status == NavMeshPathStatus.PathComplete)
		{
		  if ((target - path.corners[path.corners.Length - 1]).magnitude < 0.25f)
			 return true;
		  else
			 return false;
		}
		return false;
	 }

	 private bool CheckRay(Vector3 targetPoint)
	 {
		Vector3 startPos = target.Value.transform.position + Vector3.up / 2;
		Vector3 targetPos = targetPoint + Vector3.up / 2;


		return RaycastExt.SafeSphereCast(startPos, (startPos - targetPos).normalized, _actor.Radius, (startPos - targetPos).magnitude, -1, target.Value.transform);
	 }

  }
}