using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
namespace it.Game.PlayMaker.ActorController
{

  [ActionCategory("Actor Controller")]
  public class SetGravity : FsmStateAction
  {
	 public FsmOwnerDefault _owner;
	 public FsmBool value;

	 public override void OnEnter()
	 {
		GameObject go = Fsm.GetOwnerDefaultTarget(_owner);
		go.GetComponent<com.ootii.Actors.ActorController>().IsGravityEnabled = value.Value;
		Finish();
	 }
  }
}