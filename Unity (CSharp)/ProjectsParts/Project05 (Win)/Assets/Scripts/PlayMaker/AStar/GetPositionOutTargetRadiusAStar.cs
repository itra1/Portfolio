using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using com.ootii.Geometry;
using it.Game.Utils;

namespace it.Game.PlayMaker.Transforms
{
  [ActionCategory("Actor Controller")]
  public class GetPositionOutTargetRadiusAStar : FsmStateAction
  {

	 public FsmGameObject _gameObject;
	 public FsmGameObject source;
	 public FsmFloat radius = 5;
	 public FsmBool outNormal = true;
	 public FsmVector3 targetPosition;

	 public FsmBool _useNavMeshAgent;

	 private UnityEngine.AI.NavMeshAgent _agent;

	 public override void Awake()
	 {
		if (_useNavMeshAgent.Value)
		{
		  _agent = _gameObject.Value.GetComponent<UnityEngine.AI.NavMeshAgent>();
		}
	 }

	 public override void OnEnter()
	 {
		FindNewPosition();
		Finish();
	 }

	 private bool FindNewPosition()
	 {
		Vector3 targetPoint = Vector3.zero;

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