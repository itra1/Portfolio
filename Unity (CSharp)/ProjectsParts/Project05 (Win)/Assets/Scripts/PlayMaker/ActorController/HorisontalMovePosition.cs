using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Timing;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  [HutongGames.PlayMaker.Tooltip("Актор контроллер. Полет в заданную координату")]
  public class HorisontalMovePosition : ActorDriver
  {
	 public FsmEvent OnComplete;
	 protected Vector3 _targetPosition;
	 public FsmBool _ignoreY;

	 public FsmFloat _StopDistance = 0.1f;
	 public float StopDistance
	 {
		get { return _StopDistance.Value; }
		set { _StopDistance.Value = value; }
	 }

	 public FsmFloat _SlowDistance = 4.0f;
	 public float SlowDistance
	 {
		get { return _SlowDistance.Value; }
		set { _SlowDistance.Value = value; }
	 }

	 protected bool mIsInSlowDistance = false;
	 public bool IsInSlowDistance
	 {
		get { return IsInSlowDistance; }
	 }

	 public FsmFloat _SlowFactor = 0.25f;
	 public float SlowFactor
	 {
		get { return _SlowFactor.Value; }
		set { _SlowFactor.Value = value; }
	 }
	 protected float mTargetDistance = 0f;

	 protected Vector3 _targetVector = Vector3.zero;
	 public Vector3 TargetPosition
	 {
		get => _targetPosition;
		set
		{
		  _targetPosition = value;
		}
	 }

	 public override void OnEnter()
	 {
		base.OnEnter();
	 }

	 public override void OnUpdate()
	 {
		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;

		_targetVector = _targetPosition - _go.transform.position;

		if (_ignoreY.Value)
		  _targetVector.y = 0;

		mTargetDistance = _targetVector.magnitude;

		CalculateMove(_targetPosition, ref lMovement, ref lRotation);

		_actor.Move(lMovement);
		_actor.Rotate(lRotation);

		if (mTargetDistance < StopDistance)
		{
		  Fsm.Event(OnComplete);
		}

	 }
	 protected virtual void CalculateMove(Vector3 rWaypoint, ref Vector3 rMove, ref Quaternion rRotate)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		// Direction we need to travel in
		Vector3 lDirection = rWaypoint - _go.transform.position;
		lDirection.y = lDirection.y - 0.05f;
		lDirection.Normalize();

		// Determine our rotation
		Vector3 lVerticalDirection = Vector3.Project(lDirection, _go.transform.up);
		Vector3 lLateralDirection = lDirection - lVerticalDirection;

		float lYawAngle = Vector3Ext.SignedAngle(_go.transform.forward, lLateralDirection);

		if (rotationSpeed.Value == 0f)
		{
		  rRotate = Quaternion.AngleAxis(lYawAngle, _go.transform.up);
		}
		else
		{
		  rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), rotationSpeed.Value * lDeltaTime), _go.transform.up);
		}

		// Grab the base movement speed
		float lMoveSpeed = movementSpeed.Value;

		// Calculate our own slowing
		float lRelativeMoveSpeed = 1f;
		if (mIsInSlowDistance && SlowFactor > 0f)
		{
		  float lSlowPercent = (mTargetDistance - StopDistance) / (SlowDistance - StopDistance);
		  lRelativeMoveSpeed = ((1f - SlowFactor) * lSlowPercent) + SlowFactor;
		}

		// TRT 4/5/2016: Force the slow distance as an absolute value
		if (mIsInSlowDistance && SlowFactor > 0f)
		{
		  lMoveSpeed = SlowFactor;
		  lRelativeMoveSpeed = 1f;
		}

		// Set the final velocity based on the future rotation
		Quaternion lFutureRotation = _go.transform.rotation * rRotate;
		rMove = lFutureRotation.Forward() * (lMoveSpeed * lRelativeMoveSpeed * lDeltaTime);

	 }


  }
}