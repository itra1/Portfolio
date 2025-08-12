using UnityEngine;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using HutongGames.PlayMaker;
using DG.Tweening;

namespace it.Game.PlayMaker.Boses.Gishtil
{
  [ActionCategory("Enemyes")]
  [Tooltip("Атака тьмой")]
  public class GishtilShadowCast : FsmStateAction
  {

	 public FsmGameObject shadowCastObject;
	 private float attackTime = 2.1f;
	 private float timeActive = 20;
	 public FsmFloat timeComplete;

	 public FsmEvent OnComplete;

	 private int _animTargetState = 5;

	 [SerializeField]
	 private it.Game.Environment.Level6.Gishtil.GishtilShadowCaster _shadowCast;
	 private Animator _animator;
	 private it.Game.NPC.Enemyes.Boses.Hunter.Gishtil _enemy;
	 public override void Reset()
	 {

	 }
	 public override void Awake()
	 {
		base.Awake();
		_animator = Owner.GetComponent<Animator>();
	 }
	 public override void OnEnter()
	 {
		base.OnEnter();
		_shadowCast = Owner.GetComponent<it.Game.Environment.Level6.Gishtil.GishtilShadowCaster>();
		_animator.SetInteger("State", _animTargetState);

		DOVirtual.DelayedCall(0.7f, () =>
		{
		  _shadowCast.Activate(timeActive, _enemy);
		});

		DOVirtual.DelayedCall(3, () =>
		{
		  Fsm.Event(OnComplete);
		});
	 }

	 public override void OnExit()
	 {
		timeComplete.Value = Time.timeSinceLevelLoad;
		_animator.SetInteger("State", 0);
	 }

  }
}