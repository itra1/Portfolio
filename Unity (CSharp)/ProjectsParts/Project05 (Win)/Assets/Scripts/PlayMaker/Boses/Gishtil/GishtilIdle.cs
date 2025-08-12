using UnityEngine;
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Boses.Gishtil
{
  [ActionCategory("Gishtil")]
  [Tooltip("Бездействие")]
  public class GishtilIdle : it.Game.PlayMaker.ActorController.ActorDriver
  {
	 private Animator _animator;

	 public override void OnPreprocess()
	 {
		//Fsm.HandleAnimatorMove = true;
	 }

	 public override void OnEnter()
	 {
		base.OnEnter();

		_animator = _go.GetComponent<Animator>();
	 }

	 public override void OnUpdate()
	 {
	 }

	 public virtual void OnAnimatorMove()
	 {
		//_go.transform.TransformVector(_animator.deltaPosition);
		//_go.transform.position += _go.transform.TransformVector(_animator.deltaPosition);
		//_actor.Rotate(_animator.deltaRotation);
		//_actor.Move(-_animator.deltaPosition);
	 }

	 public override void DoAnimatorMove()
	 {
		OnAnimatorMove();
	 }

  }
}