using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using DG.Tweening;
using com.ootii.Actors;
using UnityEngine.AI;

namespace it.Game.PlayMaker.Boses
{
  [ActionCategory("Enemyes")]
  [Note("Атака 2: Атака ногой")]
  public class PriestAttack2 : FsmStateAction
  {
	 /*
	  * 0 - приседяет
	  * 1 - поднимается вверх
	  * 2 - тормозит
	  * 3 - двигается вниз
	  */

	 public FsmEvent OnComplete;
	 public FsmOwnerDefault gameObject;
	 public FsmGameObject target;
	 public FsmFloat speedUp = 10;
	 public FsmFloat speedForvard = 10;

	 private GameObject _go;
	 private Animator _animator;
	 private com.ootii.Actors.ActorController _actor;
	 private int _state = 0;
	 private Vector3 _velocity;
	 private Vector3 _startPositionState;
	 private Vector3 _currentTargetPosition;
	 private Vector3 _upPosition;
	 private float _time;


	 public override void Awake()
	 {
		base.Awake();
		HandlesOnEvent = true;
	 }

	 public override void OnEnter()
	 {
		base.OnEnter();

		if(_go == null)
		{
		  _go = Fsm.GetOwnerDefaultTarget(gameObject);
		  _actor = _go.GetComponent<com.ootii.Actors.ActorController>();
		  _animator = _go.GetComponent<Animator>();
		}

		_state = 0;
		//_actor.IsGravityEnabled = false;
		//_actor.ForceGrounding = false;
		_animator.SetInteger("State", 200);

	 }

	 public override bool Event(FsmEvent fsmEvent)
	 {
		if (fsmEvent.Name.Equals("OnJump"))
		{
		  if (_state == 0)
		  {
			 _actor.IsGravityEnabled = false;
			 _actor.ForceGrounding = false;
			 _state = 1;
			 _startPositionState = _actor._Transform.position;
			 _velocity = _actor._Transform.up * speedUp.Value;
		  }
		}

		return true;
	 }

	 public override void OnUpdate()
	 {
		if (_state == 1)
		{
		  _velocity.y += -10 * Time.deltaTime;

		  if (_velocity.y <= 0.5f)
		  {
			 _state = 2;
			 _animator.SetInteger("State", 201);
		  }

		}

		if (_state == 2)
		{
		  _velocity.y += -10 * Time.deltaTime;
		  if (_velocity.y <= 0f)
		  {
			 _state = 3;
			 _currentTargetPosition = target.Value.transform.position + Vector3.up*.5f;
			 _upPosition = _actor._Transform.position;
		  }
		}
		if (_state == 3)
		{

		  _velocity = (_currentTargetPosition - _upPosition).normalized * speedForvard.Value;

		  if (_actor.IsGrounded)
		  {
			 _state = 4;
			 _actor.IsGravityEnabled = true;
			 _actor.ForceGrounding = true;
			 _animator.SetInteger("State", 202);
			 _velocity = Vector3.zero;
			 _time = Time.time;
		  }
		}
		if (_state == 4 && _time+1 < Time.time)
		{
		  Fsm.Event(OnComplete);
		  return;
		}


		  if (_state > 0)
		  _actor.SetVelocity(_velocity);
	 }

  }
}