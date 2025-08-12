using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Timing;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.ActorController
{

  [ActionCategory("Actor Controller")]
  [HutongGames.PlayMaker.Tooltip("Актор контроллер. Прямое движение к позиции")]
  public class MoveWall : MovePosition
  {
	 public FsmEvent OnComplete;
	 public FsmGameObject _targetObject;

	 private Vector3 toTarget;
	 private Quaternion ForvardRot;
	 private Quaternion left;
	 private Quaternion right;
	 private Vector3 checkPositionStart;
	 private Vector3 checkPositionEnd;

	 public override void OnEnter()
	 {
		base.OnEnter();
		_targetPosition = _targetObject.Value.transform.position;

		_actor.IsGravityEnabled = false;
		toTarget = _targetObject.Value.transform.position - _go.transform.position;
		ForvardRot = Quaternion.LookRotation(toTarget, Vector3.up);
		left = ForvardRot * Quaternion.Euler(0, -90, 0);
		right = ForvardRot * Quaternion.Euler(0, 90, 0);
		checkPositionStart = _go.transform.position + (toTarget.normalized * 2);
		checkPositionStart += Vector3.up * 2;
		checkPositionEnd = _go.transform.position + toTarget - (toTarget.normalized * 2);
		checkPositionEnd += Vector3.up * 2;


		RaycastHit _hit1,_hit2;

		RaycastExt.SafeRaycast(checkPositionStart, left * Vector3.forward, out _hit1, 5f, -1, _go.transform);
		RaycastExt.SafeRaycast(checkPositionEnd, left * Vector3.forward, out _hit2, 5f, -1, _go.transform);
		_actor.SetPosition(_hit1.point);
		_actor.IsGravityRelative = true;
		_actor.Gravity = left * Vector3.forward * 9;
		_targetPosition = _hit2.point;
	 }

	 public override void OnUpdate()
	 {
		base.OnUpdate();
	 }


	 protected override void OnArrived()
	 {
		Fsm.Event(OnComplete);
	 }

  }
}