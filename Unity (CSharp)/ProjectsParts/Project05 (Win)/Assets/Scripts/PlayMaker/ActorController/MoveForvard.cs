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
  public class MoveForvard : ActorDriver
  {

	 public override void OnUpdate()
	 {
		base.OnUpdate();

		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;

		CalculateMove(_go.transform.position + _go.transform.forward, ref lMovement, ref lRotation);

		_actor.Move(lMovement);


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

		//float lYawAngle = Vector3Ext.SignedAngle(go.transform.forward, lLateralDirection);

		//if (rotationSpeed.Value == 0f)
		//{
		//  rRotate = Quaternion.AngleAxis(lYawAngle, go.transform.up);
		//}
		//else
		//{
		//  rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), rotationSpeed.Value * lDeltaTime), go.transform.up);
		//}

		// Grab the base movement speed
		float lMoveSpeed = movementSpeed.Value;

		// Calculate our own slowing
		//float lRelativeMoveSpeed = 1f;
		//if (mIsInSlowDistance && SlowFactor > 0f)
		//{
		//  float lSlowPercent = (mTargetDistance - StopDistance) / (SlowDistance - StopDistance);
		//  lRelativeMoveSpeed = ((1f - SlowFactor) * lSlowPercent) + SlowFactor;
		//}

		//// TRT 4/5/2016: Force the slow distance as an absolute value
		//if (mIsInSlowDistance && SlowFactor > 0f)
		//{
		//  lMoveSpeed = SlowFactor;
		//  lRelativeMoveSpeed = 1f;
		//}

		// Set the final velocity based on the future rotation
		//Quaternion lFutureRotation = go.transform.rotation * rRotate;
		rMove = _go.transform.forward * (lMoveSpeed * lDeltaTime);

	 }

  }
}