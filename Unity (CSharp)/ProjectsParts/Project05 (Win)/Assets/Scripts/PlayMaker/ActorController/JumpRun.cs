using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine.AI;
using com.ootii.Actors;
using com.ootii.Geometry;
using com.ootii.Timing;
namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  [HutongGames.PlayMaker.Tooltip("Прыжок вперед")]
  public class JumpRun : MovePosition
  {
	 public FsmEvent OnComplete;


	 public override void OnEnter()
	 {
		base.OnEnter();

		_actor.AddImpulse(_go.transform.up * _jumpForce.Value);

	 }

	 public override void OnUpdate()
	 {
		base.OnUpdate();

		if (_actor.Velocity.y < 0 && _actor.IsGrounded)
		  Fsm.Event(OnComplete);

	 }

	 public override void OnExit()
	 {
		base.OnExit();
	 }
  }
}