using UnityEngine;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using HutongGames.PlayMaker;
using DG.Tweening;
using RootMotion.FinalIK;
using com.ootii.Geometry;
using it.Game.Player;
using it.Game.PlayMaker.ActorController;

namespace it.Game.PlayMaker.Boses.Gishtil
{
  [ActionCategory("Enemyes")]
  [Tooltip("Атака рукой")]
  public class GishtilHandAttack : it.Game.PlayMaker.ActorController.AnimatorDriver
  {
	 private readonly float attackTime = 0.66f;
	 public FsmFloat _timeAttackComplete;

	 public FsmEvent OnComplete;

	 private int _animTargetState = 2;
	 private float _startTime = 0;
	 public override void Reset()
	 {
		_timeAttackComplete = null;
		OnComplete = null;
	 }

	 public override void OnEnter()
	 {
		base.OnEnter();
		_animator.SetInteger("State", _animTargetState);
		_startTime = Time.timeSinceLevelLoad;
	 }

	 public override void OnUpdate()
	 {
		if ((_startTime + attackTime) <= Time.timeSinceLevelLoad)
		  Fsm.Event(OnComplete);

	 }

	 public override void OnExit()
	 {
		base.OnExit();
		_timeAttackComplete.Value = Time.timeSinceLevelLoad;
		_animator.SetInteger("State", 0);
	 }
  }
}