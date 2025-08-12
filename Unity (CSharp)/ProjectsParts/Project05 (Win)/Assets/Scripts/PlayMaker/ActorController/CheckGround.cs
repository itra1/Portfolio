using UnityEngine;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  [HutongGames.PlayMaker.Tooltip("Проверка что на контакт с землей")]
  public class CheckGround : ActorBase
  {
	 public FsmEvent isTrue;
	 public FsmEvent isFalse;

	 public FsmBool everyUpdate;
	 public override void OnEnter()
	 {
		if (!everyUpdate.Value)
		{
		  if (_actor.IsGrounded)
			 Fsm.Event(isTrue);
		  else
			 Fsm.Event(isFalse);
		}
	 }

	 public override void OnUpdate()
	 {
		if (everyUpdate.Value)
		{
		  if (_actor.IsGrounded)
			 Fsm.Event(isTrue);
		  else
			 Fsm.Event(isFalse);
		}
	 }


  }
}