using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine.AI;
using com.ootii.Actors;
using com.ootii.Geometry;
using com.ootii.Timing;

namespace it.Game.PlayMaker.AStar
{
  [ActionCategory("A* Pathfinding")]
  public class MoveTargetAStar : MoveAStar
  {
	 public FsmGameObject TargetObject;
	 public FsmVector3 TargetPoint;

	 public FsmEvent onMoveComplete;

	 public FsmBool everyUpdate;

	 public override void OnEnter()
	 {
		base.OnEnter();
		SetTarget();
	 }

	 public override void OnUpdate()
	 {
		base.OnUpdate();

		if (everyUpdate.Value)
		  SetTarget();
	 }

	 private void SetTarget()
	 {

		//if (TargetObject.Value != null && !TargetObject.Value.Equals(Owner))
		//  TargetPosition = TargetObject.Value.transform.position;
		//else
		//  TargetPosition = TargetPoint.Value;
	 }

	 //protected override void OnArrived()
	 //{
		//Fsm.Event(onMoveComplete);
	 //}
  }
}