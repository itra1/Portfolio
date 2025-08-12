using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.Boses
{
  /// <summary>
  /// Атака орбами
  /// </summary>
  [ActionCategory("Enemyes")]
  [Note("Атака 1: бросается энерго шарами")]
  public class PriestAttack1 : FsmStateAction
  {

	 public FsmOwnerDefault gameObject;
	 public FsmGameObject target;

	 public FsmEvent OnComplete;

	 public FsmGameObject bulletPrefab;
	 public FsmGameObject bulletStartPostiion;
	 public readonly int bulletCount = 2;

	 public readonly float _timeFirst = 1;
	 public readonly float _timeBefore = 1;
	 public readonly float _timeEnd = 1;

	 private GameObject _go;
	 private Animator _animator;

	 private float _timePhase;
	 private int _bulletIndex;

	 // 0 - Ожидаем начало бросков
	 // 1 - Броски
	 // 2 - финальное ожидание
	 private int _state = 0;

	 public override void OnEnter()
	 {
		base.OnEnter();

		if(_go == null)
		{
		  _go = Fsm.GetOwnerDefaultTarget(gameObject);
		  _animator = _go.GetComponent<Animator>();
		}

		_timePhase = Time.time;
		_state = 0;
		_bulletIndex = 0;
		_animator.SetInteger("State", 100);
	 }

	 public override void OnUpdate()
	 {
		// Начальное ожидание
		if (_state == 0)
		{
		  if (_timePhase + _timeFirst < Time.time)
		  {
			 _state = 1;
			 SpawnBullet();
		  }
		}

		// Атака шарами
		if (_state == 1)
		{
		  if (_timePhase + _timeBefore < Time.time)
		  {
			 SpawnBullet();
		  }
		  if (_bulletIndex >= bulletCount)
		  {
			 _timePhase = Time.time;
			 _state = 2;
		  }
		}
		// Окончание
		if (_state == 2)
		{
		  if (_timePhase + _timeEnd < Time.time)
		  {
			 Fsm.Event(OnComplete);
		  }
		}
	 }

	 public override void OnExit()
	 {
		base.OnExit();
		_animator.SetInteger("State", 0);
	 }

	 private void SpawnBullet()
	 {
		_timePhase = Time.time;
		_bulletIndex++;
		GameObject inst = MonoBehaviour.Instantiate(bulletPrefab.Value, bulletStartPostiion.Value.transform.position, Quaternion.identity);
		inst.transform.rotation = _go.transform.rotation;
		inst.gameObject.SetActive(true);
		var bullet = inst.GetComponent<it.Game.NPC.Enemyes.Boses.Priest.Attack1Bullet>();
		bullet.Shoot(_go.transform, target.Value.transform);
	 }

  }
}