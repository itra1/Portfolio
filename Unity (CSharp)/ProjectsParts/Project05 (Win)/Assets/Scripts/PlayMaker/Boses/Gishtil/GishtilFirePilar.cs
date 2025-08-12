using UnityEngine;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.Boses.Gishtil
{
  [ActionCategory("Enemyes")]
  [Tooltip("Столб огня")]
  public class GishtilFirePilar : FsmStateAction
  {
	 public FsmGameObject target;
	 public FsmGameObject bullet;
	 public FsmFloat attackTime = 3;

	 public FsmEvent OnComplete;

	 private it.Game.Environment.Level6.Gishtil.GishtilRoundBullet _roundBullet;
	 private int _animTargetState = 4;
	 private float _startTime = 0;
	 private bool _isCast;
	 private Animator _animator;

	 public override void Reset()
	 {
		attackTime = 3;
	 }

	 public override void OnEnter()
	 {
		base.OnEnter();
		_animator = Owner.GetComponent<Animator>();
		_roundBullet = bullet.Value.GetComponent<Environment.Level6.Gishtil.GishtilRoundBullet>();

		_animator.SetInteger("State", _animTargetState);
		_startTime = Time.time;
		_isCast = false;
	 }

	 public override void OnUpdate()
	 {
		if ((_startTime + attackTime.Value) <= Time.time)
		{
		  Fsm.Event(OnComplete);
		}

		if (!_isCast && (_startTime + 0.5f) <= Time.time)
		{
		  _isCast = true;
		  _roundBullet.Cast(target.Value.transform.position);
		}

	 }

	 public override void OnExit()
	 {
		base.OnExit();
		_animator.SetInteger("State", 0);
	 }

  }
}