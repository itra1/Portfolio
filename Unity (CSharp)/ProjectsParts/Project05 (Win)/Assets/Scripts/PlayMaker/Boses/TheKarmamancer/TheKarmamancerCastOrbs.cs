using UnityEngine;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.Boses
{
  [ActionCategory("Enemyes")]
  public class TheKarmamancerCastOrbs : FsmStateAction
  {
	 public FsmGameObject _target;

	 public FsmGameObject _bulletPrefab;
	 public FsmVector3 _bulletLocalPostiion = new Vector3(0, 2.5f, 0);

	 [SerializeField]
	 public FsmBool _teleportBackToPlayer = true;

	 public FsmEvent OnCompleteWithDamage;
	 public FsmEvent OnCompleteWithoutDamage;
	 private bool _existsHit;

	 private it.Game.NPC.Enemyes.Boses.Karmamancer.MiniBulletGroup _bulletGroup;

	 private Animator _animator;
	 private float _timeSpawn;
	 private float _timeCast;
	 private int _countBullet = -1;
	 private bool _isComplete;
	 public override void Awake()
	 {
		base.Awake();
		_animator = Owner.GetComponent<Animator>();
	 }
	 public override void OnEnter()
	 {
		base.OnEnter();

		CreateInstance();
		_timeSpawn = Time.time + _bulletGroup.FullRotate;
		_timeCast = -1;
		_countBullet = -1;
		_isComplete = false;
		_existsHit = false;
	 }

	 private void CreateInstance()
	 {

		GameObject inst = MonoBehaviour.Instantiate(_bulletPrefab.Value, Owner.transform);
		inst.transform.localPosition = _bulletLocalPostiion.Value;
		inst.transform.rotation = Owner.transform.rotation;
		inst.gameObject.SetActive(true);
		inst.transform.localScale = Vector3.one / 2;
		_bulletGroup = inst.GetComponent<it.Game.NPC.Enemyes.Boses.Karmamancer.MiniBulletGroup>();
		_bulletGroup.onComplete = (playerDamage) =>
		{
		  _existsHit = playerDamage;
		  _isComplete = true;
		};

	 }

	 public override void OnUpdate()
	 {
		if (_isComplete)
		{
		  if (_existsHit || !_teleportBackToPlayer.Value)
			 Fsm.Event(OnCompleteWithDamage);
		  else
			 Fsm.Event(OnCompleteWithoutDamage);
		}

		if (_timeSpawn <= Time.time && _countBullet <= 2)
		{
		  _countBullet++;

		  _timeSpawn = Time.time + _bulletGroup.TimeBeforeOrb;

		  if (_countBullet == 0)
			 _timeSpawn -= 0.5f;

		  if (_countBullet > 0)
		  {
			 _animator.SetTrigger("Attack");
			 _timeCast = Time.time + 0.5f;
		  }

		}

		if (_timeCast != -1 && _timeCast < Time.time)
		{
		  _bulletGroup.Shoot(Owner.transform, _target.Value.transform);

		  _timeCast = -1;
		}

	 }

  }
}