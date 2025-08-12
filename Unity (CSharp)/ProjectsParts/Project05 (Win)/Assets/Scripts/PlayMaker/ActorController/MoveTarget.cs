using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Timing;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  [HutongGames.PlayMaker.Tooltip("Актор контроллер. Прямое движение к позиции")]
  public class MoveTarget : MovePosition
  {
	 public FsmEvent OnComplete;
	 public FsmGameObject _targetObject;

	 public override void OnEnter()
	 {
		_targetPosition = _targetObject.Value.transform.position;

		base.OnEnter();
	 }

	 protected override void OnArrived()
	 {
		Fsm.Event(OnComplete);
	 }

  }
}