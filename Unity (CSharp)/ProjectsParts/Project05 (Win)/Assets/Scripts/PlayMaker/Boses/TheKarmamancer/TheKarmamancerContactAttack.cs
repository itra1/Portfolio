using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using com.ootii.Geometry;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Boses.TheKarmamancer
{
  /// <summary>
  /// Большой каст
  /// </summary>
  [ActionCategory("Enemyes")]
  [Tooltip("Контактная атака")]
  public class TheKarmamancerContactAttack : it.Game.PlayMaker.ActorController.ActorDriver
  {

	 /// <summary>
	 /// Дистанция атаки
	 /// </summary>
	 private float _attackDistanceHand = 2f;
	 private float _attackDistanceKluka = 1.8f;
	 //private float _attackDistanceHand = 2f;
	 //private float _attackDistanceKluka = 1.83f;

	 /// <summary>
	 /// Время анимации атаки рукой
	 /// </summary>
	 private float _handAttack = 1.10f;

	 /// <summary>
	 /// Время атаки клюкой
	 /// </summary>
	 private float _klukaAttack = 0.5f;

	 public FsmGameObject _target;
	 public FsmFloat _stopDistance = 0.2f;
	 public bool _handOnly = false;

	 public FsmEvent OnComplete;

	 private Animator _animator;

	 private float _timeStart;
	 private float _fullTime = 4;
	 private float _fullTimeHand = 2;

	 private Vector3 _targetMove;

	 private int _state;
	 //private float Speed => DistanceAttack / _timeAttack;
	 //private float DistanceAttack	=> (_target.Value.transform.position - _go.transform.position).magnitude - _attackDistance;

	 private float _currentTimeAttack = 0;

	 private float _attackTime;
	 private RootMotion.FinalIK.AimIK _aimIk;
	 private Transform _aiming;

	 private float _startSpeed;

	 private it.Game.NPC.Enemyes.Boses.Karmamancer.TheKarmamancer _karmamancer;

	 public override void Inicialization()
	 {
		base.Inicialization();
		_animator = _go.GetComponent<Animator>();
		_karmamancer = _go.GetComponent<it.Game.NPC.Enemyes.Boses.Karmamancer.TheKarmamancer>();
	 }

	 public override void OnEnter()
	 {
		base.OnEnter();

		_aimIk = _go.GetComponentInChildren<RootMotion.FinalIK.AimIK>();
		_aiming = _go.GetComponentInChildren<RootMotion.Demos.AimBoxing>().transform;

		int substate = _handOnly ? 1 : Random.Range(1, 3);

		float distanceDamage = substate == 1 ? 1.3f : 0.7f;
		_aimIk.solver.IKPositionWeight = substate == 1 ? 1f : 0.8f;
		_attackTime = substate == 1 ? _attackDistanceHand : _attackDistanceKluka;

		_animator.SetInteger("State", 2);
		_animator.SetInteger("SubState", substate);

		float distance = (_target.Value.transform.position - _go.transform.position).magnitude;
		Vector3 vectorMove = (_target.Value.transform.position - _go.transform.position).normalized;
		_targetMove = _go.transform.position + vectorMove * (distance - distanceDamage);

		_startSpeed = (distance - distanceDamage) / _handAttack;
		movementSpeed.Value = _startSpeed;

		_karmamancer.AttackReady(true);

		//_targetMove = _go.transform.position
		//+ (_target.Value.transform.position - _go.transform.position).normalized * DistanceAttack;
		//movementSpeed.Value = Speed;
		_timeStart = Time.time;
		_state = 1;
	 }
	 public override void OnExit()
	 {
		base.OnExit();
		_karmamancer.AttackReady(false);
		_aimIk.solver.IKPositionWeight = 0;
		_animator.SetInteger("State", 0);
		_animator.SetInteger("SubState", 0);
	 }
	 public override void OnUpdate()
	 {
		base.OnUpdate();
		if (_state == 1 && (_targetMove - _go.transform.position).magnitude < _stopDistance.Value)
		{
		  _state = 2;
		}

		if (_timeStart + _attackTime < Time.time)
		{
		  movementSpeed.Value = movementSpeed.Value - _startSpeed * 2 * Time.deltaTime;
		  if (movementSpeed.Value < 0)
			 movementSpeed.Value = 0;
		}

		if (_timeStart + 2 < Time.time)
		  Fsm.Event(OnComplete);

		if (_state == 1)
		{
		  Move();
		}
		if (_state == 2)
		{
		  Rotation();
		}

	 }

	 public override void OnLateUpdate()
	 {
		_aiming.transform.position = Player.PlayerBehaviour.Instance.HipBone.position;
	 }

	 private void Rotation()
	 {
		Quaternion lRotation = Quaternion.identity;
		CalcRotation(_target.Value.transform.position, rotationSpeed.Value, ref lRotation);
		_actor.Rotate(lRotation);
	 }

	 private void Move()
	 {

		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;
		CalcMove(_targetMove, movementSpeed.Value, ref lMovement, ref lRotation);
		//CalculateMove(_targetMove, ref lMovement, ref lRotation);
		_actor.Move(lMovement);
	 }
	 protected virtual void CalculateMove(Vector3 rWaypoint, ref Vector3 rMove, ref Quaternion rRotate)
	 {
		// Direction we need to travel in
		Vector3 lDirection = rWaypoint - _go.transform.position;
		lDirection.Normalize();

		// Determine our rotation
		Vector3 lVerticalDirection = Vector3.Project(lDirection, _go.transform.up);
		Vector3 lLateralDirection = lDirection - lVerticalDirection;

		//if (_rotateToTarget.Value)
		//{
		//  Vector3 lDirectionT = _targetObject.Value.transform.position - go.transform.position;
		//  lDirectionT.Normalize();

		//  // Determine our rotation
		//  Vector3 lVerticalDirectionT = Vector3.Project(lDirectionT, go.transform.up);
		//  lLateralDirection = lDirectionT - lVerticalDirectionT;

		//}

		//float lYawAngle = Vector3Ext.SignedAngle(go.transform.forward, lLateralDirection);

		//if (_rotate.Value)
		//{

		//  if (rotationSpeed.Value == 0f)
		//  {
		//	 rRotate = Quaternion.AngleAxis(lYawAngle, go.transform.up);
		//  }
		//  else
		//  {
		//	 rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), rotationSpeed.Value * lDeltaTime), go.transform.up);
		//  }
		//}


		//Set the final velocity based on the future rotation
		//Quaternion lFutureRotation = go.transform.rotation * rRotate;

		rMove = lDirection * (movementSpeed.Value * Time.deltaTime);

	 }


  }
}