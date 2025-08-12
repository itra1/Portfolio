using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Timing;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory(ActionCategory.Character)]
  [HutongGames.PlayMaker.Tooltip("Актор контроллер. Поворот к указателю")]
  public class RotateToTarget : ActorDriver
  {

	 public FsmGameObject _target;
	 public FsmEvent _onComplete;
	 public FsmFloat targetAngle = new FsmFloat(5);

	 public FsmFloat _minAngleToRotate = new FsmFloat(40);

	 private bool _isComplete;
	 protected Vector3 _targetVector = Vector3.zero;

	 private bool _isRotate = false;

	 public override void OnUpdate()
    {
		base.OnUpdate();

		_isComplete = false;

		Quaternion lRotation = Quaternion.identity;

		if (_target.Value == null)
		  return;

		_targetVector = _target.Value.transform.position - _go.transform.position;

		CalculateRotate(_target.Value.transform.position, ref lRotation);

		if (_isRotate)
		{
		  _actor.Rotate(lRotation);

		}
		if (_isComplete)
		{
		  Fsm.Event(_onComplete);
		}

	 }

	 protected virtual void CalculateRotate(Vector3 target, ref Quaternion rRotate)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		// Direction we need to travel in
		Vector3 lDirection = target - _go.transform.position;
		lDirection.y = lDirection.y - 0.05f;
		lDirection.Normalize();

		// Determine our rotation
		Vector3 lVerticalDirection = Vector3.Project(lDirection, _go.transform.up);
		Vector3 lLateralDirection = lDirection - lVerticalDirection;

		float lYawAngle = Vector3Ext.SignedAngle(_go.transform.forward, lLateralDirection);

		if (Mathf.Abs(lYawAngle) > _minAngleToRotate.Value)
		{
		  _isRotate = true;
		}
		else
		{
		  _isComplete = true;
		}

		if (rotationSpeed.Value == 0f)
		{
		  rRotate = Quaternion.AngleAxis(lYawAngle, _go.transform.up);
		}
		else
		{
		  rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), rotationSpeed.Value * lDeltaTime), Owner.transform.up);
		}

		if(Mathf.Abs(Vector3Ext.AngleTo(_go.transform.forward, lLateralDirection)) < targetAngle.Value)
		{
		  _isComplete = true;
		  _isRotate = false;
		}

	 }

  }
}