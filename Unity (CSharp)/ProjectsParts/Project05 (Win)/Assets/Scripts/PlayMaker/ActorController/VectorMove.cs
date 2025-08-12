using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using com.ootii.Actors;
using com.ootii.Timing;
using com.ootii.Geometry;
using com.ootii.Timing;
namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  public class VectorMove : ActorDriver
  {
	 public FsmVector3 moveVector;
    protected Vector3 mTargetVector = Vector3.zero;
    public FsmBool _rotate;
    public FsmBool _rotateToTarget;
    public FsmGameObject _targetObject;
    public FsmBool _relative = new FsmBool(true);


    public override void OnUpdate()
    {
      base.OnUpdate();
      Vector3 lMovement = Vector3.zero;
      Quaternion lRotation = Quaternion.identity;

      mTargetVector = moveVector.Value;

      CalculateMove(_go.transform.position + mTargetVector, ref lMovement, ref lRotation);

      _actor.Move(lMovement);
      _actor.Rotate(lRotation);

    }
    protected virtual void CalculateMove(Vector3 rWaypoint, ref Vector3 rMove, ref Quaternion rRotate)
    {
      float lDeltaTime = TimeManager.SmoothedDeltaTime;

      // Direction we need to travel in
      Vector3 lDirection = rWaypoint - _go.transform.position;
      lDirection.Normalize();

      // Determine our rotation
      Vector3 lVerticalDirection = Vector3.Project(lDirection, _go.transform.up);
      Vector3 lLateralDirection = lDirection - lVerticalDirection;

		if (_rotateToTarget.Value)
      {
        Vector3 lDirectionT = _targetObject.Value.transform.position - _go.transform.position;
        lDirectionT.Normalize();

        // Determine our rotation
        Vector3 lVerticalDirectionT = Vector3.Project(lDirectionT, _go.transform.up);
        lLateralDirection = lDirectionT - lVerticalDirectionT;

      }

      float lYawAngle = Vector3Ext.SignedAngle(_go.transform.forward, lLateralDirection);

      if (_rotate.Value)
      {

        if (rotationSpeed.Value == 0f)
        {
          rRotate = Quaternion.AngleAxis(lYawAngle, _go.transform.up);
        }
        else
        {
          rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), rotationSpeed.Value * lDeltaTime), _go.transform.up);
        }
      }


      // Set the final velocity based on the future rotation
      Quaternion lFutureRotation = _go.transform.rotation * rRotate;

      rMove = (_relative.Value ? _go.transform.rotation : Quaternion.identity ) * lDirection * (movementSpeed.Value * lDeltaTime);

    }

  }
}