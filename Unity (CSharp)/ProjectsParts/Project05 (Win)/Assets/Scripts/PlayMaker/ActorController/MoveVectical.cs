using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using com.ootii.Actors;
using com.ootii.Timing;
using System;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  public class MoveVectical : ActorDriver
  {
    public FsmBool isVerticalMove = new FsmBool(true);
    protected Vector3 mTargetVector = Vector3.zero;
    public FsmBool _activateGroundAfter = new FsmBool(false);

    public override void OnEnter()
    {
      base.OnEnter();
      _actor.IsGravityEnabled = false;
      _actor.ForceGrounding = false;
      _actor.FixGroundPenetration = false;
    }

    public override void OnUpdate()
    {
      base.OnUpdate();
      Vector3 lMovement = Vector3.zero;
      Quaternion lRotation = Quaternion.identity;

      mTargetVector = isVerticalMove.Value ? Vector3.up : Vector3.down;

      CalculateMove(_go.transform.position + mTargetVector, ref lMovement, ref lRotation);

      _actor.Move(lMovement);
      _actor.Rotate(lRotation);
    }

	 public override void OnExit()
	 {
		base.OnExit();
      if (_activateGroundAfter.Value)
      {
        _actor.IsGravityEnabled = true;
        _actor.ForceGrounding = true;
        _actor.FixGroundPenetration = true;
      }
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

      float lYawAngle = Vector3Ext.SignedAngle(_go.transform.forward, lLateralDirection);

      if (rotationSpeed.Value == 0f)
      {
        rRotate = Quaternion.AngleAxis(lYawAngle, _go.transform.up);
      }
      else
      {
        rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), rotationSpeed.Value * lDeltaTime), _go.transform.up);
      }


      // Set the final velocity based on the future rotation
      Quaternion lFutureRotation = _go.transform.rotation * rRotate;
      rMove = lVerticalDirection * (movementSpeed.Value * lDeltaTime);

    }

    public bool CheckMoveReady(float height) {

      Vector3 startPos = height > 0 ? (_go.transform.position + Vector3.up * _actor.Height) : _go.transform.position;
      RaycastHit hit;
      if(RaycastExt.SafeSphereCast(startPos, Vector3.up * Mathf.Sign(height),_actor.Radius, out hit, height, -1, _go.transform)){
        Debug.Log(hit.collider.gameObject.name);
        return true;
      }
      return false;
    }

  }
}