using System.Collections;

using HutongGames.PlayMaker;

using UnityEngine;

namespace it.Game.PlayMaker.ActorController
{
  public class DoAnimatorMoveForvard : FsmStateAction
  {
	 public FsmEventTarget forwardTo;
	 public override void DoAnimatorMove()
	 {
		base.DoAnimatorMove();
		Fsm.Event(forwardTo, "DoAnimatorMove");
	 }
  }
}