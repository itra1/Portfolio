using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using com.ootii.Geometry;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Boses.MultiHead
{
  public class MultiheadHorizontalMove : it.Game.PlayMaker.ActorController.HorisontalMoveTarget
  {
	 private Animator _animator;

	 public override void OnEnter()
	 {
		base.OnEnter();
		_animator = _go.GetComponentInChildren<Animator>();
		_animator.SetInteger("State", 1);
	 }

	 public override void OnExit()
	 {
		base.OnExit();
		_animator.SetInteger("State", 0);
	 }

  }

}