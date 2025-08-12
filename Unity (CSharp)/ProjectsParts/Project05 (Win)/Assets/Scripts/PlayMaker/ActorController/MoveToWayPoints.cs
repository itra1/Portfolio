using UnityEngine;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory(ActionCategory.Character)]
  [HutongGames.PlayMaker.Tooltip("Актор контроллер. Перемещение по ключевыс точкам")]
  public class MoveToWayPoints : MovePosition
  {
	 [ArrayEditor(VariableType.GameObject)]
	 public FsmArray _wayPoints;
	 private int _index;
	 public FsmEvent onMoveComplete;

	 public FsmBool _loop;

	 public override void OnEnter()
	 {
		base.OnEnter();

		_index = -1;
		OnArrived();

	 }

	 protected override void OnArrived()
	 {
		_index++;

		if (_loop.Value && _index >= _wayPoints.Length)
		  _index = 0;

		if (_index >= _wayPoints.Length)
		  Fsm.Event(onMoveComplete);
		else
		{

		  TargetPosition = ((GameObject)_wayPoints.Values[_index]).transform.position;
		}
	 }

  }
}