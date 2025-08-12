using UnityEngine;
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
  [Tooltip("Скольжение в сторону")]
  public class TheKarmamancerGlide : it.Game.PlayMaker.ActorController.ActorDriver
  {
	 public FsmGameObject target;
	 public FsmFloat _stopDistance = 0.2f;
	 public FsmFloat _minHeightCheck = 0.3f;
	 public FsmBool _backOnly;

	 public FsmFloat _minDistance = 2;
	 public FsmFloat _maxDistance = 6;

	 public FsmEvent OnComplete;
	 public FsmEvent OnFailed;

	 private Animator _animator;
	 private Vector3 _targetMove;
	 private Vector3 _startPosition;
	 private bool existsVariantMove;
	 private VectorTarget _vTarget;
	 private Vector3 _direction;
	 private float _distance;

	 private float _state;


	 private enum VectorTarget
	 {
		back = 1,
		left = 2,
		right = 3
	 }

	 public override void Inicialization()
	 {
		base.Inicialization();
		_animator = _go.GetComponent<Animator>();
	 }

	 public override void OnEnter()
	 {
		base.OnEnter();
		existsVariantMove = CalcGlidePosition();

		if (existsVariantMove)
		{
		  _animator.SetInteger("State", 1);
		  _animator.SetInteger("SubState", (int)_vTarget);
		}

		_startPosition = _go.transform.position;
		_distance = (_targetMove - _startPosition).magnitude;
		_direction = (_targetMove - _go.transform.position).normalized;

		_state = 1;
	 }
	 public override void OnUpdate()
	 {
		if (!existsVariantMove)
		  Fsm.Event(OnFailed);

		float perc = _animator.GetFloat("crv_move");

		if ((_targetMove - _go.transform.position).magnitude < _stopDistance.Value)
		  Fsm.Event(OnComplete);


		Move(perc);
	 }

	 private void Move(float percent)
	 {

		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;
		CalcMove(percent, ref lMovement);
		//CalculateMove(_targetMove, ref lMovement, ref lRotation);
		_actor.Move(lMovement);
	 }

	 private void CalcMove(float percent, ref Vector3 rMove)
	 {
		Vector3 currentPosition = _go.transform.position;
		Vector3 newPosition = _startPosition + _direction * _distance * percent;
		rMove = newPosition - currentPosition;
	 }

	 private bool CalcGlidePosition()
	 {
		float minDistance = _minDistance.Value;
		float maxDistance = _maxDistance.Value;
		for (int i = 0; i < 20; i++)
		{
		  maxDistance = _maxDistance.Value;
		  Vector3 targetVector = GetRandomVector();

		  // Стреляем на максимальное расстоние

		  Vector3 startCheckPoint = _go.transform.position + Vector3.up * _minHeightCheck.Value;

		  RaycastHit _hit;
		  if (RaycastExt.SafeRaycast(startCheckPoint, targetVector, out _hit, maxDistance, -1, _go.transform))
		  {
			 var newDistance = (_hit.point - startCheckPoint).magnitude;
			 if (newDistance < minDistance)
				continue;

			 maxDistance = newDistance;
		  }
		  float dist = Random.Range(minDistance, maxDistance);
		  _targetMove = _go.transform.position + targetVector * dist;
		  return true;

		}
		return false;

	 }

	 private Vector3 GetRandomVector()
	 {
		_vTarget = (VectorTarget)Random.Range(1, 4);

		Vector3 checkVector = Vector3.zero;

		switch (_vTarget)
		{
		  case VectorTarget.back:
			 checkVector = -_go.transform.forward;
			 break;
		  case VectorTarget.left:
			 checkVector = -_go.transform.right;
			 break;
		  case VectorTarget.right:
			 checkVector = _go.transform.right;
			 break;
		}
		if (_backOnly.Value)
		{
		  _vTarget = VectorTarget.back;
		  checkVector = -_go.transform.forward;
		}

		return checkVector;

	 }

  }
}