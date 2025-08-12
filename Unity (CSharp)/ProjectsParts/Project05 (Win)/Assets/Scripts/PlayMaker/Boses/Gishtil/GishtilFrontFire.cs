using UnityEngine;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using RootMotion.FinalIK;
using HutongGames.PlayMaker;
using it.Game.PlayMaker.ActorController;

namespace it.Game.PlayMaker.Boses.Gishtil
{
  [ActionCategory("Enemyes")]
  [Tooltip("Фронтальный конус огнем")]
  public class GishtilFrontFire : ActorDriver
  {
	 public FsmGameObject _player;
	 public FsmFloat attackTime = 3;
	 public FsmGameObject spawnPoint;
	 public FsmGameObject _bullet;
	 public AnimationCurve _changeEmission;
	 public FsmFloat _timeComplete;

	 public FsmEvent OnComplete;

	 private float _startTime = 0;
	 private int _animTargetState = 3;

	 private it.Game.Environment.Level6.Gishtil.GishtilFrontFire fireBullet;

	 private AimIK _aimIK;
	 private Transform _aimTarget;

	 private Animator _animator;

	 public override void Awake()
	 {
		base.Awake();
		Inicialization();
		if(_animator == null)
		_animator = _go.GetComponent<Animator>();
		if (_aimIK == null)
		  _aimIK = _go.GetComponent<AimIK>();
		if (_aimTarget == null)
		  _aimTarget = _aimIK.solver.target;
		if (fireBullet == null)
		  fireBullet = _bullet.Value.GetComponent<it.Game.Environment.Level6.Gishtil.GishtilFrontFire>();
	 }

	 public override void Reset()
	 {
		attackTime = 3;
	 }

	 public override void OnEnter()
	 {
		base.OnEnter();

		_aimIK.solver.IKPositionWeight = 1;
		_animator.SetInteger("State", _animTargetState);
		_startTime = Time.time;

		fireBullet.Activate();

	 }

	 private void PositingIK()
	 {
		float distance = (_player.Value.transform.position - _go.transform.position).magnitude;

		Vector3 targetPoint = _go.transform.position + _go.transform.forward * distance;
		targetPoint.y = _player.Value.transform.position.y + 1;

		_aimTarget.position = targetPoint;

	 }

	 public override void OnUpdate()
	 {
		PositingIK();
		_bullet.Value.transform.position = spawnPoint.Value.transform.position;
		_bullet.Value.transform.LookAt(_aimTarget.position);

		if ((_startTime + attackTime.Value) <= Time.time)
		{
		  fireBullet.Deactivate();
		  Fsm.Event(OnComplete);
		}

		// Вращение по направлению игроку
		Rotation();
	 }

	 public override void OnLateUpdate()
	 {
		base.OnLateUpdate();
	 }

	 //public override void OnLateUpdate()
	 //{
	 //PositingIK();
	 //_bullet.Value.transform.position = spawnPoint.Value.transform.position;
	 //_bullet.Value.transform.LookAt(_aimTarget.position);
	 //}

	 private void Rotation()
	 {

		Quaternion lRotation = Quaternion.identity;
		CalcRotation(_player.Value.transform.position, rotationSpeed.Value, ref lRotation);
		_actor.Rotate(lRotation);
	 }

	 public override void OnExit()
	 {
		base.OnExit();
		_aimIK.solver.IKPositionWeight = 0;
		_animator.SetInteger("State", 0);
		_timeComplete.Value = Time.timeSinceLevelLoad;
	 }

  }
}