using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using it.Game.NPC.Enemyes;
using DG.Tweening;
using UniRx;

namespace it.Game.PlayMaker.Enemy.Golem
{
  [ActionCategory("Enemy")]
  public class GolemShoot : FsmStateAction
  {
	 public FsmEvent OnComplete;
	 public FsmOwnerDefault _gameObject;
	 public FsmGameObject SpawnBulletPosition;
	 public FsmGameObject BulletPrefab;

	 private bool _bulletHold;
	 private GameObject _go;
	 private Animator _animator;
	 private Transform _bulletSpawnPosition;
	 private LavaGolemBullet _bullet;
	 
	 public override void Awake()
	 {
		base.Awake();
		base.HandlesOnEvent = true;
		_bulletSpawnPosition = SpawnBulletPosition.Value.transform;
	 }

	 public override void OnEnter()
	 {
		if(_go == null)
		  InitCompioneny();

		Shoot();
	 }

	 private void InitCompioneny()
	 {
		_go = Fsm.GetOwnerDefaultTarget(_gameObject);
		_animator = _go.GetComponent<Animator>();
	 }

	 public override bool Event(FsmEvent fsmEvent)
	 {
		if(fsmEvent.Name == "BulletSpawnStart")
		{
		  _bulletHold = true;
		  SpanBullet();
		}
		if (fsmEvent.Name == "BulletSpawn")
		{
		  _bulletHold = false;
		  _bullet.transform.rotation = Quaternion.LookRotation(Player.PlayerBehaviour.Instance.transform.position -_go.transform.position,Vector3.up);
		  _bullet.Shoot();
		  Observable.Timer(System.TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
		  {
			 Fsm.Event(OnComplete);
		  });
		}

		return base.Event(fsmEvent);
	 }

	 private void SpanBullet()
	 {
		GameObject inst = MonoBehaviour.Instantiate(BulletPrefab.Value);
		_bullet = inst.GetComponent<LavaGolemBullet>();
	 }

	 public override void OnUpdate()
	 {
		base.OnUpdate();

		if (_bulletHold)
		{
		  _bullet.transform.position = _bulletSpawnPosition.position;
		}
	 }

	 private void Shoot()
	 {
		_animator.SetTrigger("DoublePunch");
	 }
  }
}