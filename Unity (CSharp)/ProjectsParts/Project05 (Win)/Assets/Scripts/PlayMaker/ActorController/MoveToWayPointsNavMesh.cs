using UnityEngine;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory(ActionCategory.Character)]
  [HutongGames.PlayMaker.Tooltip("Актор контроллер. Движение по маршруту с использование навмеша")]

  public class MoveToWayPointsNavMesh : MoveNawMesh
  {
	 [ArrayEditor(VariableType.GameObject)]
	 public FsmArray _wayPoints;
	 private int _index;
	 public FsmEvent onMoveComplete;

	 public override void OnEnter()
	 {
		base.OnEnter();

		_index = -1;
		OnArrived();

	 }

	 protected override void OnArrived()
	 {
		_index++;
		if (_index >= _wayPoints.Length)
		  Fsm.Event(onMoveComplete);
		else
		{

		  TargetPosition = ((GameObject)_wayPoints.Values[_index]).transform.position;
		}
	 }

  }

}