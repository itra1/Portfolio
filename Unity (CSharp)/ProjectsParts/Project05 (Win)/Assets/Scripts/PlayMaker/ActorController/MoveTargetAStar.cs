using UnityEngine;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.ActorController
{
  public class MoveTargetAStar : MoveAStar
  {
	 public FsmGameObject TargetObject;
	 public FsmVector3 TargetPoint;

	 public FsmEvent onMoveComplete;

	 public FsmBool everyUpdate;
	 public float updatePeriod = 0.5f;

	 private float _updateTime;

	 public override void OnEnter()
	 {
		base.OnEnter();
		_updateTime = Time.timeSinceLevelLoad;
		SetTarget();
	 }

	 public override void OnUpdate()
	 {
		base.OnUpdate();

		if (everyUpdate.Value && _updateTime < Time.timeSinceLevelLoad - updatePeriod)
		{
		  _updateTime = Time.timeSinceLevelLoad;
		  SetTarget();
		}
	 }

	 private void SetTarget()
	 {

		if (TargetObject.Value != null && !TargetObject.Value.Equals(Owner))
		  TargetPosition = TargetObject.Value.transform.position;
		else
		  TargetPosition = TargetPoint.Value;
	 }

	 protected override void OnArrived()
	 {
		Fsm.Event(onMoveComplete);
	 }
  }
}