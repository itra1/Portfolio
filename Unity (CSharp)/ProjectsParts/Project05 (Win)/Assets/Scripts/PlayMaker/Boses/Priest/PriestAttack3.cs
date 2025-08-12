using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using DG.Tweening;

namespace it.Game.PlayMaker.Boses
{
  /// <summary>
  /// Атака сверху
  /// </summary>
  [ActionCategory("Enemyes")]
  [Note("Атака 3: атака сверху")]
  public class PriestAttack3 : FsmStateAction
  {
	 public FsmOwnerDefault gameObject;
	 public FsmGameObject target;
	 public FsmGameObject bullet;

	 public FsmEvent OnComplete;

	 private GameObject _go;
	 private Animator _animator;

	 public override void OnEnter()
	 {
		base.OnEnter();

		if(_go == null)
		{
		  _go = Fsm.GetOwnerDefaultTarget(gameObject);
		  _animator = _go.GetComponent<Animator>();
		}

		_animator.SetInteger("State", 300);

		DOVirtual.DelayedCall(0.7f, Attack);

	 }

	 public void Attack()
	 {
		bullet.Value.GetComponent<it.Game.Environment.Level5.PriestArena.PriestLaserBullet>().Shoot();

		DOVirtual.DelayedCall(10f, ()=> {
		  _animator.SetInteger("State", 0);
		  Fsm.Event(OnComplete);
		});
	 }

  }
}