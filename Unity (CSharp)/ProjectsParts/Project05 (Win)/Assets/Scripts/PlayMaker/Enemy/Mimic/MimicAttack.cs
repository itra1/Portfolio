using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using DG.Tweening;

namespace it.Game.PlayMaker.Enemy.Mimic
{
  public class MimicAttack : FsmStateAction
  {
	 public FsmOwnerDefault _gameObject;
	 public FsmGameObject bullet;
	 public FsmGameObject spawnPoint;
	 private GameObject _go;
	 private Animator _animator;

	 private float _attackPeriod = 2;
	 private float _lastTimeAttack;

	 private void Initiate()
	 {
		_go = Fsm.GetOwnerDefaultTarget(_gameObject);
		_animator = _go.GetComponentInChildren<Animator>();
	 }

	 public override void OnEnter()
	 {
		if (_go == null)
		  Initiate();
	 }

	 public override void OnUpdate()
	 {
		if (_lastTimeAttack + _attackPeriod > Time.timeSinceLevelLoad) return;
		_lastTimeAttack = Time.timeSinceLevelLoad;
		_animator.SetTrigger("Attack");
		DOVirtual.DelayedCall(0.5f, () => {
		  OnAttack();
		});
	 }

	 private void Shoot()
	 {
		bullet.Value.GetComponent<it.Game.NPC.Enemyes.MimicBullet>().Shoot(spawnPoint.Value.transform, Owner.transform);
	 }

	 public void OnAttack()
	 {
		Shoot();
	 }

  }
}