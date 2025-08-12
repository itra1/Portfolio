using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using com.ootii.Geometry;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using UnityEngine.AI;

namespace it.Game.PlayMaker.Boses.TheKarmamancer
{
  /// <summary>
  /// Большой каст
  /// </summary>
  [ActionCategory("Enemyes")]
  [Tooltip("Опускается н поверхность")]
  public class TheKarmamancerMoveDownOnGround : it.Game.PlayMaker.ActorController.ActorDriver
  {
	 public FsmFloat _heightCheck = 10f;
	 public FsmFloat _stopDistance = 0.3f;
	 public FsmEvent OnComplete;

	 private Vector3 _targetPostition;

	 private bool _setGround = false;

	 public override void Reset()
	 {
		_heightCheck = 10;
		_stopDistance = 0.3f;
	 }

	 public override void OnEnter()
	 {
		base.OnEnter();
		_setGround = false;
		FindTargetPosition();

		Animator _animator = Owner.GetComponent<Animator>();

		_animator.SetInteger("State", 3);
		_animator.SetInteger("SubState", 2);
	 }

	 private void FindTargetPosition()
	 {

		RaycastHit _hit;
		if (RaycastExt.SafeSphereCast(Owner.transform.position, Vector3.down, _actor.Radius, out _hit, _heightCheck.Value, ProjectSettings.GroundLayerMaks, Owner.transform))
		{
		  _targetPostition = _hit.point;
		}
		else
		{
		  _actor.IsGravityEnabled = true;
		  _actor.ForceGrounding = true;
		  _actor.FixGroundPenetration = true;

		  _setGround = true;
		}

	 }

	 public override void OnUpdate()
	 {
		if (_setGround)
		  OnGround();

		if ((_targetPostition - Owner.transform.position).magnitude <= _stopDistance.Value)
		  OnGround();

		_actor.Move(Vector3.down * movementSpeed.Value * Time.deltaTime);
	 }

	 private void OnGround()
	 {
		_actor.IsGravityEnabled = true;
		_actor.ForceGrounding = true;
		_actor.FixGroundPenetration = true;
		Fsm.Event(OnComplete);
	 }

	 public override void OnExit()
	 {
		base.OnExit();
		_actor.IsGravityEnabled = true;
		_actor.ForceGrounding = true;
		_actor.FixGroundPenetration = true;
	 }

  }
}