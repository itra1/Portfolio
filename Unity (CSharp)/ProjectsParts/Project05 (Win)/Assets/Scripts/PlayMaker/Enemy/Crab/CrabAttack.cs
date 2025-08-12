using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using DG.Tweening;

namespace it.Game.PlayMaker.Enemy.Crab
{
  [ActionCategory("Enemy")]
  public class CrabAttack : FsmStateAction
  {
	 public FsmOwnerDefault _gameObject;
	 public FsmEvent OnComplete;
	 private GameObject _go;
	 private Animator _animator;
	 private Collider _playerDamageCollider;

	 public override void Awake()
	 {
		if (!Application.isPlaying)
		  return;

		_go = Fsm.GetOwnerDefaultTarget(_gameObject);
		_animator = _go.GetComponent<Animator>();
		_playerDamageCollider = _go.transform.GetComponentInChildren<it.Game.Player.Handlers.TriggerPlayerDamage>().GetComponent<Collider>();
		_playerDamageCollider.enabled = false;
	 }

	 public override void OnEnter()
	 {
		_animator.SetTrigger("Attack");

		DOVirtual.DelayedCall(0.25f, () =>
		{
		  _playerDamageCollider.enabled = true;
		  DOVirtual.DelayedCall(0.1f, () =>
		  {
			 _playerDamageCollider.enabled = false;
		  });
		});
		DOVirtual.DelayedCall(0.5f, () =>
		{
		  Fsm.Event(OnComplete);
		});
	 }

  }
}