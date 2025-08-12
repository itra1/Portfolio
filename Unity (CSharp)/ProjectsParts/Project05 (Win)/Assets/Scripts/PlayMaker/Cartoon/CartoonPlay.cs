using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker
{
  [ActionCategory("Cartoon")]
  public class CartoonPlay : FsmStateAction
  {
	 [RequiredField] public FsmString uuid;

	 public FsmEvent onComplete;
	 public FsmBool fideIn = true;
	 public FsmBool fideOut = true;

	 public override void OnEnter()
	 {
		it.Cartoons.Cartoon.Play(uuid.Value, fideIn.Value, fideOut.Value, () =>
		{
		  Fsm.Event(onComplete);
		});
	 }
  }
}