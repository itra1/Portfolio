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
  [Tooltip("Полет вверх")]
  public class TheKarmamancerFlyUp : it.Game.PlayMaker.ActorController.ActorDriver
  {
	 public FsmFloat _height = 3;
	 public FsmAnimationCurve _curveMove;

	 public FsmEvent OnComplete;
	 public FsmEvent OnFailed;

	 private Animator _animator;
	 private Vector3 _startPostion;
	 private Vector3 _targetMove;
	 private float _distance;
	 private float _timeStart;
	 private int _state;

	 public override void OnEnter()
	 {
		base.OnEnter();

		_actor.IsGravityEnabled = false;
		_actor.ForceGrounding = false;
		_actor.FixGroundPenetration = false;

		_animator = _go.GetComponent<Animator>();
		_startPostion = _go.transform.position;
		_targetMove = _startPostion + Vector3.up * _height.Value;
		_distance = (_targetMove - _startPostion).magnitude;
		_timeStart = 0;
		_state = 0;
	 }

	 public override void Reset()
	 {
		_height = null;
		_curveMove = null;
		movementSpeed = 1;
	 }

	 public override void OnUpdate()
	 {
		if (_state == 0)
		{
		  bool readyMove = CheckMoveReady(_height.Value);
		  if (!readyMove)
			 Fsm.Event(OnFailed);
		  _state = 1;
		  _animator.SetInteger("State", 3);
		  _animator.SetInteger("SubState", 0);
		}
		else
		{
		  if (_state == 1)
		  {
			 _animator.SetInteger("SubState", 1);
		  }
		}
		_timeStart += Time.deltaTime * movementSpeed.Value; ;

		float perc = _curveMove.curve.Evaluate(_timeStart);

		_actor.Move((_startPostion + Vector3.up * _distance * perc) - _go.transform.position);

		if (perc == 1)
		  Fsm.Event(OnComplete);

	 }

	 public bool CheckMoveReady(float height)
	 {

		Vector3 startPos = height > 0 ? (_go.transform.position + Vector3.up * _actor.Height) : _go.transform.position;
		RaycastHit hit;
		if (RaycastExt.SafeSphereCast(startPos, Vector3.up * Mathf.Sign(height), _actor.Radius, out hit, height, -1, _go.transform))
		{
		  Debug.Log(hit.collider.gameObject.name);
		  return false;
		}
		return true;
	 }
  }
}