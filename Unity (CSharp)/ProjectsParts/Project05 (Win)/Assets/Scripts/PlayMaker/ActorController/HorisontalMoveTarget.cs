using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Timing;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.ActorController
{
  public class HorisontalMoveTarget : HorisontalMovePosition
  {
	 public FsmGameObject _targetObject;

	 public override void OnUpdate()
	 {
		TargetPosition = _targetObject.Value.transform.position;

		base.OnUpdate();
	 }

  }
}