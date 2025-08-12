using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Geometry;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using UnityEngine.AI;
using DG.Tweening;

namespace it.Game.PlayMaker.Boses.TheKarmamancer
{
  /// <summary>
  /// Большой каст
  /// </summary>
  [ActionCategory("Enemyes")]
  [Tooltip("Перемещение за спину игрока")]
  public class TheKarmamancerTeleportToBackPlayer : it.Game.PlayMaker.ActorController.ActorDriver
  {

	 public FsmGameObject target;
	 public FsmFloat _distanceBack = 2;

	 public FsmGameObject fogObject;

	 public FsmEvent OnComplete;
	 public FsmEvent OnFailed;

	 private NavMeshAgent _agent;

	 public override void OnEnter()
	 {
		base.OnEnter();

		_agent = _go.GetComponent<NavMeshAgent>();

		Vector3 targetPosition = GetPosition();

		if (targetPosition == Vector3.zero)
		{
		  Fsm.Event(OnFailed);
		}

		var fog = fogObject.Value.GetComponent<it.Game.NPC.Enemyes.Boses.Karmamancer.KarmamancerMovePosition>();

		fog.Spawn(Owner.transform.position);
		fog.Spawn(targetPosition);

		DOVirtual.DelayedCall(3, () =>
		{
		  _actor.SetPosition(targetPosition);
		  _actor.SetRotation(Quaternion.LookRotation(target.Value.transform.position - targetPosition));
		  _actor.SetRelativeVelocity(Vector3.zero);
		  _actor.SetVelocity(Vector3.zero);
		  CompleteEmit();
		});


		//  var sourceFog = CreateInstantiateFog();
		//sourceFog.transform.position = Owner.transform.position;
		//sourceFog.SetActive(true);
		//var targetFog = CreateInstantiateFog();
		//targetFog.transform.position = targetPosition;
		//targetFog.SetActive(true);

		//sourceFog.GetComponent<it.Game.NPC.Enemyes.Boses.Karmamancer.KarmamancerMovePosition>().Activate();
		//targetFog.GetComponent<it.Game.NPC.Enemyes.Boses.Karmamancer.KarmamancerMovePosition>().Activate();

		//DOVirtual.DelayedCall(4, () =>
		//{
		//  sourceFog.GetComponent<it.Game.NPC.Enemyes.Boses.Karmamancer.KarmamancerMovePosition>().Deactivate();
		//  targetFog.GetComponent<it.Game.NPC.Enemyes.Boses.Karmamancer.KarmamancerMovePosition>().Deactivate();
		//  _actor.SetPosition(targetPosition);
		//  _actor.SetRotation(Quaternion.LookRotation(target.Value.transform.position - targetPosition));
		//  _actor.SetRelativeVelocity(Vector3.zero);
		//  _actor.SetVelocity(Vector3.zero);
		//  CompleteEmit();

		//});


	 }

	 private void CompleteEmit()
	 {
		Fsm.Event(OnComplete);
	 }

	 private GameObject CreateInstantiateFog()
	 {
		GameObject go = MonoBehaviour.Instantiate(fogObject.Value);
		return go;
	 }

	 private Vector3 GetPosition()
	 {
		Vector3 targetPos = target.Value.transform.position + target.Value.transform.forward * -_distanceBack.Value;

		if (CheckPath(targetPos) && CheckRay(targetPos))
		  return targetPos;
		else
		{
		  for (int i = 0; i < 20; i++)
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
		return true;

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

		return !RaycastExt.SafeSphereCast(startPos, (startPos - targetPos).normalized, _actor.Radius, (startPos - targetPos).magnitude, -1, target.Value.transform);
	 }

  }
}